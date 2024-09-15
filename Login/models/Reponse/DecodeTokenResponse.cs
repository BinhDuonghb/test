using System.Security.Claims;

namespace Login.models.Reponse
{
    public class DecodeTokenResponse
    {
        public ClaimsPrincipal? ClaimsPrincipal { get; set; }
        public string Message { get; set; } = string.Empty;

        public bool Success { get; set; }
    }
}
