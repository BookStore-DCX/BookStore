using BookStore.Mvc.Infrastructure;
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BookStore.Mvc.Controllers;

[Authorize(Roles = "RegisteredUser,Admin,StoreOwner")]
public class CartController : Controller
{
    private const string SelectedCopiesSessionKey = "CartSelectedCopies";
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

        var cart = result.Data ?? new();
        ApplySelectedCopies(cart);
        return View(cart);
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

        if (result.IsSuccess) this.Success(result.Message == "Book is already in your cart" ? result.Message : "Book added to cart.");
        else this.Error(result.Message);

        return RedirectToAction("Details", "Books", new { id = isbn });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(string isbn)
    {
        var result = await _cartService.RemoveAsync(GetUserId(), isbn);
        RemoveSelectedCopy(isbn);
        if (result.IsSuccess) this.Success("Item removed.");
        else this.Error(result.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        var result = await _cartService.ClearAsync(GetUserId());
        ClearSelectedCopies();
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(List<int> inventoryIds)
    {
        if (inventoryIds.Count == 0)
        {
            this.Error("Select at least one available copy before checkout.");
            return RedirectToAction(nameof(Index));
        }

        foreach (var inventoryId in inventoryIds.Distinct())
        {
            var result = await _cartService.PurchaseAsync(inventoryId);
            if (!result.IsSuccess)
            {
                this.Error(result.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        await _cartService.ClearAsync(GetUserId());
        ClearSelectedCopies();
        this.Success("Checkout complete. Your purchase history has been updated.");
        return RedirectToAction(nameof(Purchases));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SelectCopy(string isbn, List<int> inventoryIds)
    {
        var selectedCopies = GetSelectedCopies();
        selectedCopies[isbn] = inventoryIds
            .Where(id => id > 0)
            .Distinct()
            .ToList();
        SaveSelectedCopies(selectedCopies);
        return Ok();
    }

    private int GetUserId()
    {
        var raw = HttpContext.Session.GetString(SessionKeys.UserId);
        return int.TryParse(raw, out var userId) ? userId : 0;
    }

    private void ApplySelectedCopies(List<ShoppingCartItemViewModel> cart)
    {
        var selectedCopies = GetSelectedCopies();
        foreach (var item in cart)
        {
            if (selectedCopies.TryGetValue(item.Isbn, out var inventoryIds))
            {
                item.SelectedInventoryIds = inventoryIds
                    .Where(id => item.AvailableCopies.Any(copy => copy.InventoryId == id))
                    .ToList();
            }

            if (item.SelectedInventoryIds.Count == 0 && item.AvailableCopies.Count > 0)
            {
                item.SelectedInventoryIds = item.AvailableCopies
                    .Take(1)
                    .Select(copy => copy.InventoryId)
                    .ToList();
            }

            var selectedCopy = item.AvailableCopies.FirstOrDefault(copy => copy.InventoryId == item.SelectedInventoryIds.FirstOrDefault());
            if (selectedCopy != null)
            {
                item.InventoryId = selectedCopy.InventoryId;
                item.Condition = selectedCopy.Condition;
                item.Price = selectedCopy.Price;
            }
        }
    }

    private Dictionary<string, List<int>> GetSelectedCopies()
    {
        var raw = HttpContext.Session.GetString(SelectedCopiesSessionKey);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return new Dictionary<string, List<int>>();
        }

        return JsonSerializer.Deserialize<Dictionary<string, List<int>>>(raw) ?? new Dictionary<string, List<int>>();
    }

    private void SaveSelectedCopies(Dictionary<string, List<int>> selectedCopies)
    {
        HttpContext.Session.SetString(SelectedCopiesSessionKey, JsonSerializer.Serialize(selectedCopies));
    }

    private void RemoveSelectedCopy(string isbn)
    {
        var selectedCopies = GetSelectedCopies();
        if (selectedCopies.Remove(isbn))
        {
            SaveSelectedCopies(selectedCopies);
        }
    }

    private void ClearSelectedCopies()
    {
        HttpContext.Session.Remove(SelectedCopiesSessionKey);
    }
}
