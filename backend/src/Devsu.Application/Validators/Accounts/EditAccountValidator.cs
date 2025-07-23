using Devsu.Application.Dtos.Accounts;

namespace Devsu.Application.Validators.Accounts;

public class EditAccountValidator : AbstractValidator<EditAccount>
{
    public EditAccountValidator()
    {
        RuleFor(x => x.AccountNumber).NotNull()
            .WithMessage("Campo no. cuenta requerido")
            .NotEmpty().WithMessage("Campo no. cuenta requerido");
        
        RuleFor(x => x.AccountType).NotNull()
            .WithMessage("Campo tipo de cuenta es requerido")
            .NotEmpty().WithMessage("Campo tipo de cuenta es requerido")
            .Must(x => x.ToLowerInvariant() == "ahorro" || x.ToLowerInvariant() == "corriente")
            .WithMessage("El campo tipo de cuenta debe ser Ahorro o Corriente");
        
        RuleFor(x => x.InitialBalance).NotNull()
            .WithMessage("Campo balance inicial es requerido")
            .NotEmpty().WithMessage("Campo balance inicial es requerido")
            .GreaterThanOrEqualTo(1).WithMessage("El balance inicial debe ser mayor o igual a 1");
    }
}