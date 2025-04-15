using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalChallengePlatform.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Logging;

namespace PersonalChallengePlatform.Controllers
{
    [Authorize]
    public class UserSettingsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<UserSettingsController> _logger;

        public UserSettingsController(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            ILogger<UserSettingsController> logger)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserSettingsViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                Bio = user.Bio,
                PhoneNumber = user.PhoneNumber,
                Location = user.Location,
                AvatarUrl = user.AvatarUrl,
                SelectedTheme = user.SelectedTheme,
                EnableEmailNotifications = user.EnableEmailNotifications
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserSettingsViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Index", model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found while attempting to update profile");
                    return NotFound();
                }

                // Log the profile update attempt
                _logger.LogInformation("Profile update attempted by user {UserId} at {Timestamp}. Changes: Bio={Bio}, Phone={Phone}, Location={Location}",
                    user.Id, DateTime.UtcNow, model.Bio, model.PhoneNumber, model.Location);

                user.Bio = model.Bio ?? string.Empty;
                user.PhoneNumber = model.PhoneNumber ?? string.Empty;
                user.Location = model.Location ?? string.Empty;
                user.SelectedTheme = model.SelectedTheme ?? "light";
                user.EnableEmailNotifications = model.EnableEmailNotifications;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Profile successfully updated for user {UserId} at {Timestamp}",
                        user.Id, DateTime.UtcNow);
                    TempData["SuccessMessage"] = "Profile updated successfully.";
                }
                else
                {
                    _logger.LogWarning("Profile update failed for user {UserId} at {Timestamp}. Errors: {Errors}",
                        user.Id, DateTime.UtcNow, string.Join(", ", result.Errors.Select(e => e.Description)));
                    TempData["ErrorMessage"] = "Failed to update profile.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating profile for user at {Timestamp}",
                    DateTime.UtcNow);
                TempData["ErrorMessage"] = "An unexpected error occurred while updating your profile.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture == null || profilePicture.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(profilePicture.ContentType.ToLower()))
            {
                TempData["ErrorMessage"] = "Only JPG, PNG, and GIF files are allowed.";
                return RedirectToAction(nameof(Index));
            }

            // Validate file size (max 5MB)
            if (profilePicture.Length > 5 * 1024 * 1024)
            {
                TempData["ErrorMessage"] = "File size must be less than 5MB.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Create uploads directory if it doesn't exist
                var uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profiles");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                // Delete old profile picture if it exists
                if (!string.IsNullOrEmpty(user.AvatarUrl))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.AvatarUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Generate unique filename
                var fileExtension = Path.GetExtension(profilePicture.FileName).ToLower();
                var fileName = $"{user.Id}_{DateTime.UtcNow.Ticks}{fileExtension}";
                var filePath = Path.Combine(uploadsDir, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                // Update user avatar URL
                user.AvatarUrl = $"/uploads/profiles/{fileName}";
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile picture updated successfully.";
                    _logger.LogInformation("Profile picture updated for user {UserId}", user.Id);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update profile picture in database.";
                    _logger.LogError("Failed to update profile picture for user {UserId}", user.Id);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture for user {UserId}", user.Id);
                TempData["ErrorMessage"] = "Failed to upload profile picture. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UserSettingsViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Index", model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found while attempting to update password");
                    return NotFound();
                }

                if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword))
                {
                    ModelState.AddModelError(string.Empty, "Current password and new password are required.");
                    return View("Index", model);
                }

                // Log the password change attempt
                _logger.LogInformation("Password change attempted by user {UserId} at {Timestamp}",
                    user.Id, DateTime.UtcNow);

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Password successfully changed for user {UserId} at {Timestamp}",
                        user.Id, DateTime.UtcNow);
                    TempData["SuccessMessage"] = "Password changed successfully.";
                }
                else
                {
                    _logger.LogWarning("Password change failed for user {UserId} at {Timestamp}. Errors: {Errors}",
                        user.Id, DateTime.UtcNow, string.Join(", ", result.Errors.Select(e => e.Description)));
                    TempData["ErrorMessage"] = "Failed to change password. Please check your current password.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing password for user at {Timestamp}",
                    DateTime.UtcNow);
                TempData["ErrorMessage"] = "An unexpected error occurred while changing your password.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 