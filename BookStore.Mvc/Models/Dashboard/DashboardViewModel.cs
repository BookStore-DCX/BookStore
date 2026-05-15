using BookStore.Mvc.Models.Catalog;

namespace BookStore.Mvc.Models.Dashboard;

public class DashboardViewModel
{
    public int BooksCount { get; set; }
    public int AuthorsCount { get; set; }
    public int PublishersCount { get; set; }
    public int InventoryCount { get; set; }
    public int UsersCount { get; set; }
    public List<BookListItemViewModel> RecentBooks { get; set; } = new();
}
