using BackOffice.Application.DTOs;
using BackOffice.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BackOffice.Infrastructure.Security {
    public class JwtTokenGenerator : ITokenGenerator {
        private readonly JwtOptions jwtOptions;

        public JwtTokenGenerator(IOptions<JwtOptions> options) {
            jwtOptions = options.Value;
        }

        public TokenResult Generate(LoginResult loginResult) {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset expiresAt = now.AddMinutes(
                jwtOptions.ExpirationMinutes
            );

            Claim[] claims = {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    loginResult.Id.ToString()
                ),
                new Claim(ClaimTypes.Name, loginResult.Name),
                new Claim(ClaimTypes.Email, loginResult.Email),
                new Claim(ClaimTypes.Role, loginResult.Role.ToString()),
                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()
                )
            };

            byte[] keyBytes = Convert.FromBase64String(
                jwtOptions.SigningKey
            );

            var securityKey = new SymmetricSecurityKey(keyBytes);
            var signingCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );
            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: expiresAt.UtcDateTime,
                signingCredentials: signingCredentials
            );

            return new TokenResult {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt
            };
        }
    }
}