namespace eShop.API.DTO.DTOs;

public class ProductPostDTO
{
    public string ProductName { get; set; } = string.Empty;
    public string PictureUrl { get; set; } = string.Empty;
    public int Price { get; set; } = 0; 
}
public class ProductPutDTO : ProductPostDTO
{
    public int Id { get; set; }
}
public class ProductGetDTO : ProductPutDTO
{
    public List<CategorySmallGetDTO>? Categories { get; set; }
    public List<SizeGetDTO>? Sizes { get; set; }
    public List<ColorGetDTO>? Colors { get; set; }
}
