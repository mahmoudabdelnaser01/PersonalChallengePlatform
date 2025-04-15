using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class ChallengeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChallengeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChallengeController(
            ApplicationDbContext context, 
            ILogger<ChallengeController> logger, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        private async Task<List<SelectListItem>> GetCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories.ToListAsync();
                if (categories == null || !categories.Any())
                {
                    _logger.LogWarning("No categories found in the database.");
                    return new List<SelectListItem>();
                }
                return categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
                return new List<SelectListItem>();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found while accessing challenges");
                    return NotFound();
                }

                var challenges = await _context.Challenges
                    .Include(c => c.Category)
                    .Include(c => c.Progress)
                    .Where(c => c.UserId == user.Id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                var viewModel = new ChallengeViewModel
                {
                    User = user,
                    Challenges = challenges ?? new List<Challenge>(),
                    Categories = await _context.Categories.ToListAsync() ?? new List<Category>()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading challenges for user");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        [Route("Challenge/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var challenge = await _context.Challenges
                    .Include(c => c.Category)
                    .Include(c => c.Progress)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (challenge == null)
                {
                    _logger.LogWarning("Challenge {ChallengeId} not found", id);
                    return NotFound();
                }

                if (challenge.UserId != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to access challenge {ChallengeId} they don't own", user.Id, id);
                    return Forbid();
                }

                return View(challenge);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing challenge details for ID {ChallengeId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        [Route("Challenge/Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = new ChallengeCreateViewModel
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(30),
                    Categories = await GetCategoriesAsync()
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading challenge creation form");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [Route("Challenge/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChallengeCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = await GetCategoriesAsync();
                return View(viewModel);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var challenge = new Challenge
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    CategoryId = viewModel.CategoryId,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Add(challenge);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("User {UserId} created new challenge {ChallengeId}", user.Id, challenge.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating challenge");
                ModelState.AddModelError("", "An error occurred while creating the challenge. Please try again.");
                viewModel.Categories = await GetCategoriesAsync();
                return View(viewModel);
            }
        }

        [HttpGet]
        [Route("Challenge/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var challenge = await _context.Challenges.FindAsync(id);
                if (challenge == null)
                {
                    _logger.LogWarning("Challenge {ChallengeId} not found for edit", id);
                    return NotFound();
                }

                if (challenge.UserId != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to edit challenge {ChallengeId} they don't own", user.Id, id);
                    return Forbid();
                }

                var viewModel = new ChallengeCreateViewModel
                {
                    Title = challenge.Title,
                    Description = challenge.Description,
                    StartDate = challenge.StartDate,
                    EndDate = challenge.EndDate,
                    CategoryId = challenge.CategoryId,
                    Categories = await GetCategoriesAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading challenge edit form for ID {ChallengeId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [Route("Challenge/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChallengeCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = await GetCategoriesAsync();
                return View(viewModel);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var challenge = await _context.Challenges.FindAsync(id);
                if (challenge == null)
                {
                    _logger.LogWarning("Challenge {ChallengeId} not found for edit", id);
                    return NotFound();
                }

                if (challenge.UserId != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to edit challenge {ChallengeId} they don't own", user.Id, id);
                    return Forbid();
                }

                challenge.Title = viewModel.Title;
                challenge.Description = viewModel.Description;
                challenge.StartDate = viewModel.StartDate;
                challenge.EndDate = viewModel.EndDate;
                challenge.CategoryId = viewModel.CategoryId;

                _context.Update(challenge);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("User {UserId} updated challenge {ChallengeId}", user.Id, id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ChallengeExists(id))
                {
                    _logger.LogWarning("Challenge {ChallengeId} not found during concurrency update", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating challenge {ChallengeId}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating challenge {ChallengeId}", id);
                ModelState.AddModelError("", "An error occurred while updating the challenge. Please try again.");
                viewModel.Categories = await GetCategoriesAsync();
                return View(viewModel);
            }
        }

        [HttpGet]
        [Route("Challenge/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var challenge = await _context.Challenges
                    .Include(c => c.Category)
                    .Include(c => c.Progress)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (challenge == null)
                {
                    _logger.LogWarning("Challenge {ChallengeId} not found for deletion", id);
                    return NotFound();
                }

                if (challenge.UserId != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to delete challenge {ChallengeId} they don't own", user.Id, id);
                    return Forbid();
                }

                return View(challenge);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading challenge delete confirmation for ID {ChallengeId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [Route("Challenge/Delete/{id}")]
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

                var challenge = await _context.Challenges
                    .Include(c => c.Progress)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (challenge == null)
                {
                    _logger.LogWarning("Challenge {ChallengeId} not found for deletion", id);
                    return NotFound();
                }

                if (challenge.UserId != user.Id)
                {
                    _logger.LogWarning("User {UserId} attempted to delete challenge {ChallengeId} they don't own", user.Id, id);
                    return Forbid();
                }

                if (challenge.Progress != null && challenge.Progress.Any())
                {
                    _context.Progress.RemoveRange(challenge.Progress);
                }

                _context.Challenges.Remove(challenge);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("User {UserId} deleted challenge {ChallengeId}", user.Id, id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting challenge {ChallengeId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        private bool ChallengeExists(int id)
        {
            return _context.Challenges.Any(e => e.Id == id);
        }
    }
} 