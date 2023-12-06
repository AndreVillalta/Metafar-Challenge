namespace Api.Features.Transactions.Responses;

public record TransactionHistoryResponse
(
    int Id, 
    DateTime TransactionDate, 
    string TransactionType, 
    decimal Amount, 
    string CardNumber
);
