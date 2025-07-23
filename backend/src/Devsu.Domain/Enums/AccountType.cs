using System.ComponentModel.DataAnnotations;

namespace Domain.Enums;

public enum AccountType
{
    [Display(Name = "ahorros")]
    Savings,
    [Display(Name = "corriente")]
    Checking,
}