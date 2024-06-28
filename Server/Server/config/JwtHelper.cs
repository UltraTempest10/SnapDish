using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.config
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateToken(int User_ID, int User_Permission = 1)
        {
            // 1. 定义需要使用到的Claims
            var claims = new List<Claim>
            {
                new Claim("User_ID", User_ID.ToString())
            };

            if (User_Permission == 1)
            {
                claims.Add(new Claim(ClaimTypes.Role, "player"));
            }
            else if (User_Permission == 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
            }

            // 2. 从 appsettings.json 中读取SecretKey
            var secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
            Console.WriteLine(_configuration["Authentication:Issuer"]);
            Console.WriteLine(_configuration["Authentication:Audience"]);
            var secretKey = new SymmetricSecurityKey(secretByte);
            //var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;
            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);
            // 5. 根据以上，生成token
            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],     //Issuer
                _configuration["Authentication:Audience"],   //Audience
                claims,                          //Claims,
                DateTime.UtcNow,                    //notBefore
                DateTime.UtcNow.AddDays(1),         //expires
                signingCredentials               //Credentials
            );
            // 6. 将token变为string
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }

        //根据token获得用户id
        public int GetUserIdFromToken(string token)
        {
            if (!string.IsNullOrEmpty(token) && token.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase))
            {
                // 去掉 'bearer ' 前缀
                token = token.Substring("bearer ".Length);
            }


            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Authentication:Issuer"],
                ValidAudience = _configuration["Authentication:Audience"],
                IssuerSigningKey = secretKey
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var userIdClaim = claimsPrincipal.FindFirst("User_ID");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常信息: {ex.Message}");
            }

            return -1; // 解析失败时返回-1
        }
    }
}
