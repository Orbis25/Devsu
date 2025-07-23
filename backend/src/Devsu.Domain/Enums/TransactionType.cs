using System.ComponentModel.DataAnnotations;

namespace Domain.Enums;

public enum TransactionType
{
    [Display(Name = "credito")]
    Credit,
    [Display(Name = "debito")]
    Debit,
}