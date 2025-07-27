using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BurnoutTracker.Domain.Extensions
{
    public static class HttpRequestExtensions
    {
        public static long? GetUserIdFromJwtToken(this HttpRequest request)
        {
            if (!request.Headers.TryGetValue("Authorization", out var authHeaders))
                return null;

            var bearerToken = authHeaders.FirstOrDefault();
            if (string.IsNullOrEmpty(bearerToken) || !bearerToken.StartsWith("Bearer "))
                return null;

            var token = bearerToken.Substring("Bearer ".Length).Trim();

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return null;

            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return null;

            if (long.TryParse(userIdClaim.Value, out var userId))
                return userId;

            return null;
        }
    }
}
