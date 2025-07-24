namespace Devsu.Application.Dtos.Transactions;

public class SearchTransactions : Paginate
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    
}