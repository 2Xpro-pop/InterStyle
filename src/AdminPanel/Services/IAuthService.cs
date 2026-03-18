using InterStyle.Shared;

namespace AdminPanel.Services;

public interface IAuthService
{
    public Task Login(string login, string password, bool rememverMe);
}
