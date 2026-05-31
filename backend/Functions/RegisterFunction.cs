using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AuctionBackend.Functions
{
    public class RegisterFunction
    {
        private readonly ILogger<RegisterFunction> _logger;
        private const string JwtSecret = "your-super-secret-key-that-should-be-in-keyvault-min-32-chars-long!";
        private const string JwtIssuer = "auction-app";
        private const string JwtAudience = "auction-users";

        public RegisterFunction(ILogger<RegisterFunction> logger)
        {
            _logger = logger;
        }

        [Function("Register")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "register")] HttpRequestData req)
        {
            _logger.LogInformation("Register function invoked");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                string username = data?.username;
                string email = data?.email;
                string password = data?.password;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    badResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                    badResponse.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                    badResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                    await badResponse.WriteAsJsonAsync(new { message = "All fields are required" });
                    return badResponse;
                }

                if (password.Length < 6)
                {
                    var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    badResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                    badResponse.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                    badResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                    await badResponse.WriteAsJsonAsync(new { message = "Password must be at least 6 characters" });
                    return badResponse;
                }

                // In real app: Check if user exists in database, hash password, save to database
                // For demo: Create new user with bidder role
                
                var token = GenerateJwtToken(username, email, "bidder");
                var userId = Guid.NewGuid().ToString();

                var response = req.CreateResponse(System.Net.HttpStatusCode.Created);
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                await response.WriteAsJsonAsync(new
                {
                    token = token,
                    user = new
                    {
                        id = userId,
                        username = username,
                        email = email,
                        role = "bidder"
                    }
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Register error: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                errorResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                errorResponse.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                errorResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                await errorResponse.WriteAsJsonAsync(new { message = "An error occurred during registration" });
                return errorResponse;
            }
        }

        private string GenerateJwtToken(string username, string email, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("username", username)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = JwtIssuer,
                Audience = JwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
