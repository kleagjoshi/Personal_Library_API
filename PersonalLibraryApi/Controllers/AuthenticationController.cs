using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PersonalLibraryApi.Data.Models;
using PersonalLibraryApi.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PersonalLibraryApi.Data.ViewModels.Authentication;
using Microsoft.EntityFrameworkCore;

namespace PersonalLibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        //inject some services

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        //Refresh tokens
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context, IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;

            //Refresh tokens
           _tokenValidationParameters = tokenValidationParameters;
        }

        //send data from client side
        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterVM payload)
        {
            //if the user didnt provide both fields
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide all required fields");
            }

            //check if the user exists

            var userExists = await _userManager.FindByEmailAsync(payload.Email);

            if (userExists != null)
            {
                return BadRequest($"user {payload.Email} already exists");
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                Email = payload.Email,
                UserName = payload.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(newUser, payload.Password);

            if (!result.Succeeded)
            {
                return BadRequest("User could not be created");
            }

            //add user to a role - all will be users 
           await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            //after the user is created(registered) log him in

            //check if user exists in database now after registration

            var user = await _userManager.FindByEmailAsync(payload.Email);

            //generate token for the user 

            if (user != null && await _userManager.CheckPasswordAsync(user, payload.Password))
            {
                var tokenValue = await GenerateJwtTokenAsync(user, "");

                return Ok(tokenValue);
            }

            return Created(nameof(Register), $"User {payload.Email} created");
        }

        //login

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody] LoginVM payload)
        {
            //if the user didnt provide both fields
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide all required fields");
            }

            //check if user exists in database

            var user = await _userManager.FindByEmailAsync(payload.Email);

            //generate token for the user 

            if (user != null && await _userManager.CheckPasswordAsync(user, payload.Password))
            {
                var tokenValue = await GenerateJwtTokenAsync(user, "");

                return Ok(tokenValue);
            }

            //if user does not exists
            return Unauthorized();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestVM payload)
        {
            try
            {
                var result = await VerifyAndGenerateTokenAsync(payload);
                if (result == null)
                    return BadRequest("Invalid tokens");

                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

        private async Task<AuthResultVM> VerifyAndGenerateTokenAsync(TokenRequestVM payload)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                //Check 1 - Check JWT token format
                var tokenInVerification = jwtTokenHandler.ValidateToken(payload.Token, _tokenValidationParameters, out var validatedToken);

                //Check 2 - Encryption algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false) return null;
                }


                //Check 3 - Refresh token exists in the DB
                var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(n => n.Token == payload.RefreshToken);
                if (dbRefreshToken == null) throw new Exception("Refresh token does not exists in our DB");
                else
                {
                    //Check 4 - Validate Id
                    var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                    if (dbRefreshToken.JwtId != jti) throw new Exception("Token does not match");

                    //Check 5 - Refresh token expiration
                    if (dbRefreshToken.DateExpired <= DateTime.UtcNow) throw new Exception("Your refresh token has expired, please re-authenticate!");

                    //Check 6 - Refresh token Revoked
                    if (dbRefreshToken.isRevoked) throw new Exception("Refresh token is revoked");


                    //Generate new token (with existing refresh token)
                    var dbUserData = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
                    var newTokenResponse = GenerateJwtTokenAsync(dbUserData, payload.RefreshToken);

                    return await newTokenResponse;
                }
            }
            catch (SecurityTokenExpiredException)
            {
                var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(n => n.Token == payload.RefreshToken);
                //Generate new token (with existing refresh token)
                var dbUserData = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
                var newTokenResponse = GenerateJwtTokenAsync(dbUserData, payload.RefreshToken);

                return await newTokenResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<AuthResultVM> GenerateJwtTokenAsync(ApplicationUser user, string existingRefreshToken)
        {

            var authClaims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name,user.UserName),
                        new Claim(ClaimTypes.NameIdentifier,user.Id),
                        new Claim(JwtRegisteredClaimNames.Email,user.Email),
                        new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                    };

            //Add User Roles
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles) //only one
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }



            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JMT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken();

            if (string.IsNullOrEmpty(existingRefreshToken))
            {
                refreshToken = new RefreshToken()
                {
                    JwtId = token.Id,
                    isRevoked = false,
                    UserId = user.Id,
                    DateAdded = DateTime.UtcNow,
                    DateExpired = DateTime.UtcNow.AddMonths(6),
                    Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
                };
                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();
            }

            var response = new AuthResultVM()
            {
                Token = jwtToken,
                RefreshToken = (string.IsNullOrEmpty(existingRefreshToken)) ? refreshToken.Token : existingRefreshToken,
                ExpiresAt = token.ValidTo
            };

            return response;

        }

       


    }
}





    