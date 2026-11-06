using InventorySystem.API.Data;
using InventorySystem.API.Exceptions;
using InventorySystem.API.Models;
using InventorySystem.API.Repositories;
using InventorySystem.API.Services.Product;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Generic repository registration for any entity that does not need some specific custom queries
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Specific repository registration for Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IProductService, ProductService>();

// Registration for error exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
