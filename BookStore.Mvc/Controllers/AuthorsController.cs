using BookStore.Mvc.Infrastructure;
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Mvc.Controllers;

[Authorize(Roles = "Admin,StoreOwner,StoreManager")]
public class AuthorsController : Controller
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var result = await _authorService.GetAllAsync();
        if (!result.IsSuccess) this.Error(result.Message);
        return View(result.Data ?? new());
    }

    public IActionResult Create() => View(new AuthorFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuthorFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _authorService.CreateAsync(model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        this.Success("Author created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _authorService.GetByIdAsync(id);
        if (!result.IsSuccess || result.Data is null)
        {
            this.Error(result.Message);
            return RedirectToAction(nameof(Index));
        }

        return View(new AuthorFormViewModel
        {
            AuthorId = result.Data.AuthorId,
            FirstName = result.Data.FirstName,
            LastName = result.Data.LastName
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AuthorFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _authorService.UpdateAsync(id, model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        this.Success("Author updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _authorService.DeleteAsync(id);
        if (result.IsSuccess) this.Success("Author deleted.");
        else this.Error(result.Message);
        return RedirectToAction(nameof(Index));
    }
}
