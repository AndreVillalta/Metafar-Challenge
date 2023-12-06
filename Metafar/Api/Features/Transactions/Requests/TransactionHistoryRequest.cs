using MediatR;

namespace Api.Features.Transactions.Requests;

public record TransactionHistoryRequest(string CardNumber, string? SortColumn, string? SortOrder, int Page, int PageSize) 
    : IRequest<IResult>;