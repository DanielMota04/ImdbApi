using System.Security.Claims;

namespace Api.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
