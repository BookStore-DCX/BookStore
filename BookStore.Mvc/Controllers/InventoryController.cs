using BookStore.Mvc.Infrastructure;
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Mvc.Controllers;

[Authorize(Roles = "Admin,StoreOwner")]
public class InventoryController : Controller
{
    private readonly IInventoryService _inventoryService;
    private readonly IBookService _bookService;
    private readonly IReferenceDataService _referenceDataService;

    public InventoryController(IInventoryService inventoryService, IBookService bookService, IReferenceDataService referenceDataService)
    {
        _inventoryService = inventoryService;
        _bookService = bookService;
        _referenceDataService = referenceDataService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _inventoryService.GetAllAsync();
        if (!result.IsSuccess) this.Error(result.Message);
        return View(result.Data ?? new());
    }

    public async Task<IActionResult> Create()
    {
        await LoadDropDowns();
        return View(new InventoryFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InventoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropDowns();
            return View(model);
        }

        var result = await _inventoryService.CreateAsync(model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await LoadDropDowns();
            return View(model);
        }

        this.Success("Inventory copy added.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, string isbn, int ranks, byte? purchased)
    {
        await LoadDropDowns();
        return View(new InventoryFormViewModel
        {
            InventoryId = id,
            Isbn = isbn,
            Ranks = ranks,
            Purchased = purchased
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InventoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropDowns();
            return View(model);
        }

        var result = await _inventoryService.UpdateAsync(id, model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await LoadDropDowns();
            return View(model);
        }

        this.Success("Inventory updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _inventoryService.DeleteAsync(id);
        if (result.IsSuccess) this.Success("Inventory deleted.");
        else this.Error(result.Message);
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropDowns()
    {
        var books = await _bookService.GetAllAsync();
        var conditions = await _referenceDataService.GetBookConditionsAsync();
        ViewBag.Books = new SelectList(books.Data ?? new(), "Isbn", "Title");
        ViewBag.Conditions = new SelectList(conditions.Data ?? new(), "Ranks", "FullDescription");
    }
}
