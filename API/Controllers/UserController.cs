using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using Microsoft.AspNetCore.RateLimiting;
using Services.DTOs.User;

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

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                User user = await _userservice.ValidateLogin(loginDTO.Username, loginDTO.Password);

                return Ok(_authservice.CreateToken(user.Id.ToString()));
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized();
            }
        }


        [Route("CreateUser")]
        [HttpPost]
        public async Task<IActionResult> Signup([FromBody] SignupDTO signupDTO)
        {
            if (await _userservice.ValidateUsername(signupDTO.Username))
            {
                return Conflict("Username already exists.");
            }

            if (await _userservice.ValidateUserEmail(signupDTO.Email))
            {
                return Conflict("Email already exists.");
            }

            await _userservice.AddUser(signupDTO.Username, signupDTO.Email, signupDTO.Password);
            return Created();
        }

        [Route("SendPassReset")]
        [HttpPost]
        public async Task<IActionResult> SendResetPasswordToken([FromBody] string email)
        {
            if (!await _userservice.ValidateUserEmail(email)) return BadRequest("No account found with that email address.");

            string token = await _userservice.GetPasswordResetToken(email);
            await _emailservice.SendEmail(email, "TaskProject - Password Reset", @"
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

            return Accepted("Password reset email sent successfully.");
        }

        //[Route("ConfirmPasswordResetToken")]


        [Route("ChangePassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (!await _userservice.ValidateToken(changePasswordDTO.Email, changePasswordDTO.Token)) return BadRequest("Invalid token.");

            await _userservice.UpdatePassword(changePasswordDTO.Email, changePasswordDTO.NewPassword);

            return Ok("Password reset successfully");
        }
    }
}
