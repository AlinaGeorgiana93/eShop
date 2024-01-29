namespace eShop.API.DTO.DTOs;

public class SizePostDTO
{
    public string SizeName { get; set; } = string.Empty;
    public OptionType OptionType { get; set; }
    public bool IsSelected { get; set; }
}

public class SizePutDTO : SizePostDTO
{
    public int Id { get; set; }
}

public class SizeGetDTO : SizePutDTO
{
}
