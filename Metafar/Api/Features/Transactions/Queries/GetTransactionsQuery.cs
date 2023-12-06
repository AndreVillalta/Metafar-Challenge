using Api.Entities;
using Api.Entities.PagedList;
using Api.Features.Transactions.Requests;
using Api.Features.Transactions.Responses;
using Api.Infrastructure;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Api.Features.Transactions.Queries;

[Authorize]
public class GetTransactionsQuery : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        //app.MapGet("api/transaction/history?cardNumber={cardNumber}&page={}&pageSize={}", 
        app.MapGet("api/transaction/history", 
            async (string cardNumber, string? sortColumn, string? sortOrder, int page, int pageSize, IMediator mediator) =>
        {
            return await mediator.Send(new TransactionHistoryRequest(cardNumber, sortColumn, sortOrder, page, pageSize));
        })
        .WithName(nameof(GetTransactionsQuery))
        .WithTags("Operaciones")
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }

    public class GetTransactionsQueryHandler : IRequestHandler<TransactionHistoryRequest, IResult>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IValidator<TransactionHistoryRequest> _validator;

        public GetTransactionsQueryHandler(
            ApplicationDbContext dbContext,
            IValidator<TransactionHistoryRequest> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<IResult> Handle(TransactionHistoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);

            if (!result.IsValid)
                return Results.BadRequest(result.GetValidationProblems());

            IQueryable<Transaction> transactionsQuery = _dbContext.Transactions
                                                            .Join(_dbContext.Cards,
                                                                 transaction => transaction.CardId,
                                                                 card => card.CardId,
                                                                 (transaction, card) => new { transaction, card })
                                                            .Where(x => x.card.CardNumber == request.CardNumber)
                                                            .AsNoTracking()
                                                            .Select(x => x.transaction);

            var sortPropery = GetSortProperty(request);

            transactionsQuery = request.SortOrder?.ToLower() == "desc" ?
                    transactionsQuery.OrderByDescending(sortPropery) :
                    transactionsQuery.OrderBy(sortPropery);

            var transactions = await PagedList<Transaction>.CreateAsync(
                transactionsQuery,
                request.Page,
                request.PageSize);

            return Results.Ok(transactions);
        }

        private static Expression<Func<Transaction, object>> GetSortProperty(TransactionHistoryRequest request)
            => request.SortColumn?.ToLower() switch
               {
                   "date" => transaction => transaction.TransactionDate,
                   "amount" => transaction => transaction.Amount,
                   "type" => transaction => transaction.TransactionType,
                   _ => transaction => transaction.TransactionId,
               };  
    }
}
