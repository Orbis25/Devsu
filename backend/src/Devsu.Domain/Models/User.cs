using Domain.Core.Models;

namespace Domain.Models;

public class User : Person
{
    public string? ClientId { get; set; }
    public string? Password { get; set; }
    public ICollection<Account>? Accounts { get; set; }
}