using System.IdentityModel.Tokens.Jwt;

namespace SupportApp.Service
{
    public class FromTokenData
    {

        public async Task<string> tokenDataRetrieve(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Token cannot be null or empty", nameof(token));
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                throw new ArgumentException("Invalid token format", nameof(token));
            }

            var claims = jsonToken.Claims;
            var empCode = claims.FirstOrDefault(claim => claim.Type == "EmpCode")?.Value;

            if (string.IsNullOrEmpty(empCode))
            {
                throw new Exception("EmpCode claim not found in token");
            }

            return await Task.FromResult(empCode);
        }
    }
}
