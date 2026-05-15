using BookStore.Mvc.Models.Dashboard;
using BookStore.Mvc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Mvc.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IBookService _bookService;
    private readonly IAuthorService _authorService;
    private readonly IPublisherService _publisherService;
    private readonly IInventoryService _inventoryService;
    private readonly IUserService _userService;

    public DashboardController(IBookService bookService, IAuthorService authorService, IPublisherService publisherService, IInventoryService inventoryService, IUserService userService)
    {
        _bookService = bookService;
        _authorService = authorService;
        _publisherService = publisherService;
        _inventoryService = inventoryService;
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllAsync();
        var authors = await _authorService.GetAllAsync();
        var publishers = await _publisherService.GetAllAsync();

        var model = new DashboardViewModel
        {
            BooksCount = books.Data?.Count ?? 0,
            AuthorsCount = authors.Data?.Count ?? 0,
            PublishersCount = publishers.Data?.Count ?? 0,
            RecentBooks = books.Data?.Take(5).ToList() ?? new()
        };

        if (User.IsInRole("Admin") || User.IsInRole("StoreOwner"))
        {
            var inventory = await _inventoryService.GetAllAsync();
            model.InventoryCount = inventory.Data?.Count ?? 0;
        }

        if (User.IsInRole("Admin"))
        {
            var users = await _userService.GetAllAsync();
            model.UsersCount = users.Data?.Count ?? 0;
        }

        return View(model);
    }
}
