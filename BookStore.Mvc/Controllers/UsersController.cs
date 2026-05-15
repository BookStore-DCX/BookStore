using BookStore.Mvc.Infrastructure;
using BookStore.Mvc.Models.Auth;
using BookStore.Mvc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Mvc.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _userService.GetAllAsync();
        if (!result.IsSuccess) this.Error(result.Message);
        return View(result.Data ?? new());
    }
    public IActionResult Create()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _userService.RegisterAsync(model);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        this.Success("User created successfully.");
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(string id)
    {
        var result = await _userService.GetByUsernameAsync(id);
        if (!result.IsSuccess || result.Data is null)
        {
            this.Error(result.Message);
            return RedirectToAction(nameof(Index));
        }

        return View(new UserUpdateViewModel
        {
            UserName = result.Data.UserName,
            FirstName = result.Data.FirstName,
            LastName = result.Data.LastName,
            PhoneNumber = result.Data.PhoneNumber,
            RoleNumber = result.Data.RoleNumber
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UserUpdateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _userService.UpdateAsync(id, model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        this.Success("User updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _userService.DeleteAsync(id);
        if (result.IsSuccess) this.Success("User deleted.");
        else this.Error(result.Message);
        return RedirectToAction(nameof(Index));
    }
}
