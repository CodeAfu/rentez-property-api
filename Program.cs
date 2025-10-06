using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using RentEZApi.Data;
using RentEZApi.Models.Response;

var builder = WebApplication.CreateBuilder(args);

var jsonOptions = new JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

builder.Services.AddSingleton(jsonOptions);

// builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
    
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

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var jsonOptions = context.RequestServices.GetRequiredService<JsonSerializerOptions>();

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        var error = context.Features.Get<IExceptionHandlerFeature>();
        var response = new ApiResponse<object>
        {
            Error = "An unexpected error occurred."
        };

        // Only include details in development
        if (app.Environment.IsDevelopment() && error?.Error != null)
        {
            response.Message = error.Error.Message;
        }

                
        await context.Response.WriteAsJsonAsync(response, jsonOptions);
    });
});

app.MapGet("/health-check", () => "API is running");

app.Run();


