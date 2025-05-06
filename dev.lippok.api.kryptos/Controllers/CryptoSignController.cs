using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace dev.lippok.api.kryptos.Controllers;

[ApiController]
[Route("[controller]")]
public class CryptoSignController : ControllerBase
{
    private readonly ILogger<CryptoSignController> _logger;
    private readonly RSA _rsa = RSA.Create(2048);
    
    public CryptoSignController(ILogger<CryptoSignController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Signs the provided input string using RSA.
    /// </summary>
    /// <param name="input">The input string to sign.</param>
    /// <returns>A base64-encoded signature of the input string.</returns>
    [HttpPost("Sign")]
    public IActionResult SignString([FromBody] string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return BadRequest("Input string cannot be null or empty.");
        }
        try
        {
            // Convert the input string to bytes
            var data = System.Text.Encoding.UTF8.GetBytes(input);

            // Sign the data
            var  signature = _rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Convert the signature to a base64 string
            var base64Signature = Convert.ToBase64String(signature);

            return Ok(new { Signature = base64Signature });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing the input string.");
            return StatusCode(500, "Internal server error.");
        }
    }
    
    /// <summary>
    /// Retrieves the RSA public key.
    /// </summary>
    /// <returns>A base64-encoded RSA public key.</returns>
    [HttpGet("PublicKey")]
    public IActionResult GetPublicKey()
    {
        try
        {
            // Export the public key
            var publicKey = _rsa.ExportRSAPublicKey();

            // Convert the public key to a base64 string
            var base64PublicKey = Convert.ToBase64String(publicKey);

            return Ok(new { PublicKey = base64PublicKey });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting the public key.");
            return StatusCode(500, "Internal server error.");
        }
    }
    
    /// <summary>
    /// Verifies the provided signature against the input string.
    /// </summary>
    /// <param name="request">The verification request containing the input string and signature.</param>
    /// <returns>A boolean indicating whether the signature is valid.</returns>

    [HttpPost("Verify")]
    public IActionResult VerifySignature([FromBody] VerifyRequest request)
    {
        if (string.IsNullOrEmpty(request.Input) || string.IsNullOrEmpty(request.Signature))
        {
            return BadRequest("Input string and signature cannot be null or empty.");
        }
        try
        {
            // Convert the input string and signature from base64
            var data = System.Text.Encoding.UTF8.GetBytes(request.Input);
            var signature = Convert.FromBase64String(request.Signature);

            // Verify the signature
            var isValid = _rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            return Ok(new { IsValid = isValid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying the signature.");
            return StatusCode(500, "Internal server error.");
        }
    }
}