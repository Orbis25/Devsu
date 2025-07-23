using System.Text.Json.Serialization;

namespace Devsu.Application.Dtos.Accounts;

public class CreateAccount
{
    public string? AccountNumber { get; set; }
    public string? AccountType { get; set; }
    public decimal InitialBalance { get; set; }
    [JsonIgnore]
    public decimal CurrentBalance { get; set; }
    
    public Guid UserId { get; set; }
}