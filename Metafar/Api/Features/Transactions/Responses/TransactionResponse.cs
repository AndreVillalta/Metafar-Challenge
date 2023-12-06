namespace Api.Features.Transactions.Responses;

public record TransactionResponse
(
    string AccountNumber,
    decimal OldBalance,
    decimal NewBalance,
    decimal Amount,
    string TransactionType
);
