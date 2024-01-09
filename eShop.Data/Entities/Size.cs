

using eShop.Data.Shared.Enums;

namespace eShop.Data.Entities;

public class Size : IEntity
{
    public  int Id { get; set; }
    public string SizeName { get; set; }
    public List<Product> ?Products { get; set;}
    public OptionType OptionType { get; set; }
}
