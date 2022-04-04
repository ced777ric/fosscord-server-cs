using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Fosscord.API.PostData;

public class RegisterData
{
    [JsonPropertyName("email")]
    [Required]
    public string Email { get; set; }

    [JsonPropertyName("username")]
    [Required]
    public string Username { get; set; }

    [JsonPropertyName("password")]
    [Required]
    public string Password { get; set; }

    [JsonPropertyName("invite")]
    public string? Invite { get; set; }

    [JsonPropertyName("consent")]
    public bool Consent { get; set; }

    [JsonPropertyName("date_of_birth")]
    public string? DateOfBirth { get; set; }

    [JsonPropertyName("gift_code_sku_id")]
    public string? GiftCodeSkuId { get; set; }

    [JsonPropertyName("captcha_key")]
    public string? CaptchaKey { get; set; }
}

public class LoginData
{
    [JsonPropertyName("login")]
    [Required]
    public string Login { get; set; }

    [JsonPropertyName("password")]
    [Required]
    public string Password { get; set; }

    [JsonPropertyName("undelete")]
    public bool Undelete { get; set; }

    [JsonPropertyName("captcha_key")]
    public string? CaptchaKey { get; set; }

    [JsonPropertyName("login_source")] 
    public string? LoginSource { get; set; }

    [JsonPropertyName("gift_code_sku_id")]
    public string? GiftCodeSkuId { get; set; }
}