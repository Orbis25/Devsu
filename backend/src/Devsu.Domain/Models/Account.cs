namespace Domain.Models;

public class Account : BaseModel
{
    public string? AccountNumber { get; set; }
    public string? AccountType { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public User? User { get; set; }
    public Guid UserId { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
}