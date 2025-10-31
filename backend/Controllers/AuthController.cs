using ConsistencyHub.DTOs;
using ConsistencyHub.Models;
using ConsistencyHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsistencyHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IVerificationService _verificationService;
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthController(IUserService userService,
                              IVerificationService verificationService,
                              IEmailService emailService,
                              IAuthService authService,
                              IGoogleAuthService googleAuthService)
        {
            _userService = userService;
            _verificationService = verificationService;
            _emailService = emailService;
            _authService = authService;
            _googleAuthService = googleAuthService;
        }

        private bool ValidatePassword(string password)
        {
            // At least 1 uppercase, 1 digit, 1 symbol, min length 6
            var regex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{6,}$");
            return regex.IsMatch(password);
        }

        private bool ValidateEmail(string email)
        {
            var regex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
            return regex.IsMatch(email);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email and password are required.");

            if (!ValidateEmail(dto.Email))
                return BadRequest("Invalid email format.");

            if (!ValidatePassword(dto.Password))
                return BadRequest("Password must be at least 6 characters, include 1 uppercase letter, 1 number, and 1 symbol.");

            var existing = await _userService.GetByEmailAsync(dto.Email);
            if (existing != null)
                return Conflict("Email already registered.");

            var hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Firstname = dto.Firstname,
                Lastname = dto.Lastname,
                Email = dto.Email.ToLower(),
                PasswordHash = hashed,
                EmailVerified = false,
                IsGoogleUser = false
            };

            await _userService.CreateAsync(user);

            // create verification code (expires in 2 minutes => 120 seconds)
            var code = await _verificationService.CreateCodeAsync(user.Email, CodePurpose.EmailVerification, expirySeconds: 120);

            // send email
            var html = $"<p>Hi {user.Firstname},</p><p>Your verification code: <b>{code.Code}</b></p><p>This code expires in 2 minutes.</p>";
            await _emailService.SendEmailAsync(user.Email, "Verify your ConsistencyHub email", html);

            return Ok(new { message = "Registered successfully. Verification code sent to email." });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            if (!ValidateEmail(dto.Email))
                return BadRequest("Invalid email.");

            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null) return NotFound("User not found.");

            var valid = await _verificationService.ValidateCodeAsync(dto.Email, dto.Code, CodePurpose.EmailVerification);
            if (!valid) return BadRequest("Invalid or expired code.");

            user.EmailVerified = true;
            await _userService.UpdateAsync(user);

            return Ok(new { message = "Email verified." });
        }

        [HttpPost("resend-code")]
        public async Task<IActionResult> ResendCode([FromQuery] string email, [FromQuery] string purpose = "verify")
        {
            if (!ValidateEmail(email)) return BadRequest("Invalid email.");
            var user = await _userService.GetByEmailAsync(email);
            if (user == null) return NotFound("User not found.");

            var codePurpose = purpose == "reset" ? CodePurpose.PasswordReset : CodePurpose.EmailVerification;

            // Check latest code — if it exists and not expired, disallow sending (to prevent spam).
            var latest = await _verificationService.GetLatestCodeAsync(email, codePurpose);
            if (latest != null && DateTime.UtcNow < latest.ExpiresAt)
            {
                var secondsLeft = (int)(latest.ExpiresAt - DateTime.UtcNow).TotalSeconds;
                return BadRequest($"A code has already been sent. Please wait {secondsLeft} seconds or use the existing code.");
            }

            var newCode = await _verificationService.CreateCodeAsync(email, codePurpose, expirySeconds: 120);
            var subject = codePurpose == CodePurpose.EmailVerification ? "Your verification code" : "Your password reset code";
            var html = $"<p>Your code: <b>{newCode.Code}</b></p><p>This code expires in 2 minutes.</p>";
            await _emailService.SendEmailAsync(email, subject, html);

            return Ok(new { message = "Code sent." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ValidateEmail(dto.Email)) return BadRequest("Invalid email.");

            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid credentials.");

            if (!user.EmailVerified)
                return Unauthorized("Email not verified.");

            // verify hashed password
            if (string.IsNullOrEmpty(user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!valid) return Unauthorized("Invalid credentials.");

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ValidateEmail(dto.Email)) return BadRequest("Invalid email.");
            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null) return NotFound("User not found.");

            var latest = await _verificationService.GetLatestCodeAsync(dto.Email, CodePurpose.PasswordReset);
            if (latest != null && DateTime.UtcNow < latest.ExpiresAt)
            {
                var secondsLeft = (int)(latest.ExpiresAt - DateTime.UtcNow).TotalSeconds;
                return BadRequest($"A reset code was already sent. Please wait {secondsLeft} seconds or use the existing code.");
            }

            var code = await _verificationService.CreateCodeAsync(dto.Email, CodePurpose.PasswordReset, expirySeconds: 120);
            var html = $"<p>Your password reset code: <b>{code.Code}</b></p><p>This code expires in 2 minutes.</p>";
            await _emailService.SendEmailAsync(dto.Email, "Password reset code", html);

            return Ok(new { message = "Password reset code sent to your email." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ValidateEmail(dto.Email)) return BadRequest("Invalid email.");
            if (!ValidatePassword(dto.NewPassword)) return BadRequest("New password must be at least 6 characters, include 1 uppercase letter, 1 number, and 1 symbol.");

            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null) return NotFound("User not found.");

            var valid = await _verificationService.ValidateCodeAsync(dto.Email, dto.Code, CodePurpose.PasswordReset);
            if (!valid) return BadRequest("Invalid or expired code.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _userService.UpdateAsync(user);

            return Ok(new { message = "Password reset successful." });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.IdToken)) return BadRequest("No token provided.");

            Google.Apis.Auth.GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await _googleAuthService.ValidateAsync(dto.IdToken);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid Google token: " + ex.Message);
            }

            // payload contains email, name, picture, etc.
            var email = payload.Email.ToLower();
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
            {
                // create new user
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = email,
                    Firstname = payload.GivenName ?? payload.Name,
                    Lastname = payload.FamilyName ?? "",
                    EmailVerified = payload.EmailVerified,
                    IsGoogleUser = true,
                    PasswordHash = null // no password for google-only
                };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

                await _userService.CreateAsync(user);
            }
            else
            {
                // mark as google user if not already
                user.IsGoogleUser = true;
                if (payload.EmailVerified) user.EmailVerified = true;
                await _userService.UpdateAsync(user);
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }
    }
}
