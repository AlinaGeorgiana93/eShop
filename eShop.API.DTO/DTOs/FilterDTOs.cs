namespace eShop.API.DTO.DTOs;

public class FilterPostDTO
{
    public string Name { get; set; }
    public string TypeName { get; set; }
    public OptionType OptionType { get; set; }
}

public class FilterPutDTO : FilterPostDTO
{
    public int Id { get; set; }
}

public class FilterGetDTO : FilterPutDTO
{
    //public List<OptionDTO>? Options { get; set; }
}

public class FilterRequestDTO : FilterGetDTO
{
    public int CategoryId { get; set; }
}