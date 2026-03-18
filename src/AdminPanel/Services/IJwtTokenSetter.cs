namespace AdminPanel.Services;

public interface IJwtTokenSetter
{
    public Task SetTokenAsync(string token, bool remember);
}
