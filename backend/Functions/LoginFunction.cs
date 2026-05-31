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
    public class LoginFunction
    {
        private readonly ILogger<LoginFunction> _logger;
        private const string JwtSecret = "your-super-secret-key-that-should-be-in-keyvault-min-32-chars-long!";
        private const string JwtIssuer = "auction-app";
        private const string JwtAudience = "auction-users";

        public LoginFunction(ILogger<LoginFunction> logger)
        {
            _logger = logger;
        }

        [Function("Login")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "login")] HttpRequestData req)
        {
            _logger.LogInformation("Login function invoked");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                string username = data?.username;
                string password = data?.password;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    badResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                    badResponse.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                    badResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                    await badResponse.WriteAsJsonAsync(new { message = "Username and password are required" });
                    return badResponse;
                }

                // Demo users - in real app, validate against database
                var demoUsers = new[]
                {
                    new { Username = "admin", Password = "password123", Email = "admin@auctions.ch", Role = "admin" },
                    new { Username = "bidder", Password = "password123", Email = "bidder@auctions.ch", Role = "bidder" }
                };

                var user = Array.Find(demoUsers, u => u.Username == username && u.Password == password);
                
                if (user == null)
                {
                    var unauthorizedResponse = req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
                    unauthorizedResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                    unauthorizedResponse.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                    unauthorizedResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                    await unauthorizedResponse.WriteAsJsonAsync(new { message = "Invalid credentials" });
                    return unauthorizedResponse;
                }

                // Generate JWT token
                var token = GenerateJwtToken(user.Username, user.Email, user.Role);

                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                await response.WriteAsJsonAsync(new
                {
                    token = token,
                    user = new
                    {
                        id = Guid.NewGuid().ToString(),
                        username = user.Username,
                        email = user.Email,
                        role = user.Role
                    }
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                errorResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                errorResponse.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                errorResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                await errorResponse.WriteAsJsonAsync(new { message = "An error occurred during login" });
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
