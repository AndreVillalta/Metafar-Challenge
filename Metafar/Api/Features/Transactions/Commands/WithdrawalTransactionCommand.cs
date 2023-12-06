using Api.Entities;
using Api.Features.Transactions.Requests;
using Api.Features.Transactions.Responses;
using Api.Infrastructure;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Transactions.Commands;

public class WithdrawalTransactionCommand : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/transaction/withdrawal", async (HttpRequest req, IMediator mediator, [FromBody] TransactionRequest request) =>
        {
            return await mediator.Send(request);
        })
        .WithName(nameof(TransactionHandler))
        .WithTags("Operaciones")
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }

    public class TransactionHandler : IRequestHandler<TransactionRequest, IResult>
    {
        private readonly IValidator<TransactionRequest> _validator;
        private readonly ApplicationDbContext _dbContext;

        public TransactionHandler(IValidator<TransactionRequest> validator, ApplicationDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<IResult> Handle(TransactionRequest request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return Results.BadRequest(result.GetValidationProblems());

            var beLessThanOrEqualBalance = await ValidateCurrentBalance(request.CardNumber, request.Amount);

            if (!beLessThanOrEqualBalance)
                return Results.BadRequest("El monto a extraer supera el saldo disponible en la tarjeta.");

            var card = await GetCard(request.CardNumber);

            if (card is null) return Results.BadRequest("Número de tarjeta inexistente");

            var response = await GenerateTransaction(card, request.Amount, cancellationToken);

            return Results.Ok(response);
        }

        private async Task<bool> ValidateCurrentBalance(string cardNumber, decimal amount)
        {
            decimal currentBalance = await _dbContext.Cards
                                            .AsNoTracking()
                                            .Where(card => card.CardNumber == cardNumber)
                                            .Join(_dbContext.Users,
                                                  card => card.UserId,
                                                  user => user.UserId,
                                                  (card, user) => user.CurrentBalance)
                                            .FirstAsync();

            return amount <= currentBalance;
        }

        private async Task<Card?> GetCard(string cardNumber)
            => await _dbContext.Cards
                    .Include(card => card.User)
                    .Where(card => card.CardNumber == cardNumber)
                    .FirstOrDefaultAsync();

        private async Task<TransactionResponse> GenerateTransaction(Card card, decimal amount, CancellationToken cancellationToken)
        {
            var oldBalance = card.User.CurrentBalance;

            card.User.UpdateCurrentBalance(card.User, amount);

            var transaction = new Transaction
            {
                TransactionDate = DateTime.UtcNow,
                TransactionType = "Retiro",
                Amount = amount,
                CardId = card.CardId
            };

            _dbContext.Transactions.Add(transaction);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new TransactionResponse(card.User.AccountNumber, oldBalance, card.User.CurrentBalance, amount, transaction.TransactionType);
        }
    }
}