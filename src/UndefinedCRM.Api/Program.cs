using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UndefinedCRM.Application.UseCases.Clients.Create;
using UndefinedCRM.Application.UseCases.Clients.Delete;
using UndefinedCRM.Application.UseCases.Clients.GetAll;
using UndefinedCRM.Application.UseCases.Clients.Update;
using UndefinedCRM.Application.UseCases.Projects.Create;
using UndefinedCRM.Application.UseCases.Projects.Delete;
using UndefinedCRM.Application.UseCases.Projects.GetAll;
using UndefinedCRM.Application.UseCases.Projects.Update;
using UndefinedCRM.Application.UseCases.Users.GetProfile;
using UndefinedCRM.Application.UseCases.Users.Login;
using UndefinedCRM.Application.UseCases.Users.Register;
using UndefinedCRM.Infrastructure;
using UndefinedCRM.Infrastructure.InfrastructureBase;
using UndefinedCRM.Infrastructure.Security.Tokens.Access;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ClientRepository>();
builder.Services.AddScoped<ProjectRepository>();

// User Use Cases
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<LoginUserUseCase>();
builder.Services.AddScoped<GetUserProfileUseCase>();

// Client Use Cases
builder.Services.AddScoped<CreateClientUseCase>();
builder.Services.AddScoped<GetAllClientsUseCase>();
builder.Services.AddScoped<UpdateClientUseCase>();
builder.Services.AddScoped<DeleteClientUseCase>();

// Project Use Cases
builder.Services.AddScoped<CreateProjectUseCase>();
builder.Services.AddScoped<GetAllProjectsUseCase>();
builder.Services.AddScoped<UpdateProjectUseCase>();
builder.Services.AddScoped<DeleteProjectUseCase>();

builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UndefinedDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtKey = builder.Configuration["Jwt:Key"];
if (!string.IsNullOrEmpty(jwtKey))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey))
            };
        });
}
else
{
    Console.WriteLine("WARNING: JWT Key is not configured. Authentication will not work properly.");
}

var corsPolicy = "AllowAllOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors(corsPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
