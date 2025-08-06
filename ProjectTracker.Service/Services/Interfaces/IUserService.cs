using ProjectTracker.Core.Entities;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IUserService
    {
        Task ToggleActiveAsync(string userId);
        Task UpdateKvkkAsync(string userId, bool consent);
    }
}
