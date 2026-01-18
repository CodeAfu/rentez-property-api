using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

using RentEZApi.Data;
using RentEZApi.Models.Entities;

public class LambdaSnsAlertService
{
    private readonly HttpClient _httpClient;
    private readonly PropertyDbContext _dbContext;
    private readonly ILogger<LambdaSnsAlertService> _logger;

    public LambdaSnsAlertService(
        IHttpClientFactory httpClient,
        PropertyDbContext dbContext,
        ILogger<LambdaSnsAlertService> logger)
    {
        _httpClient = httpClient.CreateClient();
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SubscribeToAlerts(string email)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == email.ToLower());
        if (user == null)
            _logger.LogError("User not found for email {Email}", email);

        _logger.LogInformation("Initiating subscription for {Email}", email);
        var payload = new { email };
        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var subscribeLambdaUrl = "https://gnlejh6gti.execute-api.us-east-1.amazonaws.com/default/sns-subscribe";
        var response = await _httpClient.PostAsync(subscribeLambdaUrl, content);
        response.EnsureSuccessStatusCode();

        _logger.LogInformation("Subscription initiated for {Email}", email);

        if (user != null)
        {
            user.IsSubscribedToNotifications = true;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task UnsubscribeFromAlerts(string email)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == email.ToLower());
        if (user == null)
            _logger.LogError("User not found for email {Email}", email);

        _logger.LogInformation("Initiating unsubscription for {Email}", email);
        try
        {
            var payload = new { email };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var unsubscribeLambdaUrl = "https://h844al2fq0.execute-api.us-east-1.amazonaws.com/default/sns-unsubscribe";
            var response = await _httpClient.PostAsync(unsubscribeLambdaUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Lambda unsubscribe warning: {Code}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call unsubscribe Lambda");
        }

        if (user != null)
        {
            user.IsSubscribedToNotifications = false;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task NotifyNewProperty(Property property)
    {
        var payload = new
        {
            property = new
            {
                id = property.Id,
                title = property.Title,
                address = property.Address,
                rent = property.Rent,
            }
        };

        _logger.LogInformation("Notifying subscribers about property {PropertyId}", property.Id);
        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var notifyLambdaUrl = "https://c5wlm9u1wj.execute-api.us-east-1.amazonaws.com/default/sns-notify";
        var response = await _httpClient.PostAsync(notifyLambdaUrl, content);
        response.EnsureSuccessStatusCode();

        _logger.LogInformation("Notified subscribers about property {PropertyId}", property.Id);
    }
}
