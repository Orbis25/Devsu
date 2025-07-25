namespace Devsu.Application.Dtos.Accounts;

public class GetAccountTransaction
{
    public Guid Id { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountType { get; set; }
    public decimal InitialBalance { get; set; }
}