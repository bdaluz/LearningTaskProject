using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using Microsoft.AspNetCore.RateLimiting;
using Services.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userservice;
        private readonly IAuthService _authservice;

        public UserController(IUserService userservice, IAuthService authService)
        {
            _userservice = userservice;
            _authservice = authService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            string? idFromClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idFromClaim == null) return BadRequest("Null");

            int userid = int.Parse(idFromClaim);

            var user = await _userservice.GetUserBasicInfo(userid);

            return Ok(user);
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            User? user = await _userservice.ValidateLogin(loginDTO.Username, loginDTO.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password." });

            var accessToken = _authservice.CreateToken(user);
            var refreshToken = await _authservice.CreateRefreshTokenAsync(user.Id);

            _authservice.SetRefreshTokenCookie(refreshToken.Token, HttpContext);

            return Ok(new { token = accessToken });
        }


        [Route("CreateUser")]
        [HttpPost]
        public async Task<IActionResult> Signup([FromBody] SignupDTO signupDTO)
        {
            if (await _userservice.ValidateUsername(signupDTO.Username))
            {
                return Conflict(new { message = "Username already exists." });
            }

            if (await _userservice.ValidateUserEmail(signupDTO.Email))
            {
                return Conflict(new { message = "Email already exists." });
            }

            await _userservice.AddUser(signupDTO.Username, signupDTO.Email, signupDTO.Password);
            return StatusCode(201, new { message = "Created new User." });
        }

        [Route("SendPasswordReset")]
        [HttpPost]
        public async Task<IActionResult> SendResetPasswordToken([FromBody] EmailDTO emailDTO)
        {
            await _userservice.PasswordResetRequest(emailDTO.Email);

            return Accepted(new { message = "If an account with that email address exists, a password reset email has been sent." });
        }

        [Route("ChangePassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                await _userservice.UpdatePassword(changePasswordDTO.Token, changePasswordDTO.NewPassword);
                return Ok(new { message = "Password reset successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = "Invalid or expired token." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = "Invalid or expired token." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal server error occurred. Please try again later." });
            }
        }


        [Route("Refresh")]
        [HttpPost]
        public async Task<IActionResult> Refresh()
        {
            var refreshTokenValue = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshTokenValue))
            {
                return Unauthorized(new { message = "Refresh token not found." });
            }

            var refreshToken = await _authservice.ValidateRefreshTokenAsync(refreshTokenValue);
            if (refreshToken == null)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token." });
            }

            await _authservice.RevokeRefreshTokenAsync(refreshTokenValue);
            var newAccessToken = _authservice.CreateToken(refreshToken.User);
            var newRefreshToken = await _authservice.CreateRefreshTokenAsync(refreshToken.UserId);

            _authservice.SetRefreshTokenCookie(newRefreshToken.Token, HttpContext);

            return Ok(new { token = newAccessToken });
        }


        [Route("Logout")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var refreshTokenValue = Request.Cookies["refreshToken"];
            
            if (!string.IsNullOrEmpty(refreshTokenValue))
            {
                await _authservice.RevokeRefreshTokenAsync(refreshTokenValue);
            }

            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Logged out successfully." });
        }

    }
}
