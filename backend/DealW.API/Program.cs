using DealW.Application.Interfaces.Auth;
using DealW.Application.Services;
using DealW.Domain.Abstractions;
using DealW.Extensions;
using DealW.Infrastructure;
using DealW.Infrastructure.Authentication;
using DealW.Persistence;
using DealW.Persistence.Mappings;
using DealW.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.

services.AddApiAuthentication(configuration);

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
services.Configure<AuthorizationOptions>(configuration.GetSection(nameof(AuthorizationOptions)));

services.AddDbContext<DealWDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(DealWDbContext)));
    });

services.AddScoped<IDuelsService, DuelsService>();
services.AddScoped<IDuelsRepository, DuelsRepository>();

services.AddScoped<IAnswerService, AnswersService>();
services.AddScoped<IAnswersRepository, AnswersRepository>();

services.AddScoped<IQuestionsService, QuestionsService>();
services.AddScoped<IQuestionsRepository, QuestionsRepository>();

services.AddScoped<IUsersRepository, UsersRepository>();
services.AddScoped<UsersService>();

services.AddScoped<IJwtProvider, JwtProvider>();
services.AddScoped<IPasswordHasher, PasswordHasher>();

services.AddAutoMapper(typeof(DataBaseMappings).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x =>
{
    x.WithHeaders().AllowAnyHeader();
    x.WithOrigins("http://localhost:3000");
    x.WithMethods().AllowAnyMethod();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
