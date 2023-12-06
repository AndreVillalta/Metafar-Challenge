using Api.Common.Interfaces;
using Api.Features.Cards.Requests;
using Api.Features.Cards.Responses;
using Api.Infrastructure;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Cards.Queries;

[Authorize]
public class GetCurrentBalanceQuery : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/currentBalance/{cardNumber}", async (IMediator mediator, string cardNumber) =>
        {
            return await mediator.Send(new UserInfoRequest(cardNumber));
        })
        .WithName(nameof(GetCurrentBalanceQuery))
        .WithTags("Saldo")
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }

    public class GetCurrentBalanceQueryHandler : IRequestHandler<UserInfoRequest, IResult>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IValidator<UserInfoRequest> _validator;
        private readonly ICurrentUserService _currentUserService;

        public GetCurrentBalanceQueryHandler(
            ApplicationDbContext dbContext, 
            IValidator<UserInfoRequest> validator,
            ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _validator = validator;
            _currentUserService = currentUserService;
        }

        public async Task<IResult> Handle(UserInfoRequest request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            
            if (!result.IsValid)
                return Results.BadRequest(result.GetValidationProblems());

            var isCardBelongToUser = await ValidateCardBelongsToUser(request.CardNumber);

            if (!isCardBelongToUser)
                return Results.NotFound($"El número de tarjeta {request.CardNumber} no pertenece al usuario autenticado.");
            
            var userInfoResponse = await _dbContext.Users
                                    .Include(user => user.Cards)
                                    .ThenInclude(card => card.Transactions)
                                    .Where(user => user.Cards.Any(card => card.CardNumber == request.CardNumber && 
                                                                          card.Transactions.Any(transaction => transaction.TransactionType == "Retiro")))
                                    .Select(user => new UserInfoResponse
                                    (
                                        user.UserName,
                                        user.AccountNumber,
                                        user.CurrentBalance,
                                        user.Cards.SelectMany(card => card.Transactions).Max(transaction => transaction.TransactionDate)
                                    ))
                                    .FirstOrDefaultAsync(cancellationToken);

            if (userInfoResponse is null) return Results.NotFound("Sin información");

            return Results.Ok(userInfoResponse);
        }

        private async Task<bool> ValidateCardBelongsToUser(string cardNumber)
            => await _dbContext.Cards
                        .AsNoTracking()
                        .AnyAsync(card => card.CardNumber == cardNumber &&
                                          card.UserId == _currentUserService.User.Id);
    }
}