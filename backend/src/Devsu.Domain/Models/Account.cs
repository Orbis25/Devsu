namespace Domain.Models;

public class Account : BaseModel
{
    public decimal DailyDebitLimit { get; set; } = 1000;
    public decimal DailyDebit { get; set; }
    public DateTime LastDailyDebitUpdated { get; set; } = DateTime.UtcNow;
    public string? AccountNumber { get; set; }
    public string? AccountType { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public User? User { get; set; }
    public Guid UserId { get; set; }
    public ICollection<Transaction>? Transactions { get; set; }
}