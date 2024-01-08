global using eShop.Data.Shared.Interfaces;

namespace eShop.Data.Entities;

public class Category : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}
