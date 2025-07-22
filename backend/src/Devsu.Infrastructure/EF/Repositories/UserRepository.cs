namespace Devsu.Infrastructure.EF.Repositories;

public class UserRepository : BaseRepository<ApplicationDbContext,User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }
}