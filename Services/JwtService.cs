// using Microsoft.Extensions.Configuration;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly IConfiguration _config;
    private readonly string _secret;
    public JwtService(IConfiguration config)
    {
        _config = config;
        _secret = _config["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
    }

    private byte[] DecodeSecret()
    {
        try{
            return Convert.FromBase64String(_secret.Replace('-','+').Replace('_','/'));
        }
        catch(Exception ex)
        {
            throw new InvalidOperationException("❌Invalid base64 secret key", ex);
        }
    }

    public string GenerateToken(string userId,string username)
    {
        if (string.IsNullOrEmpty(_secret) || _secret.Length < 32)
        {
            throw new InvalidOperationException("Decoded JWT secret key must be at least 32 characters long");
        }

        // var jwtSettings = _config.GetSection("JwtSettings");
        // var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
        var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

        var claim = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = new JwtSecurityToken(
                issuer: _config["jwtSettings:Issuer"],
                audience : _config["jwtSettings:Audience"],
                claims: claim,
                signingCredentials: credentials,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["jwtSettings:ExpiryMinutes"]))
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
