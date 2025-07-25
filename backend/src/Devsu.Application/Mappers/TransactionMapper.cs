using Devsu.Application.Dtos.Transactions;

namespace Devsu.Application.Mappers;

public class TransactionMapper : Profile
{
    public TransactionMapper()
    {
        CreateMap<GetTransaction, Transaction>().ReverseMap();
        CreateMap<Transaction, ExportTransactionSearchResponse>()
            .ForMember(dest => dest.ClientName, opt => 
                opt.MapFrom(src => src.Account!.User!.Name ?? "")
            )
            .ReverseMap();
        CreateMap<SearchTransactions, Transaction>().ReverseMap();
    }
}