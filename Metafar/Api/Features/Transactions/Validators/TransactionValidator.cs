using Api.Features.Transactions.Requests;
using FluentValidation;

namespace Api.Features.Transactions.Validators;

public class TransactionValidator : AbstractValidator<TransactionRequest>
{
    //private readonly ApplicationDbContext _dbContext;
    public TransactionValidator(/*ApplicationDbContext dbContext*/)
    {
        //_dbContext = dbContext;

        RuleFor(r => r.CardNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Ingrese un número de tarjeta")
            .Matches("^[0-9]+$")
            .WithMessage("El número de tarjeta no debe contener letras");

        RuleFor(r => r.Amount)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Ingrese un monto a extraer")
            .GreaterThan(0).WithMessage("El monto a extraer debe ser mayor que cero.");
        //    .Must((request, amount) => BeLessThanOrEqualBalance(request.CardNumber, amount))
        //    .WithMessage("El monto a extraer supera el saldo disponible en la tarjeta.");
    }

    //private bool BeLessThanOrEqualBalance(string cardNumber, decimal amount)
    //{
    //    decimal currentBalance = _dbContext.Cards
    //                                .AsNoTracking()
    //                                .Where(card => card.CardNumber == cardNumber)
    //                                .Join(
    //                                    _dbContext.Users,
    //                                    card => card.UserId,
    //                                    user => user.UserId,
    //                                    (card, user) => user.CurrentBalance
    //                                ).First();

    //    return amount <= currentBalance;
    //}
}