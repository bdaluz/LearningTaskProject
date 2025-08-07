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
        private readonly IEmailService _emailservice;

        public UserController(IUserService userservice, IAuthService authService, IEmailService emailService)
        {
            _userservice = userservice;
            _authservice = authService;
            _emailservice = emailService;
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
            return Ok(new { token = _authservice.CreateToken(user) });   
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

        [Route("SendPassReset")]
        [HttpPost]
        public async Task<IActionResult> SendResetPasswordToken([FromBody] EmailDTO emailDTO)
        {
            if (!await _userservice.ValidateUserEmail(emailDTO.Email)) return BadRequest(new { message = "No account found with that email address." });

            string token = await _userservice.GetPasswordResetToken(emailDTO.Email);
            await _emailservice.SendEmail(emailDTO.Email, "TaskProject - Password Reset", @"
                            <html>
                                <body>
                                    <h1>Password Reset</h1>
                                    <p>Enter your token: <strong>" + token + @"</strong></p>
                                    <p>Click <a href='https://github.com/bdaluz/LearningTaskProject'>here</a> to reset your password.</p>
                                    <footer>
                                        <p>LearningTaskProject Team</p>
                                    </footer>
                                </body>
                            </html>");

            return Accepted(new { message = "Password reset email sent successfully." });
        }

        [Route("VerifyPasswordResetToken")]
        [HttpPost]
        public async Task<IActionResult> VerifyPasswordResetToken([FromBody] VerifyPasswordResetTokenDTO verifyPasswordResetTokenDTO)
        {
            if (!await _userservice.ValidateToken(verifyPasswordResetTokenDTO.Email, verifyPasswordResetTokenDTO.Token)) return BadRequest(new { message = "Invalid Token." });
            return Ok();
        }


        [Route("ChangePassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (!await _userservice.ValidateToken(changePasswordDTO.Email, changePasswordDTO.Token)) return BadRequest(new { message = "Invalid Token." });

            await _userservice.UpdatePassword(changePasswordDTO.Email, changePasswordDTO.NewPassword);

            return Ok(new { message = "Password reset successfully" });
        }
    }
}
