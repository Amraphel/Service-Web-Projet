using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using UserService.Entities;

namespace GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // api/User/login
        [HttpPost("login")]
        public async Task<ActionResult<JWTAndUser>> Login(UserLogin model)
        {
            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                // Set the base address of the API you want to call
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                // Send a POST request to the login endpoint
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/login", model);

                // Check if the response status code is 200 (OK)
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = await response.Content.ReadFromJsonAsync<UserDTO>();

                    if (result != null)
                    {
                        //On genere le token
                        var jwt = GenerateJwtToken(result.Id);
                        //On cree l'objet contenant l'user et le jeton
                        var userAndToken = new JWTAndUser() { Token = jwt, User = result };
                        return Ok(userAndToken);
                    }
                    else
                    {
                        return BadRequest("Login failed");
                    }
                }
                else
                {
                    return BadRequest("Login failed");
                }
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreate model)
        {
            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                // Set the base address of the API you want to call
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                // Send a POST request to the login endpoint
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/register", model);
                Console.WriteLine(response.Content.ToString());
                Console.WriteLine(response.StatusCode);

                // Check if the response status code is 2xx
                if (response.IsSuccessStatusCode)
                {
                    // You can deserialize the response content here if needed
                    var result = await response.Content.ReadFromJsonAsync<UserDTO>();
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Register failed");
                }
            }
        }


        [Authorize]
        [HttpGet("jwt")]
        public ActionResult<string> Jwt()
        {
            //On recuper les donnee de l'user
            var userName = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;

            foreach (var claim in User.Claims)
            {
                Console.WriteLine(claim.Type + " " + claim.Value);
            }

            return Ok($"Hello, {userName}");
        }
        private string GenerateJwtToken(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", userId.ToString())
            };
            //Creation de la cle 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("H6K5Wiv9JDyy8mEba5Sc6zvH3HmsFk853K85kZ2J77aR"));

            //Creation du certificat 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //Creation du token avec le certificat
            var token = new JwtSecurityToken(
                issuer: "TodoProject",
                audience: "localhost:5000",
                claims: claims,
                expires: DateTime.Now.AddMinutes(3000),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }



}
