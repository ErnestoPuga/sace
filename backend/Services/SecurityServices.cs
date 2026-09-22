using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Sace.Api.Domain;

namespace Sace.Api.Services;

public interface IPasswordService { string Hash(string password); bool Verify(string password, string hash); }
public sealed class PasswordService : IPasswordService {
  private const int Iterations = 120_000;
  public string Hash(string password) { var salt = RandomNumberGenerator.GetBytes(16); var value = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32); return $"pbkdf2-sha256${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(value)}"; }
  public bool Verify(string password, string hash) { try { var p = hash.Split('$'); var expected = Convert.FromBase64String(p[3]); var actual = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromBase64String(p[2]), int.Parse(p[1]), HashAlgorithmName.SHA256, expected.Length); return CryptographicOperations.FixedTimeEquals(actual, expected); } catch { return false; } }
}

public interface ITokenService { string Create(User user); }
public sealed class TokenService(IConfiguration configuration) : ITokenService {
  public string Create(User user) {
    var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
    var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new Claim(JwtRegisteredClaimNames.Email, user.Email), new Claim(ClaimTypes.Name, user.Name), new Claim(ClaimTypes.Role, user.Role) };
    var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}

public static class UserContext {
  public static Guid UserId(this ClaimsPrincipal principal) => Guid.TryParse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
}
