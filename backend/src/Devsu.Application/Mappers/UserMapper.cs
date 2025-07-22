namespace Devsu.Application.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<CreateUser, User>().ReverseMap();
        CreateMap<GetUser, User>().ReverseMap();
    }
}