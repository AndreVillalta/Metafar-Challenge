namespace Api.Features.Cards.Responses;

public record UserInfoResponse(
    string UserName,
    string NumberAccount,
    decimal CurrentBalance,
    DateTime LastWithdrawalDate
);