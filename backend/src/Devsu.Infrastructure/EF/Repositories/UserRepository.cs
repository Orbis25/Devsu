using System.Linq.Expressions;
using Devsu.Application.Dtos.Core;

namespace Devsu.Infrastructure.EF.Repositories;

public class UserRepository : BaseRepository<ApplicationDbContext, User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<PaginationResult<User>> GetPaginatedListAsync(Paginate paginate,
        Expression<Func<User, bool>>? expression = null,
        CancellationToken cancellationToken = default)
    {
        var results = GetAll(expression)
            .OrderByDescending(x => x.CreatedAt).AsQueryable();

        if (!string.IsNullOrEmpty(paginate.Query))
        {
            paginate.Query = paginate.Query.ToLower();

            results = results.Where(x => x.Name!.ToLower().Contains(paginate.Query) ||
                                         x.ClientId!.ToLower().Contains(paginate.Query) ||
                                         x.Age.ToString().Contains(paginate.Query) ||
                                         x.Gender!.ToLower().Contains(paginate.Query) ||
                                         x.Identification!.ToLower().Contains(paginate.Query) ||
                                         (!string.IsNullOrEmpty(x.Phone) &&
                                          x.Phone.ToLower().Contains(paginate.Query)) ||
                                         (!string.IsNullOrEmpty(x.Address) &&
                                          x.Address.ToLower().Contains(paginate.Query)));
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