namespace Devsu.Application.Validators.Users;

public class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name).NotNull()
            .WithMessage("Campo nombre requerido")
            .NotEmpty().WithMessage("Campo nombre requerido");

        RuleFor(x => x.Age)
            .Must(x => x >= 18).WithMessage("La edad debe ser mayor o igual a 18");
        
        RuleFor(x => x.Identification).NotNull()
            .WithMessage("Campo identificacion es requerido")
            .NotEmpty().WithMessage("Campo Identification requerido");
        
        RuleFor(x => x.Phone).NotNull()
            .WithMessage("Campo telefono es requerido")
            .NotEmpty().WithMessage("Campo telefono requerido");
        
        RuleFor(x => x.ClientId)
            .NotNull()
            .WithMessage("Campo ClientId es requerido")
            .NotEmpty().WithMessage("Campo ClientId requerido");
        
        RuleFor(x => x.Gender)
            .NotNull()
            .WithMessage("Campo Genero es requerido")
            .NotEmpty().WithMessage("Campo Genero requerido")
            .Must(x => x.ToLowerInvariant() == "hombre" || x.ToLowerInvariant() == "mujer")
            .WithMessage("El campo Genero debe ser Hombre o Mujer");

        RuleFor(x => x.Password)
            .NotNull()
            .WithMessage("Campo Contraseña es requerido")
            .NotEmpty().WithMessage("Campo Contraseña requerido")
            .MinimumLength(6).WithMessage("Campo Contraseña debe tener al menos 6 caracteres");
    }
}