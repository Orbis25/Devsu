using Domain.Core.Models;

namespace Domain.Models;

public class Transaction : BaseModel
{
    public string? AccountNumber { get; set; }
    public string? Type { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal Amount { get; set; }
    public string? Movement { get; set; }
    public bool IsCredit { get; set; }
    public Guid? AccountId { get; set; }
    public Account? Account { get; set; }
}