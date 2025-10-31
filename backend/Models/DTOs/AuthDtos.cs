namespace ConsistencyHub.DTOs
{
    public class RegisterDto
    {
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class VerifyEmailDto
    {
        public required string Email { get; set; }
        public required string Code { get; set; }
    }

    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class ForgotPasswordDto
    {
        public required string Email { get; set; }
    }

    public class ResetPasswordDto
    {
        public required string Email { get; set; }
        public required string Code { get; set; }
        public required string NewPassword { get; set; }
    }

    public class GoogleLoginDto
    {
        public required string IdToken { get; set; }
    }
}
