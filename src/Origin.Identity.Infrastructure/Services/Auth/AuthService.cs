using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Origin.Identity.Application.Common;
using Origin.Identity.Application.Services.Auth;
using Origin.Identity.Contracts.Auth;
using Origin.Identity.Infrastructure.Identity;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace Origin.Identity.Infrastructure.Services.Auth
{
    public sealed class AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IWebHostEnvironment webHostEnvironment,
        IConfiguration config
    ) : IAuthService
    {
        public async Task<Result<Guid>> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);

            if (existingUser is not null)
            {
                return Result<Guid>.Failure("A user with this email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };

            var createResult = await userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
            {
                var error = string.Join(
                    Environment.NewLine,
                    createResult.Errors.Select(x => x.Description)
                );

                return Result<Guid>.Failure(error);
            }

            return Result<Guid>.Success(user.Id);
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var result = await signInManager.PasswordSignInAsync(
                request.UserName,
                request.Password,
                isPersistent: true,
                lockoutOnFailure: true
            );

            if (!result.Succeeded)
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password.");
            }

            return Result<AuthResponseDto>.Success(new AuthResponseDto());
        }

        public async Task<Result<string>> DeleteUserAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user is null)
            {
                return Result<string>.Failure("User not found");
            }

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return Result<string>.Failure(result.Errors.ToString()!);
            }

            return Result<string>.Success("Deletion success");
        }

        public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return Result<string>.Failure("Invalid request.");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var payload = new
            {
                FirstName = user.FirstName ?? user.UserName,
                Email = user.Email!,
                ResetPasswordUrl = $"https://localhost:7181/auth/reset-password?userId={Uri.EscapeDataString(user.Id.ToString())}&token={Uri.EscapeDataString(token)}",
            };

            string jsonPayload = JsonSerializer.Serialize(payload);

            var mode = 2;

            switch (mode)
            {
                case 1:
                    {
                        using var client = new SmtpClient(
                            config["Email:PSmtp:Host"],
                            int.Parse(config["Email:PSmtp:Port"]!)
                        )
                        {
                            EnableSsl = false,
                        };

                        var mail = new MailMessage(
                            config["Email:From"]!,
                            user.Email!,
                            "Reset Password",
                            Render("password-reset", jsonPayload)
                        )
                        {
                            IsBodyHtml = true,
                        };
                        await client.SendMailAsync(mail);
                        break;
                    }

                case 2:
                    {
                        using var client = new SmtpClient(
                            config["Email:GSmtp:Host"],
                            int.Parse(config["Email:GSmtp:Port"]!)
                        )
                        {
                            EnableSsl = bool.Parse(config["Email:GSmtp:EnableSsl"]!),
                            Credentials = new NetworkCredential(
                                config["Email:GSmtp:Username"],
                                config["Email:GSmtp:Password"]
                            ),
                        };

                        var mail = new MailMessage(
                            config["Email:From"]!,
                            user.Email!,
                            "Reset Password",
                            Render("password-reset", jsonPayload)
                        )
                        {
                            IsBodyHtml = true,
                        };
                        await client.SendMailAsync(mail);

                        break;
                    }
            }

            return Result<string>.Success(token);
        }

        public async Task<Result<string>> ResetPasswordAsync(ResetPasswordRequestDto request)
        {

            var userId = Uri.UnescapeDataString(request.UserId);


            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return Result<string>.Failure("Invalid request.");
            }


            var decodedToken = Uri.UnescapeDataString(request.Token);

            var resetResult = await userManager.ResetPasswordAsync(
                user,
               decodedToken,
                request.NewPassword
            );

            if (!resetResult.Succeeded)
            {
                return Result<string>.Failure(
                    string.Join(", ", resetResult.Errors.Select(e => e.Description))
                );
            }

            return Result<string>.Success("Success");
        }

        public string Render(string templateName, string jsonPayload)
        {
            var path = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "HTML",
                $"{templateName}.html"
            );

            var template = File.ReadAllText(path);

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonPayload)!;

            foreach (var kv in data)
            {
                template = template.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            }

            return template;
        }
    }
}
