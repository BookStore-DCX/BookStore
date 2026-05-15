using BookStore.Mvc.Infrastructure;
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Mvc.Controllers;

[Authorize(Roles = "Admin,StoreOwner")]
public class PublishersController : Controller
{
    private readonly IPublisherService _publisherService;
    private readonly IReferenceDataService _referenceDataService;

    public PublishersController(IPublisherService publisherService, IReferenceDataService referenceDataService)
    {
        _publisherService = publisherService;
        _referenceDataService = referenceDataService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var result = await _publisherService.GetAllAsync();
        if (!result.IsSuccess) this.Error(result.Message);
        return View(result.Data ?? new());
    }

    public async Task<IActionResult> Create()
    {
        await LoadStates();
        return View(new PublisherFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PublisherFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadStates();
            return View(model);
        }

        var result = await _publisherService.CreateAsync(model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await LoadStates();
            return View(model);
        }

        this.Success("Publisher created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _publisherService.GetByIdAsync(id);
        if (!result.IsSuccess || result.Data is null)
        {
            this.Error(result.Message);
            return RedirectToAction(nameof(Index));
        }

        await LoadStates();
        return View(new PublisherFormViewModel
        {
            PublisherId = result.Data.PublisherId,
            Name = result.Data.Name,
            City = result.Data.City,
            StateCode = result.Data.StateCode
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PublisherFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadStates();
            return View(model);
        }

        var result = await _publisherService.UpdateAsync(id, model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await LoadStates();
            return View(model);
        }

        this.Success("Publisher updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _publisherService.DeleteAsync(id);
        if (result.IsSuccess) this.Success("Publisher deleted.");
        else this.Error(result.Message);
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadStates()
    {
        var states = await _referenceDataService.GetStatesAsync();
        ViewBag.States = new SelectList(states.Data ?? new(), "StateCode", "StateName");
    }
}
