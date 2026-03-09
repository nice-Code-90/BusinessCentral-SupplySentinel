using SupplySentinel.Application;
using SupplySentinel.Infrastructure;
using SupplySentinel.Infrastructure.REST;
using SupplySentinel.Application.Common.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddRestInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("SupplySentinel API")
               .WithTheme(ScalarTheme.Mars)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });

    
    app.MapGet("/api/test/bc/item/{sku}", async (IERPComparisonTool tool, string sku) =>
    {
        var result = await tool.GetItemBySkuAsync(sku);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Json(result.Error, statusCode: 400);
    })
    .WithName("GetBcItemTest"); 
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();