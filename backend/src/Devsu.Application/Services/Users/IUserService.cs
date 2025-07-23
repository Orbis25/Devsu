namespace Devsu.Application.Services.Users;

public interface IUserService : IBaseService<GetUser>
{
    Task<Response<Guid>> CreateAsync(CreateUser input, CancellationToken cancellationToken);
    Task<Response> UpdateAsync(Guid id, EditUser input, CancellationToken cancellationToken = default);
}