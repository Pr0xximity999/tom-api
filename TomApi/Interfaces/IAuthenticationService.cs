namespace TomApi.Interfaces;

public interface IAuthenticationService
{
    /// <summary>
    /// Returns the name of the authenticated user
    /// </summary>
    /// <returns></returns>
    string? GetCurrentUserId();
}