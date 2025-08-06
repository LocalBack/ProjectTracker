using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using ProjectTracker.Service.Services.Interfaces;

namespace ProjectTracker.Service.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task ToggleActiveAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null) return;

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateKvkkAsync(string userId, bool consent)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null) return;

            user.KVKK = consent;
            user.KvkkTimestamp = consent ? DateTime.UtcNow : null;
            await _context.SaveChangesAsync();
        }
    }
}
