using BookStore.Mvc.Infrastructure;
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Mvc.Controllers;

[Authorize(Roles = "RegisteredUser,Admin,StoreOwner")]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var result = await _cartService.GetCartAsync(userId);
        if (!result.IsSuccess) this.Error(result.Message);
        return View(result.Data ?? new());
    }

    [HttpGet]
    public async Task<IActionResult> Count()
    {
        var result = await _cartService.GetCartAsync(GetUserId());
        return Json(new { count = result.Data?.Count ?? 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string isbn)
    {
        var result = await _cartService.AddAsync(new ShoppingCartCreateViewModel
        {
            UserId = GetUserId(),
            Isbn = isbn
        });

        if (result.IsSuccess) this.Success("Book added to cart.");
        else this.Error(result.Message);

        return RedirectToAction("Details", "Books", new { id = isbn });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove()
    {
        var result = await _cartService.RemoveAsync(GetUserId());
        if (result.IsSuccess) this.Success("Item removed.");
        else this.Error(result.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        var result = await _cartService.ClearAsync(GetUserId());
        if (result.IsSuccess) this.Success("Cart cleared.");
        else this.Error(result.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Purchase(int inventoryId)
    {
        var result = await _cartService.PurchaseAsync(inventoryId);
        if (result.IsSuccess) this.Success("Purchase recorded.");
        else this.Error(result.Message);
        return RedirectToAction(nameof(Purchases));
    }

    public async Task<IActionResult> Purchases()
    {
        var result = await _cartService.MyPurchasesAsync();
        if (!result.IsSuccess) this.Error(result.Message);
        return View(result.Data ?? new());
    }

    private int GetUserId()
    {
        var raw = HttpContext.Session.GetString(SessionKeys.UserId);
        return int.TryParse(raw, out var userId) ? userId : 0;
    }
}
