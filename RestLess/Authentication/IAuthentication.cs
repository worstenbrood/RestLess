using System.Net.Http;

namespace RestLess.Authentication
{
    public interface IAuthentication
    {
        void SetAuthentication(HttpRequestMessage request);
    }
}
