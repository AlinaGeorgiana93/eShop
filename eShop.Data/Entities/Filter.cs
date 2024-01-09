namespace eShop.Data.Entities;

public class Filter : IEntity
{ 
    public int Id { get; set; }
    public string Name { get; set; } 
    public string SortBy { get; set; }

    public List <Category> Categories { get; set; }
}
