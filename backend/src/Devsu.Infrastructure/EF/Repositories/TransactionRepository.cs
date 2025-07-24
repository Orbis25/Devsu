using System.Globalization;
using System.Linq.Expressions;
using Devsu.Application.Dtos.Core;

namespace Devsu.Infrastructure.EF.Repositories;

public class TransactionRepository : BaseRepository<ApplicationDbContext,Transaction>, ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    
    public override async Task<PaginationResult<Transaction>> GetPaginatedListAsync(Paginate paginate,
        Expression<Func<Transaction, bool>>? expression = default,
        CancellationToken cancellationToken = default)
    {
        var results = GetAll(expression).Include(x => x.Account)
            .ThenInclude(x => x!.User).AsQueryable();

        if (!string.IsNullOrEmpty(paginate.Query))
        {
            paginate.Query = paginate.Query.ToLowerInvariant();

            results = results.Where(x => x.Type!.ToLower().Contains(paginate.Query) ||
                                         (!string.IsNullOrEmpty(x.Movement) && x.Movement.ToLower().Contains(paginate.Query)) ||
                                         x.Amount.ToString(CultureInfo.CurrentCulture).ToLower().Contains(paginate.Query));
        }

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
    
}