using Login.models.Aplication;
using Login.models.Request;
using MongoDbIdentity.Dtos;

namespace Login.Services
{
    public interface IUserManagerS
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<RegisterResponse> Register(RegisterRequest request);
        Task<string> GenerateToken(AUser user);
        Task<TokenResponse> RefreshToken(RefreshTokenRequest request);
    }
}
