namespace eShop.UI.Models.Link;

public class LinkGroup
{
    public string Name { get; set; } = string.Empty; //vårat rubrik namn i sidorutslistan för själva gruppen.
    public List<LinkOption> LinkOptions { get; set; } = []; // Vi måste lägga till en Colletion av linkoption/en lista
}
