using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalChallengePlatform.Data;
using PersonalChallengePlatform.Models;
using PersonalChallengePlatform.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace PersonalChallengePlatform.Controllers
{
    [Authorize]
    public class ProgressController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProgressController> _logger;

        public ProgressController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            ILogger<ProgressController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [Route("Progress")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found while accessing progress");
                    return NotFound();
                }

                var progress = await _context.Progress
                    .Include(p => p.Challenge)
                    .Where(p => p.UserId == user.Id)
                    .OrderByDescending(p => p.Date)
                    .ToListAsync();

                var viewModel = new ProgressViewModel
                {
                    User = user,
                    Progress = progress ?? new List<Progress>(),
                    TotalChallenges = progress?.Count ?? 0,
                    CompletedChallenges = progress?.Count(p => p.IsCompleted) ?? 0,
                    CurrentStreak = CalculateCurrentStreak(progress ?? new List<Progress>())
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading progress for user");
                return RedirectToAction("Error", "Home");
            }
        }

        [Route("Progress/Challenge/{challengeId}")]
        public async Task<IActionResult> ChallengeProgress(int challengeId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var challenge = await _context.Challenges.FindAsync(challengeId);
                if (challenge == null)
                {
                    _logger.LogWarning("Challenge {ChallengeId} not found", challengeId);
                    return NotFound();
                }

                if (challenge.UserId != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to access progress for challenge {ChallengeId} they don't own", user.Id, challengeId);
                    return Forbid();
                }

                var progress = await _context.Progress
                    .Include(p => p.Challenge)
                    .Where(p => p.ChallengeId == challengeId)
                    .OrderByDescending(p => p.Date)
                    .ToListAsync();
                
                ViewBag.ChallengeId = challengeId;
                return View("Index", progress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing progress for challenge {ChallengeId}", challengeId);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> Create(int challengeId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var challenge = await _context.Challenges.FindAsync(challengeId);
                if (challenge == null)
                {
                    _logger.LogWarning("Challenge {ChallengeId} not found for progress creation", challengeId);
                    return NotFound();
                }

                if (challenge.UserId != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to create progress for challenge {ChallengeId} they don't own", user.Id, challengeId);
                    return Forbid();
                }

                ViewBag.ChallengeId = challengeId;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading progress creation form for challenge {ChallengeId}", challengeId);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Progress progress)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ChallengeId = progress.ChallengeId;
                return View(progress);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var challenge = await _context.Challenges.FindAsync(progress.ChallengeId);
                if (challenge == null)
                {
                    _logger.LogWarning("Challenge {ChallengeId} not found for progress creation", progress.ChallengeId);
                    return NotFound();
                }

                if (challenge.UserId != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to create progress for challenge {ChallengeId} they don't own", user.Id, progress.ChallengeId);
                    return Forbid();
                }

                progress.UserId = user.Id;
                progress.Date = DateTime.UtcNow;

                _context.Add(progress);

                if (progress.IsCompleted)
                {
                    user.Points += progress.Points;
                    user.CompletedChallenges++;
                    _logger.LogInformation("User {UserId} completed challenge {ChallengeId}", user.Id, progress.ChallengeId);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("User {UserId} created progress for challenge {ChallengeId}", user.Id, progress.ChallengeId);
                return RedirectToAction(nameof(Index), new { challengeId = progress.ChallengeId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating progress for challenge {ChallengeId}", progress.ChallengeId);
                ModelState.AddModelError("", "An error occurred while saving progress. Please try again.");
                ViewBag.ChallengeId = progress.ChallengeId;
                return View(progress);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var progress = await _context.Progress
                    .Include(p => p.Challenge)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (progress == null)
                {
                    _logger.LogWarning("Progress {ProgressId} not found for edit", id);
                    return NotFound();
                }

                if (progress.UserId != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to edit progress {ProgressId} they don't own", user.Id, id);
                    return Forbid();
                }

                return View(progress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading progress edit form for ID {ProgressId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Progress progress)
        {
            if (id != progress.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(progress);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var existingProgress = await _context.Progress
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (existingProgress == null)
                {
                    _logger.LogWarning("Progress {ProgressId} not found for edit", id);
                    return NotFound();
                }

                if (existingProgress.UserId != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to edit progress {ProgressId} they don't own", user.Id, id);
                    return Forbid();
                }

                if (existingProgress.IsCompleted != progress.IsCompleted)
                {
                    if (progress.IsCompleted)
                    {
                        user.Points += progress.Points;
                        user.CompletedChallenges++;
                        _logger.LogInformation("User {UserId} completed challenge {ChallengeId}", user.Id, progress.ChallengeId);
                    }
                    else
                    {
                        user.Points -= existingProgress.Points;
                        user.CompletedChallenges--;
                        _logger.LogInformation("User {UserId} uncompleted challenge {ChallengeId}", user.Id, progress.ChallengeId);
                    }
                }

                existingProgress.Description = progress.Description;
                existingProgress.Notes = progress.Notes;
                existingProgress.Points = progress.Points;
                existingProgress.IsCompleted = progress.IsCompleted;

                _context.Update(existingProgress);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("User {UserId} updated progress {ProgressId}", user.Id, id);
                return RedirectToAction(nameof(Index), new { challengeId = progress.ChallengeId });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProgressExists(progress.Id))
                {
                    _logger.LogWarning("Progress {ProgressId} not found during concurrency update", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating progress {ProgressId}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating progress {ProgressId}", id);
                ModelState.AddModelError("", "An error occurred while updating progress. Please try again.");
                return View(progress);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var progress = await _context.Progress
                    .Include(p => p.Challenge)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (progress == null)
                {
                    _logger.LogWarning("Progress {ProgressId} not found for deletion", id);
                    return NotFound();
                }

                if (progress.UserId != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to delete progress {ProgressId} they don't own", user.Id, id);
                    return Forbid();
                }

                return View(progress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading progress delete confirmation for ID {ProgressId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var progress = await _context.Progress
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (progress == null)
                {
                    _logger.LogWarning("Progress {ProgressId} not found for deletion", id);
                    return NotFound();
                }

                if (progress.UserId != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to delete progress {ProgressId} they don't own", user.Id, id);
                    return Forbid();
                }

                var challengeId = progress.ChallengeId;

                if (progress.IsCompleted)
                {
                    user.Points -= progress.Points;
                    user.CompletedChallenges--;
                    _logger.LogInformation("User {UserId} removed completion for challenge {ChallengeId}", user.Id, challengeId);
                }

                _context.Progress.Remove(progress);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("User {UserId} deleted progress {ProgressId}", user.Id, id);
                return RedirectToAction(nameof(Index), new { challengeId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting progress {ProgressId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        private bool ProgressExists(int id)
        {
            return _context.Progress.Any(e => e.Id == id);
        }

        private int CalculateCurrentStreak(List<Progress> progress)
        {
            if (progress == null || !progress.Any())
            {
                return 0;
            }

            var completedProgress = progress
                .Where(p => p.IsCompleted)
                .OrderByDescending(p => p.Date)
                .ToList();

            if (!completedProgress.Any())
            {
                return 0;
            }

            var currentStreak = 1;
            var currentDate = completedProgress[0].Date.Date;

            for (int i = 1; i < completedProgress.Count; i++)
            {
                var previousDate = completedProgress[i].Date.Date;
                if (currentDate.AddDays(-1) == previousDate)
                {
                    currentStreak++;
                    currentDate = previousDate;
                }
                else
                {
                    break;
                }
            }

            return currentStreak;
        }

        [HttpGet]
        [Route("Progress/Leaderboard")]
        public async Task<IActionResult> Leaderboard()
        {
            try
            {
                var users = await _context.Users
                    .OrderByDescending(u => u.Points)
                    .Take(100)
                    .Select(u => new
                    {
                        u.Id,
                        u.FullName,
                        u.Points,
                        u.CompletedChallenges,
                        u.Level
                    })
                    .ToListAsync();

                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading leaderboard");
                return RedirectToAction("Error", "Home");
            }
        }
    }
} 