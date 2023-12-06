using Api.Features.Transactions.Requests;
using FluentValidation;

namespace Api.Features.Transactions.Validators;

public class TransactionHistoryValidator : AbstractValidator<TransactionHistoryRequest>
{
    public TransactionHistoryValidator()
    {
        RuleFor(r => r.CardNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Ingrese un número de tarjeta")
            .Matches("^[0-9]+$")
            .WithMessage("El número de tarjeta no debe contener letras");
    }
}
