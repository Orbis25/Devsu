namespace Devsu.Application.Dtos.Transactions;

public class GetTransaction
{
    public string? Type { get; set; }
    public Guid Id { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal Amount { get; set; }
    public string? Movement { get; set; }
    public Guid? AccountId { get; set; }
}