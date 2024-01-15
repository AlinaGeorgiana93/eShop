namespace eShop.API.DTO;

public class CategoryPostDTO
{
    public string CategoryName { get; set; } = string.Empty;

}
public class CategoryPutDTO : CategoryPostDTO
{
    public int CategoryId { get; set; }
}
public class CategoryGetDTO : CategoryPutDTO
{

}
public class CategorySmallGetDTO: CategoryPutDTO
{

}
