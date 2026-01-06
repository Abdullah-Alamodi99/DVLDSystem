using System.Security.Claims;

namespace DVLDSystem.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetCurrentUserId(this ClaimsPrincipal user)
        {
            var claimIdentity = user.Identity as ClaimsIdentity;
            return claimIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
