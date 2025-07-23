using Devsu.Application.Dtos.Core;

namespace Devsu.Infrastructure.EF.Repositories;

public class TransactionRepository : BaseRepository<ApplicationDbContext,Transaction>, ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext context) : base(context)
    {
    }
    
}