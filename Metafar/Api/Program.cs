using Api;
using Api.Entities.AppSettings;
using Carter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistance(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddSecurity(builder.Configuration);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();