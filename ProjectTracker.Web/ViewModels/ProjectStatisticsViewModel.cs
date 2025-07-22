// ViewModels/ProjectStatisticsViewModel.cs
namespace ProjectTracker.Web.ViewModels
{
    public class ProjectStatisticsViewModel
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal TotalActualCost { get; set; }
    }
}