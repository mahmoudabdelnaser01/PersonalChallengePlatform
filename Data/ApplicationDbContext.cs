using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalChallengePlatform.Models;

namespace PersonalChallengePlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add DbSet for your models
        public DbSet<Challenge> Challenges { get; set; } = null!;
        public DbSet<Progress> Progress { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<NotFoundError> NotFoundErrors { get; set; } = null!;
        public DbSet<UserAchievement> UserAchievements { get; set; } = null!;
        public DbSet<UserStatistic> UserStatistics { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Challenge>()
                .HasOne(c => c.User)
                .WithMany(u => u.CreatedChallenges)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Challenge>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Challenges)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Progress>()
                .HasOne(p => p.Challenge)
                .WithMany(c => c.Progress)
                .HasForeignKey(p => p.ChallengeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Progress>()
                .HasOne(p => p.User)
                .WithMany(u => u.ChallengeProgress)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure UserAchievement
            builder.Entity<UserAchievement>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.Achievements)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure UserStatistic
            builder.Entity<UserStatistic>()
                .HasOne(us => us.User)
                .WithMany(u => u.Statistics)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Add indexes for performance
            builder.Entity<Challenge>()
                .HasIndex(c => c.UserId);

            builder.Entity<Challenge>()
                .HasIndex(c => c.CategoryId);

            builder.Entity<Progress>()
                .HasIndex(p => p.UserId);

            builder.Entity<Progress>()
                .HasIndex(p => p.ChallengeId);

            builder.Entity<Progress>()
                .HasIndex(p => p.Date);

            // Seed default categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fitness", Description = "Physical fitness and exercise goals" },
                new Category { Id = 2, Name = "Learning", Description = "Educational and skill development goals" },
                new Category { Id = 3, Name = "Career", Description = "Professional development and career goals" },
                new Category { Id = 4, Name = "Personal Development", Description = "Self-improvement and personal growth" },
                new Category { Id = 5, Name = "Health", Description = "Health and wellness goals" }
            );
        }
    }
}