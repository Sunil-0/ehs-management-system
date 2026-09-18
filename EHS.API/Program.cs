using System.Text;
using EHS.Application.Interfaces;
using EHS.Application.Services;
using EHS.Infrastructure.Auth;
using EHS.Infrastructure.Data;
using EHS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---- Services registration (Dependency Injection container) ----

// 1. Register the DbContext so it can be injected into any controller/service.
//    "options.UseSqlServer(...)" tells EF Core which database provider + connection string to use.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 1b. Register the repository and service so the DI container can build them
//     whenever a controller asks for IIncidentService.
//     AddScoped = one instance per HTTP request (the right lifetime for anything
//     that touches AppDbContext, since DbContext itself is request-scoped).
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<IIncidentService, IncidentService>();

// 1c. Auth-related services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

// 1d. JWT Bearer authentication -- this tells ASP.NET Core how to validate
//     the "Authorization: Bearer <token>" header on every incoming request:
//     check the signature against our Key, and confirm Issuer/Audience/expiry.
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
    };
});

builder.Services.AddAuthorization();

// 2. Controllers (needed for [ApiController] classes to work)
builder.Services.AddControllers();

// 3. Swagger / OpenAPI (auto-generated API docs + test UI at /swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Defines a security scheme named "Bearer" so Swagger knows this API
    // expects "Authorization: Bearer <token>" -- this is what makes the
    // padlock icon and Authorize button actually appear.
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter ONLY the raw token (Swagger adds 'Bearer ' automatically)."
    });

    // Tells Swagger to actually attach that scheme to every request once
    // you've clicked Authorize -- without this, the button appears but does nothing.
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 4. CORS - allows your Angular app (running on a different port, e.g. localhost:4200)
//    to call this API. Without this, the browser blocks the request.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ---- HTTP request pipeline (middleware, runs in this exact order) ----

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // browse to /swagger to test endpoints manually
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp"); // must come before UseAuthorization

app.UseAuthentication(); // WHO is calling? (validates the JWT, populates User.Claims)
app.UseAuthorization();  // WHAT are they allowed to do? (checks [Authorize(Roles=...)])

app.MapControllers();

app.Run();