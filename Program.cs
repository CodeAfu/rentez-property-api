using System.Text.Json;
using System.Text.Json.Serialization;
using RentEZApi.Data;
using RentEZApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var jsonOptions = new JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

builder.Services.AddSingleton(jsonOptions);

builder.Services.AddOpenApi("v1");
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddDbContext<PropertyDbContext>();

builder.Services.AddSingleton<ConfigService>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthorizationService>();
    
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.MapGet("/health", () => new { message = "API is running" });

app.Run();
