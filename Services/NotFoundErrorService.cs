using Microsoft.EntityFrameworkCore;
using PersonalChallengePlatform.Data;
using PersonalChallengePlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalChallengePlatform.Services
{
    public interface INotFoundErrorService
    {
        Task LogNotFoundErrorAsync(string path, string referrer, string userId, string userAgent, string ipAddress);
        Task<List<NotFoundError>> GetRecentErrorsAsync(int count = 50);
        Task<List<NotFoundError>> GetErrorsByPathAsync(string path);
        Task<List<NotFoundError>> GetErrorsByUserAsync(string userId);
        Task<Dictionary<string, int>> GetErrorCountsByPathAsync(int days = 7);
    }

    public class NotFoundErrorService : INotFoundErrorService
    {
        private readonly ApplicationDbContext _context;

        public NotFoundErrorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogNotFoundErrorAsync(string path, string referrer, string userId, string userAgent, string ipAddress)
        {
            var error = new NotFoundError
            {
                Path = path,
                Referrer = referrer,
                UserId = userId,
                UserAgent = userAgent,
                IPAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            _context.NotFoundErrors.Add(error);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NotFoundError>> GetRecentErrorsAsync(int count = 50)
        {
            return await _context.NotFoundErrors
                .Include(e => e.User)
                .OrderByDescending(e => e.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<NotFoundError>> GetErrorsByPathAsync(string path)
        {
            return await _context.NotFoundErrors
                .Include(e => e.User)
                .Where(e => e.Path == path)
                .OrderByDescending(e => e.Timestamp)
                .ToListAsync();
        }

        public async Task<List<NotFoundError>> GetErrorsByUserAsync(string userId)
        {
            return await _context.NotFoundErrors
                .Include(e => e.User)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Timestamp)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetErrorCountsByPathAsync(int days = 7)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            
            return await _context.NotFoundErrors
                .Where(e => e.Timestamp >= cutoffDate)
                .GroupBy(e => e.Path)
                .Select(g => new { Path = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToDictionaryAsync(x => x.Path, x => x.Count);
        }
    }
} 