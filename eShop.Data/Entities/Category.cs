namespace eShop.Data.Entities;

public class Category : IEntity
{
    public int Id { get; set; }
    public string ?CategoryName { get; set; }
    public List<Product>? Products { get; set; }
    public List<Filter>? Filters { get; set; }
    public OptionType OptionType { get; set; }
}
