
namespace eShop.API.DTO;

public class ColorPostDTO
{
    public string ColorName { get; set; } = string.Empty;
    public OptionType OptionType { get; set; }
    public string? ColorHex { get; set; }
    public string? BkColorHex { get; set; }
    public bool IsSelected { get; set; }
}

public class ColorPutDTO : ColorPostDTO
{
    public int Id { get; set; }
}

public class ColorGetDTO : ColorPutDTO
{

}
