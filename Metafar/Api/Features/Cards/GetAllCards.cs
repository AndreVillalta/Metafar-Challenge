using Api.Entities;
using Api.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Cards;

public static class GetAllCards
{
    public sealed class Query : IRequest<List<Card>> { }

    internal sealed class QueryHandler(ApplicationDbContext dbContext) : IRequestHandler<Query, List<Card>>
    {
        public async Task<List<Card>> Handle(Query request, CancellationToken cancellationToken)
            => await dbContext.Cards.ToListAsync(cancellationToken);
    }

    public static void AddEnpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/cards", async (ISender sender) =>
        {
            var cards = await sender.Send(new Query());
            return Results.Ok(cards);
        }).RequireAuthorization();
    }
}
