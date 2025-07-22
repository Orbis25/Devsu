using System.Linq.Expressions;
using Devsu.Application.Dtos.Core;
using Devsu.Application.Repositories.Core;
using Domain.Core.Models;

namespace Devsu.Infrastructure.EF.Repositories.Core;

public abstract class BaseRepository<TContext, TModel> : IBaseRepository<TModel>
    where TContext : DbContext
    where TModel : BaseModel
{
    private readonly TContext _context;
    protected BaseRepository(TContext context)
    {
        _context = context;
    }
    
    public virtual async Task<TModel> CreateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        _context.Set<TModel>().Add(model);

        await _context.SaveChangesAsync(cancellationToken);

        return model;
    }

    public virtual IQueryable<TModel> GetAll(Expression<Func<TModel, bool>>? expression = default)
    {
        var results = _context.Set<TModel>().AsQueryable();
        results = expression != null ? results.Where(expression) : results.OrderBy(x => x.CreatedAt);
        return results;
    }

    public virtual async Task<PaginationResult<TModel>> GetPaginatedListAsync(Paginate paginate,
        Expression<Func<TModel, bool>>? expression = default, CancellationToken cancellationToken = default)
    {
        var results = GetAll(expression);

        if (paginate.NoPaginate)
        {
            return new()
            {
                Results = await results.AsNoTracking().ToListAsync(cancellationToken)
            };
        }

        var total = results.Count();
        var pages = (int)Math.Ceiling((decimal)total / paginate.Qyt);

        results = results.Skip((paginate.Page - 1) * paginate.Qyt).Take(paginate.Qyt);
        
        return new()
        {
            ActualPage = paginate.Page,
            Qyt = paginate.Qyt,
            PageTotal = pages,
            Total = total,
            Results = await results.AsNoTracking().ToListAsync(cancellationToken)
        };
    }
    
    public virtual async Task<TModel?> UpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        _context.Set<TModel>().Update(model);
        await _context.SaveChangesAsync(cancellationToken);
        return model;
    }
    

    public virtual async Task<bool> SoftRemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetOneAsync(x => x.Id == id,cancellationToken);

        if (entity == null) return false;

        entity.IsDeleted = true;

        _context.Set<TModel>().Update(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return true;

    }
    
    public async Task<bool> ExistAsync(Expression<Func<TModel, bool>>? expression = default, CancellationToken cancellationToken = default)
    {
        var result = _context.Set<TModel>();

        if (expression != null)
        {
            return await result.AnyAsync(expression, cancellationToken);
        }

        return await result.AnyAsync(cancellationToken);
    }

    public Task<TModel?> GetOneAsync(Expression<Func<TModel, bool>> expression, CancellationToken cancellationToken = default)
    {
        return _context.Set<TModel>().FirstOrDefaultAsync(expression, cancellationToken);
    }
}
