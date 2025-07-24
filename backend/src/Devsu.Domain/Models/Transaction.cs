namespace Domain.Models;

public class Transaction : BaseModel
{
    public string? Type { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal Amount { get; set; }
    
    public string? Movement { get; set; }

    public Guid? AccountId { get; set; }
    public Account? Account { get; set; }
}