using Devsu.Application.Dtos.Transactions;
using Devsu.Application.Extensions;
using Domain.Enums;

namespace Devsu.Application.Validators.Transactions;

public class EditTransactionValidator: AbstractValidator<EditTransaction>
{
    public EditTransactionValidator()
    {
        RuleFor(x => x.Amount).NotNull()
            .WithMessage("Campo monto requerido");

        RuleFor(x => x.Type).NotNull()
            .WithMessage("Campo tipo de movimiento es requerido")
            .NotEmpty().WithMessage("Campo tipo de movimiento es requerido")
            .Must(x => x.ToLowerInvariant() == TransactionType.Credit.GetDisplay() ||
                       x.ToLowerInvariant() == TransactionType.Debit.GetDisplay())
            .WithMessage("El campo tipo de transaccion debe ser Credito o Debito");

    }
}