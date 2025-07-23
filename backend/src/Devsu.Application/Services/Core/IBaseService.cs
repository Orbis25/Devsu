namespace Devsu.Application.Services.Core;

public interface IBaseService<TGet>
    where TGet : class
{
    Task<Response<TGet>>  GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Response> RemoveAsync(Guid id, CancellationToken cancellationToken);
    Task<Response<PaginationResult<TGet>>> GetAllAsync(Paginate paginate,
        CancellationToken cancellationToken = default);
}