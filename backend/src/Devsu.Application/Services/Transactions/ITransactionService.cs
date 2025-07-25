using Devsu.Application.Dtos.Transactions;

namespace Devsu.Application.Services.Transactions;

public interface ITransactionService : IBaseService<GetTransaction>
{
    Task<Response<Guid>> CreateAsync(CreateTransaction input, CancellationToken cancellationToken);
    Task<Response> UpdateAsync(Guid id, EditTransaction input, CancellationToken cancellationToken = default);

    Task<Response<string>> ExportTransactionReportAsync(ExportTransactionsSearch search,
        CancellationToken cancellationToken = default);

    Task<Response<PaginationResult<T>>> SearchAsync<T>(SearchTransactions paginate,
        CancellationToken cancellationToken = default) where T : GetTransaction, new();
}