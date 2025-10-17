using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using RentEZApi.Data;
using RentEZApi.Models.Response;
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
builder.Services.AddScoped<UserService>();
    
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Title = "RentEZ API",
//         Version = "v1",
//         Description = "RentEZ API Documentation"
//     });
// });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    // app.UseSwagger();
    // app.UseSwaggerUI(c =>
    // {
        // c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    // });
}

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

app.MapGet("/health", () => "API is running");

app.Run();
