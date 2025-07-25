using Devsu.Application.Dtos.Accounts;

namespace Devsu.Application.Mappers;

public class AccountMapper : Profile
{
    public AccountMapper()
    {
        CreateMap<CreateAccount, Account>().ReverseMap();
        CreateMap<GetAccount, Account>().ReverseMap();
        CreateMap<GetAccountTransaction, Account>().ReverseMap();
    }
}