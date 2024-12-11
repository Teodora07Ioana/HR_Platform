using Microsoft.AspNetCore.SignalR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        // Returnează email-ul utilizatorului autentificat
        return connection.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
    }
}
