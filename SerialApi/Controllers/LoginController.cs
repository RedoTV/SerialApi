using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SerialApi.DatabaseContext;
using SerialApi.FormModels;
using SerialApi.HelperClasses;
using SerialApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SerialApi.Controllers
{
    [ApiController]
    [Route("/login")]
    public class LoginController : Controller
    {
        record JwtClass(string jwt_token);

        AuthorizeContext AuthorizeContext;
        public LoginController(AuthorizeContext authorizeContext)
        {
            AuthorizeContext = authorizeContext;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginForm loginData)
        {
            try
            {
                User User = AuthorizeContext.Users.Where(x => x.Name == loginData.Name && x.Password == loginData.Password).First();
            }
            catch
            {
                return Unauthorized("User Not Found");
            }
            string Role = AuthorizeContext.Users.Where(x => x.Name == loginData.Name && x.Password == loginData.Password).First().Role;
            var claims = new List<Claim> {  new Claim(ClaimTypes.Name, loginData.Name), 
                                            new Claim(ClaimTypes.Role,Role)};
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)), // время действия 1 день
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(jwt);
            
            return Json(new { token = jwtToken,
                              role = Role});

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string name, string password)
        {
            bool userFound = true;
            try
            {
                User newUser = AuthorizeContext.Users.Where(x => x.Name == name).First();
            }
            catch
            {
                userFound = false;
            }

            if (userFound == true)
            {
                return BadRequest("User with this login already exists, try another login");
            }
            else if (userFound == false)
            {
                User newUser = new User();
                newUser!.Name = name;
                newUser.Password = password;
                newUser.Role = "User";
                AuthorizeContext.Users.Add(newUser);
                await AuthorizeContext.SaveChangesAsync();
            }
            return Ok();
        }
    }
}
