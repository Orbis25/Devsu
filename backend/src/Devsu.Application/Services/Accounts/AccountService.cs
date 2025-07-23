using Devsu.Application.Dtos.Accounts;

namespace Devsu.Application.Services.Accounts;

public class AccountService : BaseService<Account, GetAccount, IAccountRepository>, IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IAccountRepository _repository;
    private readonly IMapper _mapper;

    public AccountService(IAccountRepository repository, IMapper mapper, ILogger<AccountService> logger) : base(
        repository, mapper,
        logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Response<Guid>> CreateAsync(CreateAccount input, CancellationToken cancellationToken)
    {
        try
        {
            var exist = await _repository.ExistAsync(x => x.AccountNumber == input.AccountNumber, cancellationToken);

            if (exist)
            {
                _logger.LogWarning("account with AccountNumber {AccountNumber} already exists", input.AccountNumber);

                return new($"Cuenta con AccountNumber ya existe");
            }


            _logger.LogInformation("Creating account with AccountNumber: {AccountNumber}", input.AccountNumber);

            input.CurrentBalance = input.InitialBalance;
            var result = await _repository.CreateAsync(_mapper.Map<Account>(input), cancellationToken);

            return new(result.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating user: {Message}", e.Message);

            return new("Error creating user");
        }
    }

    public async Task<Response> UpdateAsync(Guid id, EditAccount input, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating account with id {Id}", id);
            var account = await _repository.GetOneAsync(x => x.Id == id, cancellationToken);

            if (account == null)
            {
                _logger.LogWarning("Account with Id {Id} not found", id);
                return new() { Message = "Account not found", IsNotFound = true };
            }

            if (account.AccountNumber != input.AccountNumber)
            {
                var exist = await _repository.ExistAsync(x => x.AccountNumber == input.AccountNumber && x.Id != id,
                    cancellationToken);
                if (exist)
                {
                    _logger.LogWarning("Account with AccountNumber {AccountNumber} already exists",
                        input.AccountNumber);
                    return new() { Message = "Cuenta con este numero ya existe" };
                }
            }

            account.AccountNumber = input.AccountNumber;
            account.AccountType = input.AccountType;

            await _repository.UpdateAsync(account, cancellationToken).ConfigureAwait(false);

            return new();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating account: {Message}", e.Message);
            return new() { Message = "Error updating account" };
        }
    }
    
}