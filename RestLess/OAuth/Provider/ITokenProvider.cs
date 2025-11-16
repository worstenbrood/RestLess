using RestLess.OAuth.Models;

namespace RestLess.OAuth.Provider
{
    public interface ITokenProvider
    {
        TokenResponse GetToken();
    }
}
