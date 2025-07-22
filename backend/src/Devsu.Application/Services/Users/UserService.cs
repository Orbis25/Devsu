namespace Devsu.Application.Services.Users;

public class UserService(IUserRepository repository, ILogger<UserService> logger, IMapper mapper) : IUserService
{
    public async Task<Response<Guid>> CreateAsync(CreateUser input, CancellationToken cancellationToken)
    {
        try
        {
            var exist = await repository.ExistAsync(x => x.Identification == input.Identification
                                                         || x.ClientId == input.ClientId, cancellationToken);

            if (exist)
            {
                logger.LogWarning("User with identification {Identification} already exists", input.Identification);

                return new($"Cliente con identificación o clientId ya existe");
            }


            logger.LogInformation("Creating user with input: {Identification}", input.Identification);

            var result = await repository.CreateAsync(mapper.Map<User>(input), cancellationToken);

            return new(result.Id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating user: {Message}", e.Message);

            return new("Error creating user");
        }
    }
    public async Task<Response<GetUser>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await repository.GetOneAsync(x => x.Id == id, cancellationToken);
            if (result == null)
            {
                logger.LogWarning("User with Id {Id} not found", id);
                return new("User not found") { IsNotFound = true };
            }

            var user = mapper.Map<GetUser>(result);

            return new(user);
        }
        catch (Exception e)
        {
            logger.LogError(e, " Error getting user by Id: {Message}", e.Message);
            return new("Error getting user by Id");
        }
    }
    public async Task<Response> RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Removing user with id {Id}", id);
            var result = await repository.SoftRemoveAsync(id, cancellationToken);

            if (!result)
            {
                logger.LogWarning("User with Id {Id} not found or already removed", id);
                return new() { Message = "User not found or already removed", IsNotFound = true };
            }

            return new();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error removing user: {Message}", e.Message);
            return new() { Message = "Error removing user" };
        }
    }
    public async Task<Response> UpdateAsync(Guid id, EditUser input, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Updating user with id {Id}", id);
            var user = await repository.GetOneAsync(x => x.Id == id, cancellationToken);

            if (user == null)
            {
                logger.LogWarning("User with Id {Id} not found", id);
                return new() { Message = "User not found", IsNotFound = true };
            }

            if (user.ClientId != input.ClientId)
            {
                var exist = await repository.ExistAsync(x => x.ClientId == input.ClientId && x.Id != id, cancellationToken);
                
                if (exist)
                {
                    logger.LogWarning("User with ClientId {ClientId} already exists", input.ClientId);
                    return new() { Message = "ClientId ya existe" };
                }
            }

            if (user.Identification != input.Identification)
            {
                var exist = await repository.ExistAsync(x => x.Identification == input.Identification && x.Id != id, cancellationToken);
                
                if (exist)
                {
                    logger.LogWarning("User with Identification {Identification} already exists", input.Identification);
                    return new() { Message = "Identification ya existe" };
                }
            }

            // Update user properties
            user.Name = input.Name;
            user.Gender = input.Gender;
            user.Phone = input.Phone;
            user.Identification = input.Identification;
            user.ClientId = input.ClientId;
            user.Age = input.Age;
            user.Address = input.Address;
            
            if (!string.IsNullOrEmpty(input.Password))
            {
                user.Password = input.Password; 
            }

            var result = await repository.UpdateAsync(user, cancellationToken);

            return new();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating user: {Message}", e.Message);
            return new() { Message = "Error updating user" };
        }
    }
    
    public async Task<Response<PaginationResult<GetUser>>> GetAllAsync(Paginate paginate, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Getting all users with pagination: {Paginate}", paginate);
            var result = await repository.GetPaginatedListAsync(paginate, cancellationToken:cancellationToken);

            var users = mapper.Map<List<GetUser>>(result.Results);

            return new(new PaginationResult<GetUser>
            {
                ActualPage = result.ActualPage,
                Qyt = result.Qyt,
                PageTotal = result.PageTotal,
                Total = result.Total,
                Results = users
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting all users: {Message}", e.Message);
            return new("Error getting all users");
        }
    }
}