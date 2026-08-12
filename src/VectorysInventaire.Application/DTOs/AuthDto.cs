namespace VectorysInventaire.Application.DTOs;

public class LoginRequestDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? CodePersonnel { get; set; }
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public int UserId { get; set; }
}
