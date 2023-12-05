using Api.Features.Cards.Requests;
using FluentValidation;

namespace Api.Features.Cards.Validators;

public class UserInfoValidator : AbstractValidator<UserInfoRequest>
{
    public UserInfoValidator()
    {
        RuleFor(r => r.CardNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Ingrese un número de tarjeta")
            .Matches("^[0-9]+$")
            .WithMessage("El número de tarjeta no debe contener letras");
    }
}