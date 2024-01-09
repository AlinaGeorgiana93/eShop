global using eShop.Data.Shared.Interfaces;
using eShop.Data.Shared.Enums;
namespace eShop.Data.Entities;

public class Color : IEntity
{
    public int Id { get; set; }
    public string ColorName { get; set; }
    public List<Product> ?Products { get; set; }
    public OptionType OptionType { get; set; }
}
