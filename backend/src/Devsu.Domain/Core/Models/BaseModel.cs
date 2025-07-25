namespace Domain.Core.Models;

public abstract class BaseModel
{
    public virtual Guid Id { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual bool IsDeleted { get; set; }
    public virtual bool Status { get; set; } = true;
}