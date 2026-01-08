using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RentEZApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Bearer", Policy = "UserOrAdmin")]
public class UploadController : ControllerBase
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly ILogger<UploadController> _logger;

    public UploadController(IConfiguration configuration, ILogger<UploadController> logger)
    {
        _logger = logger;
        _bucketName = configuration["AWS:S3BucketName"] ?? throw new InvalidOperationException("S3 bucket name not configured");

        var accessKey = configuration["AWS:AccessKeyId"];
        var secretKey = configuration["AWS:SecretAccessKey"];
        var sessionToken = configuration["AWS:SessionToken"];

        if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("AWS credentials not configured");
        }

        var credentials = new SessionAWSCredentials(accessKey, secretKey, sessionToken);
        _s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast1);
    }

    [HttpPost("get-upload-url")]
    public IActionResult GetUploadUrl([FromBody] UploadRequest request)
    {
        try
        {
            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var fileExtension = Path.GetExtension(request.FileName).ToLower();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = "Invalid file type. Only images are allowed." });
            }

            // Generate unique filename
            var uniqueFileName = $"properties/{Guid.NewGuid()}{fileExtension}";

            // Create pre-signed URL (valid for 5 minutes)
            var presignedRequest = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = uniqueFileName,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(5),
                ContentType = request.ContentType
            };

            var uploadUrl = _s3Client.GetPreSignedURL(presignedRequest);

            _logger.LogInformation("Generated pre-signed URL for file: {FileName}", uniqueFileName);

            return Ok(new
            {
                uploadUrl = uploadUrl,
                key = uniqueFileName,
                bucket = _bucketName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating pre-signed URL");
            return StatusCode(500, new { message = "Failed to generate upload URL" });
        }
    }

    [HttpPost("confirm-upload")]
    public async Task<IActionResult> ConfirmUpload([FromBody] ConfirmUploadRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            // Verify file exists in S3
            var metadataRequest = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = request.Key
            };

            await _s3Client.GetObjectMetadataAsync(metadataRequest);

            // Generate the full S3 URL
            var imageUrl = $"https://{_bucketName}.s3.amazonaws.com/{request.Key}";

            _logger.LogInformation("Confirmed upload for key: {Key}", request.Key);

            return Ok(new
            {
                imageUrl = imageUrl,
                key = request.Key
            });
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "File not found in S3: {Key}", request.Key);
            return BadRequest(new { message = "File not found in S3" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming upload");
            return StatusCode(500, new { message = "Failed to confirm upload" });
        }
    }
}

public class UploadRequest
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}

public class ConfirmUploadRequest
{
    public string Key { get; set; } = string.Empty;
}
