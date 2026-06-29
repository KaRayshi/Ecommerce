using System.ComponentModel.DataAnnotations;

public class ResendOtpDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}