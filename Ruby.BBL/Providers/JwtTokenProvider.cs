using BBL.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Ruby.DAL.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace BBL.Providers
{
    public sealed class JwtTokenProvider : IJwtTokenProvider
    {
        public readonly JwtOptions _jwtOptions;

        public JwtTokenProvider(IOptions<JwtOptions> jwtOptions) {

            _jwtOptions = jwtOptions.Value;

        }

        public string GenerateJwtToken(UserEntity user) {

            var claims = new List<Claim> {

            new Claim (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim (ClaimTypes.Name, user.Username ),
            new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

        };

            if(!string.IsNullOrEmpty(user.Email))
            {
                claims.Add( new Claim (ClaimTypes.Email, user.Email));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); 

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes), 
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(tokenDescriptor);
        }


        



    }
}
