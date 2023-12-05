using Api.Features.Auth.Requests;
using FluentValidation;

namespace Api.Features.Auth.Validators;

public class SignInValidator : AbstractValidator<SignInRequest>
{
    public SignInValidator()
    {
        RuleFor(r => r.CardNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Ingrese un número de tarjeta")
            .Matches("^[0-9]+$")
            .WithMessage("El número de tarjeta no debe contener letras");

        RuleFor(r => r.Pin)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Ingrese el número de PIN")
            .InclusiveBetween(1000, 9999)
            .WithMessage("El PIN debe estar entre 1000 y 9999");
    }
}
