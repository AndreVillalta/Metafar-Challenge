using MediatR;

namespace Api.Features.Transactions.Requests;

public record TransactionRequest(string CardNumber, decimal Amount) : IRequest<IResult>;

