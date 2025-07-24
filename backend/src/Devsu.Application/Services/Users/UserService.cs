namespace Devsu.Application.Services.Users;

public class UserService: BaseService<User,GetUser,IUserRepository>, IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    public UserService(IUserRepository repository, IMapper mapper, ILogger<UserService> logger) : base(repository, mapper, logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Response<Guid>> CreateAsync(CreateUser input, CancellationToken cancellationToken)
    {
        try
        {
            var exist = await _repository.ExistAsync(x => x.Identification == input.Identification
                                                         || x.ClientId == input.ClientId, cancellationToken);

            if (exist)
            {
                _logger.LogWarning("User with identification {Identification} already exists", input.Identification);

                return new($"Cliente con identificación o clientId ya existe");
            }


            _logger.LogInformation("Creating user with input: {Identification}", input.Identification);

            var result = await _repository.CreateAsync(_mapper.Map<User>(input), cancellationToken);

            return new(result.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating user: {Message}", e.Message);

            return new("Error creating user");
        }
    }
   
    public async Task<Response> UpdateAsync(Guid id, EditUser input, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with id {Id}", id);
            var user = await _repository.GetOneAsync(x => x.Id == id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User with Id {Id} not found", id);
                return new() { Message = "User not found", IsNotFound = true };
            }

            if (user.ClientId != input.ClientId)
            {
                var exist = await _repository.ExistAsync(x => x.ClientId == input.ClientId && x.Id != id, cancellationToken);
                
                if (exist)
                {
                    _logger.LogWarning("User with ClientId {ClientId} already exists", input.ClientId);
                    return new() { Message = "ClientId ya existe" };
                }
            }

            if (user.Identification != input.Identification)
            {
                var exist = await _repository.ExistAsync(x => x.Identification == input.Identification && x.Id != id, cancellationToken);
                
                if (exist)
                {
                    _logger.LogWarning("User with Identification {Identification} already exists", input.Identification);
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

            var _ = await _repository.UpdateAsync(user, cancellationToken);

            return new();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating user: {Message}", e.Message);
            return new() { Message = "Error updating user" };
        }
    }
    
}