namespace BookStore.Mvc.Models.Reference;

public class CategoryViewModel
{
    public int CatId { get; set; }
    public string? CatDescription { get; set; }
}

public class BookConditionViewModel
{
    public int Ranks { get; set; }
    public string? Description { get; set; }
    public string? FullDescription { get; set; }
    public decimal Price { get; set; }
}

public class StateViewModel
{
    public string StateCode { get; set; } = string.Empty;
    public string? StateName { get; set; }
}
