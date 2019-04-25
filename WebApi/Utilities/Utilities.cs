using System.Linq;
using System.Security.Claims;

namespace WebApi.Utilities
{
    public static class AuthentificationUtils
    {
        /// <summary>
        /// Compare the user id included in the token with the userId provided.
        /// </summary>
        /// <param name="userFromContext"></param>
        /// <param name="userId"></param>
        /// <returns>true if they are the same</returns>
        public static bool CompareIdWithTokenId(this ClaimsPrincipal userFromContext, string userId)
        {
            string tokenUserId = userFromContext.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (string.IsNullOrWhiteSpace(tokenUserId))
            {
                return false;
            }
            return tokenUserId.Equals(userId);
        }
    }
}
