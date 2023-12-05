using MediatR;

namespace Api.Features.Cards.Requests;

public record UserInfoRequest(string CardNumber) : IRequest<IResult>;