using Devsu.Application.Dtos.Users;

namespace Devsu.Application.Services.Users;

public interface IUserService
{
    Task<Response<Guid>> CreateAsync(CreateUser input, CancellationToken cancellationToken);
    Task<Response<GetUser>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Response> RemoveAsync(Guid id, CancellationToken cancellationToken);
    Task<Response> UpdateAsync(Guid id, EditUser input, CancellationToken cancellationToken = default);

    Task<Response<PaginationResult<GetUser>>> GetAllAsync(Paginate paginate,
        CancellationToken cancellationToken = default);
}