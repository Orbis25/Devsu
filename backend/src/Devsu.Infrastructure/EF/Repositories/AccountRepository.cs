namespace Devsu.Infrastructure.EF.Repositories;

public class AccountRepository : BaseRepository<ApplicationDbContext,Account>, IAccountRepository
{
    public AccountRepository(ApplicationDbContext context) : base(context)
    {
    }
}