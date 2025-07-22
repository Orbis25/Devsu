using System.Linq.Expressions;
using Devsu.Application.Dtos.Core;
using Domain.Core.Models;

namespace Devsu.Application.Repositories.Core;

public interface IBaseRepository<TModel>
    where TModel : BaseModel
{
    Task<PaginationResult<TModel>> GetPaginatedListAsync(Paginate paginate,
        Expression<Func<TModel, bool>>? expression = default, 
        CancellationToken cancellationToken = default);
    IQueryable<TModel> GetAll(Expression<Func<TModel, bool>>? expression = default);
    Task<TModel> CreateAsync(TModel model, CancellationToken cancellationToken = default);
    Task<TModel?> UpdateAsync(TModel model, CancellationToken cancellationToken = default);
    Task<bool> SoftRemoveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistAsync(Expression<Func<TModel, bool>>? expression = default, CancellationToken cancellationToken = default);
    Task<TModel?> GetOneAsync(Expression<Func<TModel, bool>> expression, CancellationToken cancellationToken = default);
}