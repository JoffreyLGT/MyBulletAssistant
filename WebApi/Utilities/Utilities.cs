using System.Linq;
using System.Security.Claims;

namespace WebApi.Utilities
{
    public static class AuthentificationUtils
    {
        public static bool CompareIdWithTokenId(this ClaimsPrincipal userFromContext, string userId)
        {
            string tokenUserId = userFromContext.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            return (string.IsNullOrWhiteSpace(tokenUserId) || !tokenUserId.Equals(userId));
        }
    }
}
