using System.Threading.Tasks;
using ProjectTracker.Service.Enums;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IReportingService
    {
        Task<byte[]> ExportWorkLogsAsync(int userId, ExportFormat format);
        Task<byte[]> ExportActivityAsync(int userId, ExportFormat format);
        Task<byte[]> ExportPerformanceAsync(int userId, ExportFormat format);
    }
}
