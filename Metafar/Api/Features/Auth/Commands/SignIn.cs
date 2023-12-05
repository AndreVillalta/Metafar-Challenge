using Api.Entities.AppSettings;
using Api.Features.Auth.Requests;
using Api.Features.Auth.Responses;
using Api.Infrastructure;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Features.Auth.Commands;

[Authorize]
public class SignIn : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth", async (HttpRequest req, IMediator mediator, [FromBody] SignInRequest request) =>
        {
            return await mediator.Send(request);
        })
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status401Unauthorized)
        .AllowAnonymous();
    }

    public class SignInHandler : IRequestHandler<SignInRequest, IResult>
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ApplicationDbContext _dbContext;
        private readonly IValidator<SignInRequest> _validator;

        public SignInHandler(ApplicationDbContext dbContext, 
                            IOptionsMonitor<JwtSettings> jwtSettings,
                            IValidator<SignInRequest> validator)
        {
            _dbContext = dbContext;
            _jwtSettings = jwtSettings.CurrentValue;
            _validator = validator;
        }

        public async Task<IResult> Handle(SignInRequest request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return Results.BadRequest(result.GetValidationProblems());
            
            // Verificamos si existe el número de tarjeta
            var card = await _dbContext.Cards.FirstOrDefaultAsync(card => card.CardNumber == request.CardNumber, cancellationToken);

            if (card is null) 
                return Results.NotFound($"El número de tarjeta {request.CardNumber} no existe en nuestra base de datos.");

            //https://stackoverflow.com/questions/32752578/whats-the-appropriate-http-status-code-to-return-if-a-user-tries-logging-in-wit
            if (card.IsBlocked) 
                return Results.Unauthorized();

            if (card.Pin != request.Pin)
            {
                card.UpdateFailedAttempts(card);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Results.NotFound("PIN asociado inválido");
            }

            var userCard = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserId == card.UserId, cancellationToken);

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Name, userCard!.UserName),
                new (JwtRegisteredClaimNames.Sid, userCard!.AccountNumber)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer!,
                audience: _jwtSettings.Audience!,
                claims: claims,
                expires: DateTime.Now.AddMinutes(720),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return Results.Ok(new SignInResponse
            {
                JwtToken = jwt
            });
        }
    }  
}