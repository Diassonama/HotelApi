using Microsoft.AspNetCore.Http;

namespace Hotel.Application.Common
{
    public class IpAddressService
    {
         private readonly IHttpContextAccessor _httpContextAccessor;

    public IpAddressService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetIpAddress()
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress;
        return ipAddress?.ToString();
    }
    }
}