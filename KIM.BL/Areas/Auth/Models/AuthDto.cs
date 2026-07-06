namespace KIM.BL.Areas.Auth.Models;

public class AuthDto
{
    public string Token { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
