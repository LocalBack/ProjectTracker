using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Web.Extensions
{
    public static class WorkLogExtensions
    {
        public static int GetDetailCount(this WorkLogDto workLog)
        {
            return workLog?.Details?.Count ?? 0;
        }

        public static int GetAttachmentCount(this WorkLogDto workLog)
        {
            return workLog?.Attachments?.Count ?? 0;
        }
    }
}