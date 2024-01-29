namespace eShop.Data.Entities;

public class Product : IEntity
{
    public int Id { get; set; }
    public string ProductName { get; set; }

    public List <Category> Categories { get; set; }=   new List<Category>();
    public List <Color> Colors { get; set; } = new List<Color>();
    public List <Size> Sizes { get; set; } = new List<Size>();


    
}
