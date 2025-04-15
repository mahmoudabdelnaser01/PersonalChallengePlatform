using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalChallengePlatform.Models;
using PersonalChallengePlatform.Services;
using System.Threading.Tasks;

namespace PersonalChallengePlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly INotFoundErrorService _notFoundErrorService;

        public AdminController(INotFoundErrorService notFoundErrorService)
        {
            _notFoundErrorService = notFoundErrorService;
        }

        public async Task<IActionResult> ErrorDashboard()
        {
            var recentErrors = await _notFoundErrorService.GetRecentErrorsAsync(50);
            var errorCounts = await _notFoundErrorService.GetErrorCountsByPathAsync(7);

            var viewModel = new ErrorDashboardViewModel
            {
                RecentErrors = recentErrors,
                ErrorCounts = errorCounts
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ErrorDetails(string path)
        {
            var errors = await _notFoundErrorService.GetErrorsByPathAsync(path);
            return View(errors);
        }
    }
} 