﻿using Api.Features.Transactions.Requests;
using FluentValidation;

namespace Api.Features.Transactions.Validators;

public class TransactionValidator : AbstractValidator<TransactionRequest>
{
    public TransactionValidator()
    {
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
    }
}