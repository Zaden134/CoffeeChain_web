namespace CoffeeChainManagement.Infrastructure.Auth;

// ServerSessionMarker thay doi moi lan backend khoi dong de token cu bi tu choi sau restart.
public sealed class ServerSessionMarker
{
    public const string ClaimType = "server_session_id";
    public const string RestartRevocationReason = "server_restart";

    public string SessionId { get; } = Guid.NewGuid().ToString("N");
}