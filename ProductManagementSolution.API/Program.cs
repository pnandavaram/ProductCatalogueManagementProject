using Microsoft.EntityFrameworkCore;
using ProductCatalogueManagement.API.Middleware;
using ProductCatalogueManagement.Repository;
using ProductCatalogueManagement.Repository.Interfaces;
using ProductCatalogueManagement.Repository.Repository;
using ProductCatalogueManagement.Services.Interfaces;
using ProductCatalogueManagement.Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>( options =>
    options.UseSqlite("Data Source=products.db"));

builder.Services.AddHttpClient<IInventoryService, InventoryService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5008/"); 
    client.Timeout = TimeSpan.FromSeconds(3);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IInventoryService, InventoryService>();

builder.Services.AddHttpClient<IInventoryService, InventoryService>()
    .ConfigurePrimaryHttpMessageHandler(() => new MockHttpMessageHandler());

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection(); If we're deploying the API onto server with SSL certificate, we can enable this to redirect all HTTP requests to HTTPS.
app.MapControllers();
app.Run();