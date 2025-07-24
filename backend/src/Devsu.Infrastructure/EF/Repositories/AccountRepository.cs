using System.Globalization;
using System.Linq.Expressions;
using Devsu.Application.Dtos.Core;

namespace Devsu.Infrastructure.EF.Repositories;

public class AccountRepository : BaseRepository<ApplicationDbContext,Account>, IAccountRepository
{
    public AccountRepository(ApplicationDbContext context) : base(context)
    {
    }
    
        
    public override async Task<PaginationResult<Account>> GetPaginatedListAsync(Paginate paginate,
        Expression<Func<Account, bool>>? expression = default,
        CancellationToken cancellationToken = default)
    {
        var results = GetAll(expression);

        if (!string.IsNullOrEmpty(paginate.Query))
        {
            paginate.Query = paginate.Query.ToLowerInvariant();

            results = results.Where(x => x.AccountType!.ToLower().Contains(paginate.Query) ||
                                         (!string.IsNullOrEmpty(x.AccountNumber) && x.AccountNumber.ToLower().Contains(paginate.Query)) ||
                                         x.DailyDebitLimit.ToString(CultureInfo.CurrentCulture).Contains(paginate.Query) || 
                                         x.DailyDebit.ToString(CultureInfo.CurrentCulture).Contains(paginate.Query) || 
                                         x.CurrentBalance.ToString(CultureInfo.CurrentCulture).Contains(paginate.Query) || 
                                         x.InitialBalance.ToString(CultureInfo.CurrentCulture).Contains(paginate.Query)
                                         );
            
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