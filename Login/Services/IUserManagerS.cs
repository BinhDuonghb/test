using Login.models.Aplication;
using Login.models.Reponse;
using Login.models.Request;

namespace Login.Services
{
    public interface IUserManagerS
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<RegisterResponse> Register(RegisterRequest request);
        Task<string> GenerateToken(AUser user);
        Task<TokenResponse> RefreshToken(RefreshTokenRequest request);
        Task<UserResponse> GetCurrentUser(string accessToken);

        DecodeTokenResponse ValidateToken(string accessToken);
    }
}
