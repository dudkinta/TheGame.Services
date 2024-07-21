namespace InnerTokenService.Interfaces
{
    public interface ITokenService
    {
        string GenerateServiceToken(TimeSpan expiration);
    }
}
