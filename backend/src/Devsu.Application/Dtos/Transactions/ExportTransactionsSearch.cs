namespace Devsu.Application.Dtos.Transactions;

public class ExportTransactionsSearch
{
    [FromQuery]
    public string? Query { get; set; }
    [FromQuery]
    public DateTime? From { get; set; }
    [FromQuery]
    public DateTime? To { get; set; }
}