using InventorySystem.API.Data;
using InventorySystem.API.Exceptions;
using InventorySystem.API.Models;
using InventorySystem.API.Repositories;
using InventorySystem.API.Services.Product;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using InventorySystem.API.Services.Auth;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Inventory Management API",
        Version = "v1",
        Description = "Enterprise Web API for inventory maangement, stock adjustments, and audit trails."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Define the bearer security scheme in Swagger 
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the text below. \r\nExample: ehruiehbcvievnoy732vreSDSCNere3435gvrteb..."
    });

    // Make all endpoints require Bearer token if decorated with [Authorize]
    options.AddSecurityRequirement( new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme 
            { 
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }    
    });
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Generic repository registration for any entity that does not need some specific custom queries
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Specific repository registration for Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

// Register the password hasher for secure cryptographic hashing
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Registration for error exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Define CORS policy name
var corsPolicy = "AllowFrontendClients";

// Register CORS in DI container 
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
    {
        // MVC web app front end
        policy.WithOrigins("https://localhost:7000", "http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// JWT Auth Registration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Eliminates the default 5 minute clock drift grace period
    };
});

builder.Services.AddAuthorization();

// Register rate limiter policy

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("ip-fixed", httpContext =>
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0 // Reject immediatly
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// here we enable the exception handler middleware
app.UseExceptionHandler();

// Add cors middleware 
app.UseCors(corsPolicy);

// Enable middleware
app.UseRateLimiter();

app.UseAuthentication(); // 1. Who are you (validates token & creates ClaimsPrincipal)
app.UseAuthorization(); // 2. Are you allowed in? (check Roles/Policies)

app.MapControllers();

app.Run();

// For integration tests
public partial class Program { }