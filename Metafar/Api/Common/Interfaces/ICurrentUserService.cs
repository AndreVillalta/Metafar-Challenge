namespace Api.Common.Interfaces;

public interface ICurrentUserService
{
    CurrentUser User { get; }
}

public record CurrentUser(int Id, string AccountNumber, string UserName, bool IsAuthenticated);