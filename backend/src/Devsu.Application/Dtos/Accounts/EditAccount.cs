namespace Devsu.Application.Dtos.Accounts;

public class EditAccount
{
    public string? AccountNumber { get; set; }
    public string? AccountType { get; set; }
    public decimal InitialBalance { get; set; }
    public Guid UserId { get; set; }
}