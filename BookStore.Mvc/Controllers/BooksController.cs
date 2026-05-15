using BookStore.Mvc.Infrastructure;
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace BookStore.Mvc.Controllers;

public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly IReviewService _reviewService;
    private readonly IReferenceDataService _referenceDataService;
    private readonly IPublisherService _publisherService;

    public BooksController(IBookService bookService, IReviewService reviewService, IReferenceDataService referenceDataService, IPublisherService publisherService)
    {
        _bookService = bookService;
        _reviewService = reviewService;
        _referenceDataService = referenceDataService;
        _publisherService = publisherService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string? title, string? authorName, string? description)
    {
        var result = string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(authorName) && string.IsNullOrWhiteSpace(description)
            ? await _bookService.GetAllAsync()
            : await _bookService.SearchAsync(title, authorName, description);

        ViewBag.TitleFilter = title;
        ViewBag.AuthorFilter = authorName;
        ViewBag.DescriptionFilter = description;

        if (!result.IsSuccess)
        {
            this.Error(result.Message);
        }

        return View(result.Data ?? new());
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(string id)
    {
        var result = await _bookService.GetByIsbnAsync(id);
        if (!result.IsSuccess || result.Data is null)
        {
            this.Error(result.Message);
            return RedirectToAction(nameof(Index));
        }

        var reviews = await _reviewService.GetByBookAsync(id);
        result.Data.Reviews = reviews.Data ?? new();
        return View(result.Data);
    }

    [Authorize(Roles = "Admin,StoreOwner")]
    public async Task<IActionResult> Create()
    {
        await LoadDropDowns();
        return View(new BookFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,StoreOwner")]
    public async Task<IActionResult> Create(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropDowns();
            return View(model);
        }

        var result = await _bookService.CreateAsync(model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await LoadDropDowns();
            return View(model);
        }

        this.Success("Book created.");
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,StoreOwner")]
    public async Task<IActionResult> Edit(string id)
    {
        var detailResult = await _bookService.GetByIsbnAsync(id);
        if (!detailResult.IsSuccess || detailResult.Data is null)
        {
            this.Error(detailResult.Message);
            return RedirectToAction(nameof(Index));
        }

        await LoadDropDowns();
        return View(new BookFormViewModel
        {
            Isbn = detailResult.Data.Isbn,
            Title = detailResult.Data.Title,
            Description = detailResult.Data.Description,
            Category = detailResult.Data.CategoryId,
            Edition = NormalizeEditionForNumericInput(detailResult.Data.Edition),
            PublisherId = detailResult.Data.PublisherId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,StoreOwner")]
    public async Task<IActionResult> Edit(string id, BookFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropDowns();
            return View(model);
        }

        var result = await _bookService.UpdateAsync(id, model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await LoadDropDowns();
            return View(model);
        }

        this.Success("Book updated.");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,StoreOwner")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _bookService.DeleteAsync(id);
        if (result.IsSuccess) this.Success("Book deleted.");
        else this.Error(result.Message);
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropDowns()
    {
        var categories = await _referenceDataService.GetCategoriesAsync();
        var publishers = await _publisherService.GetAllAsync();
        ViewBag.Categories = new SelectList(categories.Data ?? new(), "CatId", "CatDescription");
        ViewBag.Publishers = new SelectList(publishers.Data ?? new(), "PublisherId", "Name");
    }

    private static string? NormalizeEditionForNumericInput(string? edition)
    {
        if (string.IsNullOrWhiteSpace(edition))
        {
            return null;
        }

        var match = Regex.Match(edition, @"\d+");
        if (match.Success)
        {
            return match.Value;
        }

        return edition.Trim().ToLowerInvariant() switch
        {
            "first" => "1",
            "second" => "2",
            "third" => "3",
            "fourth" => "4",
            "fifth" => "5",
            "sixth" => "6",
            "seventh" => "7",
            "eighth" => "8",
            "ninth" => "9",
            "tenth" => "10",
            "i" => "1",
            "ii" => "2",
            "iii" => "3",
            "iv" => "4",
            "v" => "5",
            "vi" => "6",
            "vii" => "7",
            "viii" => "8",
            "ix" => "9",
            "x" => "10",
            _ => null
        };
    }
}
