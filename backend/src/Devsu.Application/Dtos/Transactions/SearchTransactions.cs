namespace Devsu.Application.Dtos.Transactions;

public class SearchTransactions : Paginate
{
    [FromQuery]
    public DateTime? From { get; set; }
    [FromQuery]
    public DateTime? To { get; set; }
    
}