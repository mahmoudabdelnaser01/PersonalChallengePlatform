using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PersonalChallengePlatform.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PersonalChallengePlatform.Data;
using Microsoft.Extensions.Logging;
using PersonalChallengePlatform.Models.ViewModels;
using PersonalChallengePlatform.Services;

namespace PersonalChallengePlatform.Controllers
{
    [Authorize]
    [Route("Leaderboard")]
    public class LeaderboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LeaderboardController> _logger;
        private readonly IAchievementService _achievementService;

        public LeaderboardController(
            UserManager<ApplicationUser> userManager, 
            ApplicationDbContext context,
            ILogger<LeaderboardController> logger,
            IAchievementService achievementService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _achievementService = achievementService ?? throw new ArgumentNullException(nameof(achievementService));
        }

        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Current user not found while accessing leaderboard");
                    return NotFound();
                }

                // Get top 10 users with their progress
                var users = await _context.Users
                    .AsNoTracking()
                    .Include(u => u.ChallengeProgress)
                    .OrderByDescending(u => u.Points)
                    .Take(10)
                    .Select(u => new LeaderboardUser
                    {
                        UserId = u.Id,
                        UserName = u.FullName,
                        AvatarUrl = u.AvatarUrl ?? "/images/default-avatar.svg",
                        Points = u.Points,
                        CompletedChallenges = u.CompletedChallenges,
                        AchievementLevel = _achievementService.DetermineAchievementLevel(u.Points)
                    })
                    .ToListAsync();

                // Calculate current user's rank
                var currentUserRank = await _context.Users
                    .AsNoTracking()
                    .CountAsync(u => u.Points > currentUser.Points) + 1;

                // Get total user count
                var totalUsers = await _context.Users
                    .AsNoTracking()
                    .CountAsync();

                var viewModel = new LeaderboardViewModel
                {
                    TopUsers = users,
                    CurrentUserRank = currentUserRank,
                    TotalUsers = totalUsers,
                    CurrentUser = new LeaderboardUser
                    {
                        UserId = currentUser.Id,
                        UserName = currentUser.FullName,
                        AvatarUrl = currentUser.AvatarUrl ?? "/images/default-avatar.svg",
                        Points = currentUser.Points,
                        CompletedChallenges = currentUser.CompletedChallenges,
                        AchievementLevel = _achievementService.DetermineAchievementLevel(currentUser.Points)
                    }
                };

                _logger.LogInformation("Leaderboard accessed by user {UserId}", currentUser.Id);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading leaderboard");
                return RedirectToAction("Error", "Home");
            }
        }
    }

    public class LeaderboardViewModel
    {
        public List<LeaderboardUser> TopUsers { get; set; } = new();
        public int CurrentUserRank { get; set; }
        public int TotalUsers { get; set; }
        public LeaderboardUser? CurrentUser { get; set; }
    }

    public class LeaderboardUser
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = "/images/default-avatar.svg";
        public int Points { get; set; }
        public int CompletedChallenges { get; set; }
        public string AchievementLevel { get; set; } = "Beginner";
    }
} 