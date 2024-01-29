namespace eShop.API.DTO;

public class CategoryPostDTO
{
    public string CategoryName { get; set; } = string.Empty;
    public OptionType OptionType { get; set; }
}
public class CategoryPutDTO : CategoryPostDTO
{
    public int CategoryId { get; set; }
}
public class CategoryGetDTO : CategoryPutDTO
{
    //public List<FilterGetDTO>? Filters { get; set; } // för att hämta category med filter. 
    public List<ProductGetDTO>? Products { get; set; }
}
public class CategorySmallGetDTO: CategoryPutDTO
{

}
