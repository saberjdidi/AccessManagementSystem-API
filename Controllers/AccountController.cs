using AccessManagementSystem_API.Dtos;
using AccessManagementSystem_API.Models;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AccessManagementSystem_API.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser
            {
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                UserName = registerDto.FullName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            if (registerDto.Roles is null)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
            else
            {
                foreach (var role in registerDto.Roles)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Account Created Successfully!"
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if(user is null)
            {
                return Unauthorized(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found"
                });  
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                return Unauthorized(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid Credentials"
                });
            }

            var token = GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                IsSuccess = true,
                Message = "Login Success"
            });
        }

        private string GenerateToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration.GetSection("JWTSetting").GetSection("securityKey").Value!;
            var key = Encoding.ASCII.GetBytes(secretKey);
            var expiration = DateTime.UtcNow.AddHours(1);

            var roles = _userManager.GetRolesAsync(user).Result;

            List<Claim> claims = [
                 new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                 new(JwtRegisteredClaimNames.Name, user.FullName ?? ""),
                 new(JwtRegisteredClaimNames.NameId, user.Id ?? ""),
                 new(JwtRegisteredClaimNames.Aud, _configuration.GetSection("JWTSetting").GetSection("validAudience").Value!),
                 new(JwtRegisteredClaimNames.Iss, _configuration.GetSection("JWTSetting").GetSection("validIssuer").Value!),
                ];

            foreach (var role in roles) {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpGet("detail")]
        public async Task<ActionResult<UserDetailDto>> GetUserDetail()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(currentUserId!);

            if (user is null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found"
                });
            }

            return Ok(new UserDetailDto
            {
                Id = user.Id,
                FullName = user.UserName,
                Email = user.Email,
                AccessFailedCount = user.AccessFailedCount,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                Roles = [.. await _userManager.GetRolesAsync(user)]
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDetailDto>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userResponseList = new List<UserDetailDto>();

            foreach (var user in users) { 
              var roles = _userManager.GetRolesAsync(user).Result.ToArray();

                userResponseList.Add(new UserDetailDto
                {
                    Id=user.Id,
                    Email = user.Email,
                    FullName = user.UserName,
                    Roles = roles
                });
            }
            
            return Ok(userResponseList);

            //var users = await _userManager.Users.Select(u => new UserDetailDto
            //{
            //    Id = u.Id,  
            //    Email = u.Email,
            //    FullName = u.FullName,
            //    Roles = _userManager.GetRolesAsync(u).Result.ToArray()
            //}).ToListAsync();

            //return Ok(users);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDto forgotPassword)
        {
            var _user = await _userManager.FindByEmailAsync(forgotPassword.Email);
            if (_user is null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User does not exist with this email"
                });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(_user);
            var resetLink = $"http://localhost:4200/reset-password?email={_user.Email}&token={WebUtility.UrlEncode(token)}";

            var client = new RestClient("https://sandbox.api.mailtrap.io/api/send/3305998");

            var request = new RestRequest
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };

            request.AddHeader("Authorization", "Bearer 07136cfdf9f4efbba65ec5b22406ae1b");
            request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("application/json", "{\"from\":{\"email\":\"hello@demomailtrap.com\",\"name\":\"Mailtrap Test\"},\"to\":[{\"email\":\"saberjdidi30@gmail.com\"}],\"template_uuid\":\"2b35b0fd-d84b-4d6c-b996-8b55701816c2\",\"template_variables\":{\"company_info_name\":\"Test_Company_info_name\",\"name\":\"Test_Name\",\"company_info_address\":\"Test_Company_info_address\",\"company_info_city\":\"Test_Company_info_city\",\"company_info_zip_code\":\"Test_Company_info_zip_code\",\"company_info_country\":\"Test_Company_info_country\"}}", ParameterType.RequestBody);
            //var response = client.Post(request);
            request.AddJsonBody(new
            {
                from = new {email = "hello@demomailtrap.com" },
                to = new[] { new { email = _user.Email } },
                template_uuid = "2b35b0fd-d84b-4d6c-b996-8b55701816c2",
                template_variables = new { user_email = _user.Email, pass_reset_link = resetLink }
            });

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Email sent with password reset link. Please check you email."
                });    
            } else
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = response.Content!.ToString()
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            //resetPasswordDto.Token = WebUtility.UrlDecode(resetPasswordDto.Token);

            if (user is null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User does not exist with this email"
                });
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Password Reset Successfully."
                });
            }
            else
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = result.Errors.FirstOrDefault()!.Description
                });
            }
        }
    }
}
