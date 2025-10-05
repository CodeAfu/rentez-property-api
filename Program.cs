using Microsoft.OpenApi.Models;
using RentEZApi.Data;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RentEZ API",
        Version = "v1",
        Description = "RentEZ API Documentation"
    });
});

builder.Services.AddDbContext<PropertyDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
Console.WriteLine($"ENV_VAR={env}");

var environment = app.Environment.EnvironmentName;
Console.WriteLine($"===============================");
Console.WriteLine($"Environment: {environment}");
Console.WriteLine($"===============================");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health-check", () => "API is running");

app.Run();


