namespace Devsu.Application.Dtos.Transactions;

public class EditTransaction
{
    public required string Type { get; set; }
    public decimal Amount { get; set; }
    public Guid AccountId { get; set; }
}