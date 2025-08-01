using System.Security.Claims;

namespace BurnoutTracker.Domain.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("User ID claim not found");
            }

            if (!long.TryParse(userIdClaim.Value, out var userId))
            {
                throw new Exception("User ID claim is not a valid long");
            }

            return userId;
        }
    }
}
