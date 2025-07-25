using Devsu.Application.Dtos.Accounts;

namespace Devsu.Application.Dtos.Transactions;

public class ExportTransactionSearchResponse : GetTransaction
{
    public DateTime? CreatedAt { get; set; }
    public string? ClientName { get; set; }
    public GetAccountTransaction? Account { get; set; }
    public bool Status { get; set; }

}