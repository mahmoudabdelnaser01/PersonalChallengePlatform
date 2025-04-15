using Microsoft.AspNetCore.Identity;
using PersonalChallengePlatform.Models;
using PersonalChallengePlatform.Data;

namespace PersonalChallengePlatform.Services
{
    public interface IDatabaseSeeder
    {
        Task SeedSampleUsersAsync();
    }

    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseSeeder(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task SeedSampleUsersAsync()
        {
            var sampleUsers = new[]
            {
                new { Email = "john.doe@example.com", FirstName = "John", LastName = "Doe", Points = 1200, CompletedChallenges = 15 },
                new { Email = "sarah.smith@example.com", FirstName = "Sarah", LastName = "Smith", Points = 950, CompletedChallenges = 12 },
                new { Email = "mike.johnson@example.com", FirstName = "Mike", LastName = "Johnson", Points = 800, CompletedChallenges = 10 },
                new { Email = "emily.brown@example.com", FirstName = "Emily", LastName = "Brown", Points = 700, CompletedChallenges = 8 },
                new { Email = "david.wilson@example.com", FirstName = "David", LastName = "Wilson", Points = 600, CompletedChallenges = 7 },
                new { Email = "lisa.taylor@example.com", FirstName = "Lisa", LastName = "Taylor", Points = 500, CompletedChallenges = 6 },
                new { Email = "james.anderson@example.com", FirstName = "James", LastName = "Anderson", Points = 400, CompletedChallenges = 5 },
                new { Email = "emma.thomas@example.com", FirstName = "Emma", LastName = "Thomas", Points = 300, CompletedChallenges = 4 },
                new { Email = "alex.martinez@example.com", FirstName = "Alex", LastName = "Martinez", Points = 200, CompletedChallenges = 3 },
                new { Email = "olivia.garcia@example.com", FirstName = "Olivia", LastName = "Garcia", Points = 100, CompletedChallenges = 2 }
            };

            foreach (var sampleUser in sampleUsers)
            {
                var userExists = await _userManager.FindByEmailAsync(sampleUser.Email);
                if (userExists == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = sampleUser.Email,
                        Email = sampleUser.Email,
                        FirstName = sampleUser.FirstName,
                        LastName = sampleUser.LastName,
                        Points = sampleUser.Points,
                        CompletedChallenges = sampleUser.CompletedChallenges,
                        CreatedAt = DateTime.UtcNow,
                        EmailConfirmed = true
                    };

                    await _userManager.CreateAsync(user, "Password123!");
                }
            }
        }
    }
} 