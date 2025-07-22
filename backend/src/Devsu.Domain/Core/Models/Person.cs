namespace Domain.Core.Models;

public abstract class Person : BaseModel
{
    public virtual string? Name { get; set; }
    public virtual string? Gender { get; set; }
    public virtual int Age { get; set; }
    public virtual string? Identification { get; set; }
    public virtual string? Phone { get; set; }
    public virtual string? Address { get; set; }
}