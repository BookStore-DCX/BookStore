using Microsoft.AspNetCore.Mvc;

namespace BookStore.Mvc.Infrastructure;

public static class TempDataExtensions
{
	public static void Success(this Controller controller, string message)
	{
		controller.TempData["Success"] = message;
	}

	public static void Error(this Controller controller, string message)
	{
		controller.TempData["Error"] = message;
	}
}
