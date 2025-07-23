
using Devsu.Application.Dtos.Accounts;

namespace Devsu.Application.Services.Accounts;

public interface IAccountService: IBaseService<GetAccount>
{
    Task<Response<Guid>> CreateAsync(CreateAccount input, CancellationToken cancellationToken);
    Task<Response> UpdateAsync(Guid id, EditAccount input, CancellationToken cancellationToken = default);
}