using InventorySystem.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");

// Register the ProductApi 
builder.Services.AddHttpClient<IProductApiClientService, ProductApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddStandardResilienceHandler(options =>
{
    // 1. Configure Retry Strategy
    options.Retry.MaxRetryAttempts = 3; // Max 3 retries of a request
    options.Retry.BackoffType = Polly.DelayBackoffType.Exponential; // 2s, 4s, 8s...
    options.Retry.UseJitter = true; // Ads random miliseconds to prevent "thundering herd"

    // 2. Configure Individual Attempt timeout
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5); // each attempt can take up to 5 seconds

    // 3. Configure total request timeout
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
