namespace Ugolek.Backend.Web.Application.Services;

public class TokenServiceOptions
{
    public required string ISSUER { get; init; }
    public required string AUDIENCE { get; init; }
    public required string KEY { get; init; }
}