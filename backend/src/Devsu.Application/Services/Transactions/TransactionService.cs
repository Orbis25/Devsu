using System.Text;
using System.Text.Json;
using Devsu.Application.Dtos.Transactions;
using Devsu.Application.Extensions;
using Devsu.Application.Resources;
using Devsu.Application.Services.Core.Pdf;
using Domain.Enums;

namespace Devsu.Application.Services.Transactions;

public class TransactionService : BaseService<Transaction, GetTransaction, ITransactionRepository>, ITransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<TransactionService> _logger;
    private readonly IPdfService _pdfService;
    private readonly IMapper _mapper;

    public TransactionService(ITransactionRepository repository, IMapper mapper, ILogger<TransactionService> logger,
        IAccountRepository accountRepository, IPdfService pdfService) : base(repository, mapper, logger)
    {
        _repository = repository;
        _logger = logger;
        _accountRepository = accountRepository;
        _pdfService = pdfService;
        _mapper = mapper;
    }

    public async Task<Response<Guid>> CreateAsync(CreateTransaction input, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating transaction with AccountId: {AccountId}", input.AccountId);

            var account = await _accountRepository.GetOneAsync(x => x.Id == input.AccountId, cancellationToken);

            if (account is null)
            {
                _logger.LogWarning("Account with Id {AccountId} not found", input.AccountId);
                return new("Account not found") { IsNotFound = true };
            }

            var isDebit = input.Type.ToLowerInvariant() == TransactionType.Debit.GetDisplay();

            //if is a debit transaction, check if the account has sufficient balance
            if (account.CurrentBalance <= 0 && isDebit)
            {
                _logger.LogWarning("Account with Id {AccountId} has insufficient balance", input.AccountId);
                return new("Saldo no disponible");
            }

            if (isDebit)
            {
                var (limitMessage, limitResult) = CanCreateOrUpdateTransaction(input, account);

                if (!limitResult)
                {
                    _logger.LogWarning("User with AccountId {AccountId} has exceeded transaction limits: {Message}",
                        account.Id,
                        limitMessage);
                    return new(limitMessage);
                }
            }

            // Update the account's current balance based on the transaction type
            switch (isDebit)
            {
                case false:
                    account.CurrentBalance += input.Amount;
                    break;
                case true:
                    account.CurrentBalance -= input.Amount;
                    break;
            }

            var data = new Transaction
            {
                AccountId = account.Id,
                Amount = input.Amount,
                Type = input.Type.ToLowerInvariant(),
                CurrentBalance = account.CurrentBalance,
                Movement = isDebit
                    ? $"Retiro de {input.Amount}"
                    : $"Deposito de {input.Amount}",
            };

            var result = _repository.Attach(data);

            

            await _repository.CommitAsync(cancellationToken).ConfigureAwait(false);

            return new(result.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating transaction: {Message}", e.Message);

            return new("Error creating transaction");
        }
    }

    public async Task<Response> UpdateAsync(Guid id, EditTransaction input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating account with id {Id}", id);
            var account = await _accountRepository.GetOneAsync(x => x.Id == input.AccountId, cancellationToken);

            if (account is null)
            {
                _logger.LogWarning("Account with Id {Id} not found", id);
                return new() { Message = "Account no encontrada", IsNotFound = true };
            }

            var transaction = await _repository.GetOneAsync(x => x.Id == id, cancellationToken);

            if (transaction == null)
            {
                _logger.LogWarning("Transaction with Id {Id} not found", id);
                return new() { Message = "Transaction no encontrada", IsNotFound = true };
            }


            var isDebit = input.Type.ToLowerInvariant() == TransactionType.Debit.GetDisplay();

            if (isDebit)
            {
                var trx = new CreateTransaction
                {
                    Amount = input.Amount - transaction.Amount,
                    Type = input.Type,
                    AccountId = input.AccountId
                };

                var (limitMessage, limitResult) = CanCreateOrUpdateTransaction(trx, account, false);

                if (!limitResult)
                {
                    _logger.LogWarning("User with AccountId {AccountId} has exceeded transaction limits: {Message}",
                        account.Id,
                        limitMessage);
                    return new() { Message = limitMessage };
                }
            }


            // handler transaction reversal if the type or amount has changed
            await TransactionHandlerAsync(input, transaction, cancellationToken).ConfigureAwait(false);
            
            transaction.CurrentBalance = account.CurrentBalance;
            transaction.Type = input.Type;
            transaction.Amount = input.Amount;
            transaction.AccountId = input.AccountId;
            transaction.Movement = input.Type.ToLowerInvariant() == TransactionType.Debit.GetDisplay()
                ? $"Retiro de {input.Amount}"
                : $"Deposito de {input.Amount}";


            _repository.Attach(transaction);

            // Apply changes
            await _repository.CommitAsync(cancellationToken).ConfigureAwait(false);

            return new();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating transaction: {Message}", e.Message);
            return new() { Message = "Error updating transaction" };
        }
    }

    public override async Task<Response> RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            //update the account balance before removing the transaction
            var transaction = await _repository.GetOneAsync(x => x.Id == id, cancellationToken);

            if (transaction == null)
            {
                _logger.LogWarning("Transaction with Id {Id} not found", id);
                return new() { Message = "Transaction no encontrada", IsNotFound = true };
            }

            // Reverse the transaction amount from the account
            await ApplyUpdateToAccountAsync(transaction.AccountId!.Value, transaction, true, cancellationToken)
                .ConfigureAwait(false);
            
            if (transaction.Type.ToLowerInvariant() == TransactionType.Debit.GetDisplay() && transaction.Account is not null)
            {
                ReverseAccountLimit(transaction.Account, transaction);
            }
            
            var result = await _repository.SoftRemoveAsync(id, cancellationToken);
            return new();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error removing transaction: {Message}", e.Message);
            return new() { Message = "Error removing transaction" };
        }
    }


    public async Task<Response<PaginationResult<T>>> SearchAsync<T>(SearchTransactions search,
        CancellationToken cancellationToken = default) where T : GetTransaction, new()
    {
        try
        {
            var hasDateRange = search is { From: not null, To: not null };
            var from = hasDateRange ? search.From!.Value.Date : DateTime.MinValue;
            var to = hasDateRange ? search.To!.Value.Date : DateTime.MaxValue;

            var results = await _repository.GetPaginatedListAsync(search,
                    x => x.CreatedAt.Date >= from.Date && x.CreatedAt <= to.Date, cancellationToken)
                .ConfigureAwait(false);

            var mappedResults = _mapper.Map<List<T>>(results.Results);

            return new()
            {
                Data = new()
                {
                    ActualPage = results.ActualPage,
                    Qyt = results.Qyt,
                    PageTotal = results.PageTotal,
                    Total = results.Total,
                    Results = mappedResults
                }
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error searching transactions: {Message}", e.Message);
            return new();
        }
    }


    public async Task<Response<string>> ExportTransactionReportAsync(ExportTransactionsSearch search, bool isPdf = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Exporting transaction report with search criteria: {Search}", search.Query);


            var results = await SearchAsync<ExportTransactionSearchResponse>(new()
            {
                Query = search.Query,
                From = search.From,
                To = search.To,
                NoPaginate = true
            }, cancellationToken).ConfigureAwait(false);

            var items = results.Data?.Results ?? [];

            if (!isPdf)
            {
                var json = JsonSerializer.Serialize(items, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });
                
                return new()
                {
                    Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
                };
            }
            
            
            var sb = new StringBuilder();

            foreach (var transaction in items)
            {
                //body of table
                
                var isDebit = transaction.Type!.ToLowerInvariant() == TransactionType.Debit.GetDisplay();
                var movement = isDebit
                    ? $"-{transaction.Amount}"
                    : $"{transaction.Amount}";

                sb.AppendLine(
                    $@"<tr>
                        <td style=""border: 1px solid #999; padding: 8px;"">{transaction.CreatedAt:dd-MM-yyyy}</td>
                        <td style=""border: 1px solid #999; padding: 8px;"">{transaction.ClientName}</td>
                        <td style=""border: 1px solid #999; padding: 8px;"">{transaction.Account?.AccountNumber}</td>
                        <td style=""border: 1px solid #999; padding: 8px;"">{transaction.Account?.AccountType}</td>
                        <td style=""border: 1px solid #999; padding: 8px;"">{transaction.Account?.InitialBalance}</td>
                        <td style=""border: 1px solid #999; padding: 8px;"">{transaction.Status.ToString()}</td>
                        <td style=""border: 1px solid #999; padding: 8px;"">{movement}</td>
                        <td style=""border: 1px solid #999; padding: 8px;"">{transaction.CurrentBalance}</td>
                        </tr>");
            }

            var body = sb.ToString();
            var html = HtmlResources.TransactionsHtml.Replace("[body]", body);

            return new()
            {
                Data = _pdfService.GeneratePdfAsBase64(html)
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error exporting transaction report: {Message}", e.Message);
            return new() { Message = "Error exporting transaction report" };
        }
    }

    private async Task TransactionHandlerAsync(EditTransaction edit, Transaction transaction,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Reversing transaction with Id {Id}", transaction.Id);

        if (edit.Type == transaction.Type && edit.Amount == transaction.Amount &&
            edit.AccountId == transaction.AccountId)
        {
            _logger.LogInformation("No changes detected for transaction with Id {Id}, skipping reversal",
                transaction.Id);
            return;
        }

        var accountNew = await _accountRepository.GetOneAsync(x => x.Id == edit.AccountId, cancellationToken);

        if (accountNew == null)
        {
            _logger.LogWarning("Account with Id {AccountId} not found", transaction.AccountId);
            return;
        }

        if (edit.AccountId != transaction.AccountId)
        {
            await ApplyUpdateToAccountAsync(edit.AccountId, transaction, false, cancellationToken)
                .ConfigureAwait(false);
            await ApplyUpdateToAccountAsync(transaction.AccountId!.Value, transaction, true, cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            ApplyTransactionReversal(accountNew, transaction);
            ApplyTransaction(edit, accountNew);

            // Update the account balance
            _accountRepository.Attach(accountNew);
        }

        // Reverse the user limit if the transaction type is debit
        var isDebit = transaction.Type!.ToLowerInvariant() == TransactionType.Debit.GetDisplay();
        if (isDebit && accountNew.Id != transaction.AccountId)
        {
            var oldAccount = await _accountRepository.GetOneAsync(x => x.Id == transaction.AccountId, cancellationToken)
                .ConfigureAwait(false);

            if (oldAccount is not null)
            {
                ReverseAccountLimit(oldAccount, transaction);
            }

            ApplyAccountLimit(accountNew, edit);
        }
        else if (isDebit)
        {
            ReverseAccountLimit(accountNew, transaction);
            ApplyAccountLimit(accountNew, edit);
        }
    }

    private async Task ApplyUpdateToAccountAsync(Guid accountId,
        Transaction transaction,
        bool isReversal = false,
        CancellationToken cancellationToken = default)
    {
        var newAccount = await _accountRepository.GetOneAsync(x => x.Id == accountId, cancellationToken);

        if (newAccount == null)
        {
            _logger.LogWarning("New account with Id {AccountId} not found", accountId);
            return;
        }

        // Apply the transaction amount to the new account
        if (transaction.Type!.ToLowerInvariant() == TransactionType.Credit.GetDisplay())
        {
            if (isReversal)
            {
                newAccount.CurrentBalance -= transaction.Amount;
            }
            else
            {
                newAccount.CurrentBalance += transaction.Amount;
            }
        }
        else if (transaction.Type.ToLowerInvariant() == TransactionType.Debit.GetDisplay())
        {
            if (isReversal)
            {
                newAccount.CurrentBalance += transaction.Amount;
            }
            else
            {
                newAccount.CurrentBalance -= transaction.Amount;
            }
        }

        // Update the new account balance
        _accountRepository.Attach(newAccount);
    }


    private static void ApplyTransaction(EditTransaction edit, Account account)
    {
        if (edit.Type.ToLowerInvariant() == TransactionType.Credit.GetDisplay())
            account.CurrentBalance += edit.Amount;
        else if (edit.Type.ToLowerInvariant() == TransactionType.Debit.GetDisplay())
            account.CurrentBalance -= edit.Amount;
    }

    private static void ApplyTransactionReversal(Account account, Transaction transaction)
    {
        if (transaction.Type!.ToLowerInvariant() == TransactionType.Credit.GetDisplay())
            account.CurrentBalance -= transaction.Amount;
        else if (transaction.Type.ToLowerInvariant() == TransactionType.Debit.GetDisplay())
            account.CurrentBalance += transaction.Amount;
    }

    private (string, bool) CanCreateOrUpdateTransaction(CreateTransaction transaction, Account account,
        bool isCreate = true)
    {
        //check daily transaction limit
        if (transaction.Amount > account.DailyDebitLimit)
        {
            _logger.LogWarning("User with AccountId {AccountId} has exceeded daily debit limit", account.Id);
            return ("Cupo diario Excedido", false);
        }

        var consumedToday = account.DailyDebit + transaction.Amount;
        //check if the user has exceeded the daily debit limit after the transaction
        if (consumedToday > account.DailyDebitLimit)
        {
            _logger.LogWarning("User with AccountId {AccountId} has exceeded daily debit limit after transaction",
                account.Id);

            var current = account.DailyDebitLimit - account.DailyDebit;
            var msg = "Cupo diario Excedido";

            if (current > 0)
            {
                msg = $"Cupo diario Excedido, solo tiene permitido retirar {current}";
            }

            return (msg, false);
        }

        if (isCreate)
        {
            ApplyAccountLimit(account, transaction);
        }

        return (string.Empty, true);
    }

    private void ApplyAccountLimit(Account account, CreateTransaction transaction)
    {
        account.DailyDebit += transaction.Amount;
        _accountRepository.Attach(account);
    }

    private void ReverseAccountLimit(Account account, Transaction transaction)
    {
        account.DailyDebit -= transaction.Amount;
        _accountRepository.Attach(account);
    }
}