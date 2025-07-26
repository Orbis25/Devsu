using Devsu.Application.Dtos.Accounts;

namespace Devsu.Application.Dtos.Transactions;

public class GetTransaction
{
    public string? Type { get; set; }
    public Guid Id { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal Amount { get; set; }
    public string? Movement { get; set; }
    public Guid? AccountId { get; set; }
    public virtual GetAccount? Account { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedAtStr => 
        CreatedAt.ToString("dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

    public bool Status { get; set; }
}