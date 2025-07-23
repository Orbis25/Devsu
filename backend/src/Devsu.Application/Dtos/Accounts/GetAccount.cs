namespace Devsu.Application.Dtos.Accounts;

public class GetAccount
{
    public Guid Id { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountType { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public User? User { get; set; }
    public Guid UserId { get; set; }
}