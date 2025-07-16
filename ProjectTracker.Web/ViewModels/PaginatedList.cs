using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Web.ViewModels
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        // Arama ve filtreleme için
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        // Sayfa başına kayıt sayısı
        public int[] PageSizeOptions { get; } = { 10, 20, 50, 100 };

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize, string currentFilter = "", string currentSort = "")
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            CurrentFilter = currentFilter;
            CurrentSort = currentSort;

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, string currentFilter = "", string currentSort = "")
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize, currentFilter, currentSort);
        }
    }
}