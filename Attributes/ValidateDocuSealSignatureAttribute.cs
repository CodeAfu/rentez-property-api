using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RentEZApi.Services;

public class ValidateDocuSealSignatureAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var config = context.HttpContext.RequestServices.GetRequiredService<ConfigService>();
        var webhookSecret = config.GetWebhookSecret();

        var signature = context.HttpContext.Request.Headers["X-Docuseal-Signature"].FirstOrDefault();

        if (string.IsNullOrEmpty(webhookSecret) || string.IsNullOrEmpty(signature))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        context.HttpContext.Request.EnableBuffering();
        using var reader = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.HttpContext.Request.Body.Position = 0;

        var keyBytes = Encoding.UTF8.GetBytes(webhookSecret);
        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
        var computedSignature = Convert.ToHexString(hash).ToLower();

        if (signature != computedSignature)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next();
    }
}
