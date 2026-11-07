using InventorySystem.API.Data;
using InventorySystem.API.Exceptions;
using InventorySystem.API.Models;
using InventorySystem.API.Repositories;
using InventorySystem.API.Services.Product;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Inventory Management API",
        Version = "v1",
        Description = "Enterprise Web API for inventory maangement, stock adjustments, and audit trails."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Generic repository registration for any entity that does not need some specific custom queries
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Specific repository registration for Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IProductService, ProductService>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
