using BookStore.Mvc.Infrastructure;
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Mvc.Controllers;

[Authorize]
public class ReviewsController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "RegisteredUser")]
    public async Task<IActionResult> Create(ReviewCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            this.Error("Rating must be between 1 and 10.");
            return RedirectToAction("Details", "Books", new { id = model.Isbn });
        }

        var result = await _reviewService.CreateAsync(model);
        if (result.IsSuccess) this.Success("Review added.");
        else this.Error(result.Message);

        return RedirectToAction("Details", "Books", new { id = model.Isbn });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "RegisteredUser")]
    public async Task<IActionResult> Delete(string isbn, int reviewerId)
    {
        var result = await _reviewService.DeleteAsync(isbn, reviewerId);
        if (result.IsSuccess) this.Success("Review deleted.");
        else this.Error(result.Message);

        return RedirectToAction("Details", "Books", new { id = isbn });
    }
}
