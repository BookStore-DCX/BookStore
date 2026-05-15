using BookStore.Mvc.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Mvc.Controllers;

public class ReferenceController : Controller
{
    private readonly IReferenceDataService _referenceDataService;

    public ReferenceController(IReferenceDataService referenceDataService)
    {
        _referenceDataService = referenceDataService;
    }

    public async Task<IActionResult> Categories()
    {
        var result = await _referenceDataService.GetCategoriesAsync();
        return View(result.Data ?? new());
    }

    public async Task<IActionResult> Conditions()
    {
        var result = await _referenceDataService.GetBookConditionsAsync();
        return View(result.Data ?? new());
    }

    public async Task<IActionResult> States()
    {
        var result = await _referenceDataService.GetStatesAsync();
        return View(result.Data ?? new());
    }
}
