namespace eShop.API.DTO;

public class SizePostDTO
{
    public string SizeName { get; set; } = string.Empty;
    public OptionType OptionType { get; set; }
    
}

public class SizePutDTO : SizePostDTO
{
    public int Id { get; set; }
}

public class SizeGetDTO : SizePutDTO
{
    public bool IsSelected { get; set; }
}
