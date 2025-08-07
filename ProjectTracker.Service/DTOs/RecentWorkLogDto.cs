namespace ProjectTracker.Service.DTOs
{
    public class RecentWorkLogDto
    {
        public System.DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Employee { get; set; } = string.Empty;
        public decimal Duration { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
