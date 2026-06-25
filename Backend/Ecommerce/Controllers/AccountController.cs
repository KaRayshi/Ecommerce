using Ecommerce.Dto.User;
using Ecommerce.Dtos.Account;
using Ecommerce.Helpers;
using Ecommerce.Interfaces;
using Ecommerce.Mappers;
using Ecommerce.Models;
using Ecommerce.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ecommerce.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppUserRepository _appUserRepo;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IOtpRepository _otpRepo;
        private readonly OtpService _otpService;
        private readonly EmailService _emailService;

        public AccountController(UserManager<AppUser> userManager, IAppUserRepository appUserRepo, ITokenService tokenService, SignInManager<AppUser> signInManager, IOtpRepository otpRepo,
            OtpService otpService,
            EmailService emailService)
        {
            _userManager = userManager;
            _appUserRepo = appUserRepo;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _otpRepo = otpRepo;
            _otpService = otpService;
            _emailService = emailService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginDto.UsernameOrEmail.ToLower() || u.Email.ToLower() == loginDto.UsernameOrEmail.ToLower());

            if(user == null)
            {
                return Unauthorized("Invalid Username");
            }

            if (!user.EmailConfirmed)
            {
                return Unauthorized("Your email has not been verified yet. Please check your inbox for the OTP code.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                if (user == null)
                {
                    return Unauthorized("Username not found and/or password incorrect");
                }
            }

            var roles = await _userManager.GetRolesAsync(user);

            var primaryRole = roles.FirstOrDefault() ?? "User";

            return Ok(
                new AppUserDto
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user, roles),
                    Role = primaryRole

                }
            );

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (createdUser.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                if (roleResult.Succeeded)
                {
                    string code = _otpService.GenerateSecureOtp();

                    var otpRecord = new OtpVerification
                    {
                        Email = appUser.Email,
                        OtpCode = code,
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                        IsUsed = false
                    };

                    await _otpRepo.CreateOtpAsync(otpRecord);
                    await _emailService.SendOtpEmailAsync(appUser.Email, code);
                    // === NEW OTP LOGIC ENDS HERE ===

                    return Ok(new { message = "Account created successfully. Please check your email for the verification code." });
                }
                else
                {
                    return StatusCode(500, roleResult.Errors);
                }
            }
            else
            {
                return BadRequest(createdUser.Errors);
            }
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var users = await _userManager.Users
                .Where(u => u.Email == verifyDto.Email)
                .ToListAsync();

            if (users.Count == 0) return BadRequest("User not found.");
            if (users.Count > 1) return BadRequest("Multiple accounts found with this email. Please contact support.");

            var user = users.First();

            if (user.EmailConfirmed) return BadRequest("Email is already verified.");

            var otpRecord = await _otpRepo.GetLatestOtpByEmailAsync(verifyDto.Email);

            if (otpRecord == null || otpRecord.OtpCode != verifyDto.OtpCode)
                return BadRequest("Invalid OTP Code.");

            if (otpRecord.IsUsed)
                return BadRequest("This OTP has already been used.");

            if (DateTime.UtcNow > otpRecord.ExpiresAt)
                return BadRequest("This OTP has expired. Please request a new one.");

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            otpRecord.IsUsed = true;
            await _otpRepo.UpdateOtpAsync(otpRecord);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles.ToList());

            return Ok(new AppUserDto
            {
                Username = user.UserName,
                Email = user.Email,
                Token = token,
                Role = roles.FirstOrDefault() ?? "User"
            });
        }

        [HttpGet("View_All_User")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewAllUser([FromQuery] AppUserQueryObject query)
        {
            var users = await _appUserRepo.GetAllUserAsync(query);

            var userDtos = users.Select(u => u.ToAppUserDto()).ToList();

            return Ok(userDtos);
        }

        [HttpGet("View_User/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewUser(string id)
        {
            var user = await _appUserRepo.GetUserByIdAsync(id);

           if(user == null)
            {
                return NotFound("User not found.");
            }

            var userDtos = user.ToAppUserDto();

            return Ok(userDtos);
        }

        [HttpGet("myProfile")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("No valid token found.");
            }

            var user = await _appUserRepo.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userDtos = user.ToAppUserDto();

            return Ok(userDtos);
        }

        [HttpPut("Update_User/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var updatedUser = await _appUserRepo.UpdateUserAsync(id, update);

            if (updatedUser == null)
            {
                return NotFound("User not found.");
            }

            return Ok("Account has been updated!");
        }

        [HttpPut("userProfileUpdate")]
        [Authorize]
        public async Task<IActionResult> UserUpdateProfile([FromBody] UpdateUserDto update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var updatedUser = await _appUserRepo.UpdateUserAsync(userId, update);

            if (updatedUser == null)
            {
                return NotFound("User not found.");
            }

            return Ok("Account has been updated!");
        }

        [HttpDelete("Delete_User/{id}")]
        public async Task<IActionResult> deleteUser(string id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _appUserRepo.deleteUserAsync(id);

            if(user == null)
            {
                return BadRequest("User not found");
            }

            return Ok("User deleted");
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestOtpDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var users = await _userManager.Users
               .Where(u => u.Email == requestDto.Email)
               .ToListAsync();

            if (users.Count > 1) return BadRequest("Multiple accounts found with this email. Please contact support.");

            var user = users.FirstOrDefault();
            if (user == null)
            {
                return Ok(new { message = "If this email exists, an OTP has been sent." });
            }

            string code = _otpService.GenerateSecureOtp();

            var otpRecord = new OtpVerification
            {
                Email = requestDto.Email,
                OtpCode = code,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false
            };

            await _otpRepo.CreateOtpAsync(otpRecord);

            await _emailService.SendOtpEmailAsync(requestDto.Email, code);

            return Ok(new { message = "If this email exists, an OTP has been sent." });
        }

        [HttpPost("verify-otp-and-reset")]
        public async Task<IActionResult> VerifyOtpAndReset([FromBody] VerifyOtpDto verifyDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var otpRecord = await _otpRepo.GetLatestOtpByEmailAsync(verifyDto.Email);

            if (otpRecord == null || otpRecord.OtpCode != verifyDto.OtpCode)
                return BadRequest("Invalid OTP Code.");

            if (otpRecord.IsUsed)
                return BadRequest("This OTP has already been used.");

            if (DateTime.UtcNow > otpRecord.ExpiresAt)
                return BadRequest("This OTP has expired. Please request a new one.");

            var users = await _userManager.Users
               .Where(u => u.Email == verifyDto.Email)
               .ToListAsync();

            if (users.Count > 1) return BadRequest("Multiple accounts found with this email. Please contact support.");

            var user = users.FirstOrDefault();
            if (user == null) return BadRequest("User not found.");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, verifyDto.NewPassword);

            if (!resetResult.Succeeded) return BadRequest(resetResult.Errors);

            otpRecord.IsUsed = true;
            await _otpRepo.UpdateOtpAsync(otpRecord);

            return Ok(new { message = "Password has been successfully reset." });
        }

    }
}
