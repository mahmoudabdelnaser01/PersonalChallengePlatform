using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PersonalChallengePlatform.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace PersonalChallengePlatform.Controllers
{
    [Authorize]
    [Route("Dashboard")]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // TODO: Add logic to fetch user's challenges, progress, and statistics
            var viewModel = new DashboardViewModel
            {
                User = user,
                ActiveChallenges = new List<Challenge>(), // Will be populated with actual data
                RecentProgress = new List<Progress>(),    // Will be populated with actual data
                Statistics = new DashboardStatistics()    // Will be populated with actual data
            };

            return View(viewModel);
        }
    }

    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            User = null!;
            ActiveChallenges = new List<Challenge>();
            RecentProgress = new List<Progress>();
            Statistics = new DashboardStatistics();
        }

        public ApplicationUser User { get; set; }
        public List<Challenge> ActiveChallenges { get; set; }
        public List<Progress> RecentProgress { get; set; }
        public DashboardStatistics Statistics { get; set; }
    }

    public class DashboardStatistics
    {
        public int TotalChallenges { get; set; }
        public int CompletedChallenges { get; set; }
        public int CurrentStreak { get; set; }
        public int TotalPoints { get; set; }
    }
} 