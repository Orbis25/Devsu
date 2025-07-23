using Devsu.Application.Dtos.Transactions;

namespace Devsu.Application.Mappers;

public class TransactionMapper : Profile
{
    public TransactionMapper()
    {
        CreateMap<GetTransaction, Transaction>().ReverseMap();
    }
}