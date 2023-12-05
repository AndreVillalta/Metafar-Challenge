using MediatR;

namespace Api.Features.Auth.Requests;

public class SignInRequest : IRequest<IResult>
{
    public string CardNumber { get; set; } = string.Empty;
    public int Pin { get; set; }
}