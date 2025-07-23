using Devsu.Application.Dtos.Transactions;

namespace Devsu.Application.Services.Transactions;

public interface ITransactionService : IBaseService<GetTransaction>
{
    Task<Response<Guid>> CreateAsync(CreateTransaction input, CancellationToken cancellationToken);
    Task<Response> UpdateAsync(Guid id, EditTransaction input, CancellationToken cancellationToken = default);
}