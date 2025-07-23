using Domain.Core.Models;

namespace Devsu.Application.Services.Core;

public abstract class BaseService<TModel, TGet, TRepository> : IBaseService<TGet>
    where TGet : class
    where TModel : BaseModel
    where TRepository : IBaseRepository<TModel>

{
    private readonly TRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public BaseService(TRepository repository, IMapper mapper, ILogger logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }


    public virtual async Task<Response<TGet>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _repository.GetOneAsync(x => x.Id == id, cancellationToken);

            if (result == null)
            {
                _logger.LogWarning("entity with Id {Id} not found", id);
                return new("entity not found") { IsNotFound = true };
            }

            var data = _mapper.Map<TGet>(result);

            return new(data);
        }
        catch (Exception e)
        {
            _logger.LogInformation("Error getting user by Id: {Message}", e.Message);
            return new("Error getting Id");
        }
    }

    public virtual async Task<Response> RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Removing entity with id {Id}", id);
            var result = await _repository.SoftRemoveAsync(id, cancellationToken);

            if (result) return new();
            
            _logger.LogWarning("entity with Id {Id} not found or already removed", id);
            return new() { Message = "entity not found or already removed", IsNotFound = true };

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error removing entity: {Message}", e.Message);
            return new() { Message = "Error removing entity" };
        }
    }

    public async Task<Response<PaginationResult<TGet>>> GetAllAsync(Paginate paginate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all data with pagination: {Paginate}", paginate);
            var result = await _repository.GetPaginatedListAsync(paginate, cancellationToken:cancellationToken);

            var users = _mapper.Map<List<TGet>>(result.Results);

            return new(new PaginationResult<TGet>
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
            _logger.LogError(e, "Error getting all data: {Message}", e.Message);
            return new("Error getting all data");
        }
    }
}