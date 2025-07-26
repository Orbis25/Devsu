using System.Globalization;
using System.Linq.Expressions;
using Devsu.Application.Dtos.Core;

namespace Devsu.Infrastructure.EF.Repositories;

public class TransactionRepository : BaseRepository<ApplicationDbContext,Transaction>, ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Transaction?> GetOneAsync(Expression<Func<Transaction, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await GetAll(expression)
            .Include(x => x.Account)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public override async Task<PaginationResult<Transaction>> GetPaginatedListAsync(Paginate paginate,
        Expression<Func<Transaction, bool>>? expression = default,
        CancellationToken cancellationToken = default)
    {
        var results = GetAll(expression).OrderByDescending(x => x.CreatedAt)
            .Include(x => x.Account)
            .ThenInclude(x => x!.User).AsQueryable();

        if (!string.IsNullOrEmpty(paginate.Query))
        {
            paginate.Query = paginate.Query.ToLowerInvariant();

            results = results.Where(x => x.Type!.ToLower().Contains(paginate.Query) ||
                                         (!string.IsNullOrEmpty(x.Movement) && x.Movement.ToLower().Contains(paginate.Query)) ||
                                         x.Amount.ToString().ToLower().Contains(paginate.Query) ||
                                         x.Status.ToString().ToLower().Contains(paginate.Query) ||
                                         x.CurrentBalance.ToString().ToLower().Contains(paginate.Query) ||
                                          (x.Account != null && !string.IsNullOrEmpty(x.Account.AccountNumber) && x.Account.AccountNumber.ToLower().Contains(paginate.Query)) ||
                                         (x.Account != null && !string.IsNullOrEmpty(x.Account.AccountType) && x.Account.AccountType.ToLower().Contains(paginate.Query)) ||
                                         (x.Account != null && x.Account.InitialBalance.ToString().ToLower().Contains(paginate.Query)) ||
                                         (x.Account != null && x.Account.User != null && !string.IsNullOrEmpty(x.Account.User.Name) && x.Account.User.Name.ToLower().Contains(paginate.Query)) ||
                                         x.Movement.ToLower().Contains(paginate.Query) ||
                                         x.CreatedAt.ToString().Contains(paginate.Query)
                                         
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