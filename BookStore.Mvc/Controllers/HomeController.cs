using System.Diagnostics;
using BookStore.Mvc.Models;
using BookStore.Mvc.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly IBookService _bookService;

    public HomeController(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllAsync();
        return View(books.Data?.Take(8).ToList() ?? new());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
