using System.ComponentModel.DataAnnotations;

namespace dev.lippok.api.kryptos;

/// <summary>
/// Represents a request to verify a signature.
/// </summary>
public class VerifyRequest
{
    /// <summary>
    /// The input string to verify.
    /// </summary>
    [Required]
    public string Input { get; set; } = string.Empty;
    
    /// <summary>
    /// The base64-encoded signature to verify.
    /// </summary>
    [Required]
    public string Signature { get; set; } = string.Empty;
}