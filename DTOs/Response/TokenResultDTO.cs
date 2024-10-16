namespace WebAPI.DTOs.Request;

public class TokenResultDTO
{
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }
}
