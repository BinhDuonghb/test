using Amazon.Runtime;
using AspNetCore.Identity.MongoDbCore.Models;
using Login.models.Aplication;
using Login.models.MongoCollection;
using Login.models.Request;
using Login.models.setting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDbIdentity.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Login.Services
{
    public class UserManagerS : IUserManagerS
    {
        private readonly UserManager<AUser> _userManager;
        private readonly JwtSetting _jwtSetting;
        private readonly IMongoCollection<RefreshToken> _refreshToken;
        public UserManagerS(UserManager<AUser> userManager, IOptions<JwtSetting> jwtSettingl,IDatabase setting ,IMongoClient mongoClient)
        {
            _userManager = userManager;
            _jwtSetting = jwtSettingl.Value;
            var database = mongoClient.GetDatabase(setting.DatabaseName);
            _refreshToken = database.GetCollection<RefreshToken>(nameof(RefreshToken));
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return new LoginResponse()
                    {
                        Message = "Invalid Email/password",
                        Success = false
                    };
                }

                return new LoginResponse
                {
                    AccessToken = await GenerateToken(user),
                    RefreshToken =await ReGenerateToken(user),
                    Success = true,
                    Message = "Login Success full",
                    UserId = user?.Id.ToString(),
                    Email = user?.Email
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new LoginResponse()
                {
                    Message = e.Message,
                    Success = false
                };

            }
        }
    

        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user != null)
                {
                    return new RegisterResponse()
                    {
                        Message = "Email has Already register",
                        Success = false

                    };

                }
                user = new AUser
                {
                    Email = request.Email,
                    UserName = request.UserName,
                };
                var created = await _userManager.CreateAsync(user, request.Password);
                if (!created.Succeeded)
                {
                    return new RegisterResponse()
                    {
                        Message = $"Create user failed {created?.Errors?.First()?.Description}",
                        Success = false
                    };
                }

                var roleAdd = await _userManager.AddToRoleAsync(user, "USER");
                if (!roleAdd.Succeeded)
                {
                    return new RegisterResponse()
                    {
                        Message = $"Create user success but role not avaliable {roleAdd?.Errors?.First()?.Description}",
                        Success = false
                    };
                }
                
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var setCaim = await _userManager.AddClaimsAsync(user, claims);
                if (!setCaim.Succeeded)
                {
                    return new RegisterResponse()
                    {
                        Message = $"Create user success but Claims not avaliable {roleAdd?.Errors?.First()?.Description}",
                        Success = false
                    };
                }
                return new RegisterResponse()
                {
                    Message = "Register Successfull",
                    Success = true
                };


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new RegisterResponse()
                {
                    Message = e.Message,
                    Success = false
                };
            }
        }
        public async Task<string> GenerateToken(AUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_jwtSetting.SecurityKey);

            List<Claim> claims = new();
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var role = await _userManager.GetRolesAsync(user);
            var roelClaims = role.Select(x => new Claim(ClaimTypes.Role, x));
            claims.AddRange(roelClaims);


            var key = new SymmetricSecurityKey(tokenKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(30);

            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDesc);

            var existion = await _refreshToken.Find(t => t.UserId == user.Id.ToString()).FirstOrDefaultAsync();
            if (existion == null) {
                await _refreshToken.InsertOneAsync(new RefreshToken { AccessToken = tokenHandler.WriteToken(token), UserId = user.Id.ToString() });
            } 


            return tokenHandler.WriteToken(token);
        }
        public async Task<TokenResponse> RefreshToken(RefreshTokenRequest request)
        {
            var refreshToken = await _refreshToken.Find(item => item.RefreshTo == request.RefreshToken).FirstOrDefaultAsync();
            if (refreshToken != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSetting.SecurityKey);
                var principal = tokenHandler.ValidateToken(request.AccessToken, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey)
                }, out SecurityToken securityToken);
                if (securityToken is JwtSecurityToken token && token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                {
                    string userId = principal.Identity.Name;
                    if (userId == null) return new TokenResponse { Success = false };
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null) return new TokenResponse { Success = false };
                    var _exsitdata = await _refreshToken.Find(t => t.UserId == userId && t.RefreshTo == request.RefreshToken).FirstOrDefaultAsync();
                    if (_exsitdata != null)
                    {
                        var toke = new JwtSecurityToken(
                            claims: principal.Claims.ToArray(),
                            expires: DateTime.Now.AddSeconds(30),
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                            SecurityAlgorithms.HmacSha256)
                            );
                        var finalToken = tokenHandler.WriteToken(toke);
                        return new TokenResponse()
                        {
                            Token = finalToken,
                            Refresh = await ReGenerateToken(user),
                            Success = true
                        };
                    }
                    else
                    {
                        return new TokenResponse { Success = false };
                    }
                }
                else
                {
                    return new TokenResponse { Success = false };
                }
            }
            else
            {
                return new TokenResponse { Success = false };
            }
        }
        private async Task<string> ReGenerateToken(AUser user)
        {
            var randomnumber = new byte[32];
            using var randomnumbergenerate = RandomNumberGenerator.Create();

            randomnumbergenerate.GetBytes(randomnumber);
            string refreshtoken = Convert.ToBase64String(randomnumber);
            var existion = _refreshToken.Find(t => t.UserId == user.Id.ToString()).FirstOrDefaultAsync();
            if (existion != null)
            {
                var update = Builders<RefreshToken>.Update.Set(t => t.RefreshTo, refreshtoken);
                await _refreshToken.UpdateOneAsync(t => t.UserId == user.Id.ToString(), update);

            }
            else
            {
                await _refreshToken.InsertOneAsync(new RefreshToken
                {
                    UserId = user.Id.ToString(),
                    AccessToken = await GenerateToken(user),
                    RefreshTo = refreshtoken
                });
            }
            return refreshtoken;
        }

    }
}
