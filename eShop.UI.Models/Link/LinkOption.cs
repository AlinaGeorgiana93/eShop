namespace eShop.UI.Models.Link;

public class LinkOption // är varje länk som vi kan klicka på, linkoption använder vi
                        // inne i linkgroup för att tala om vilka options/länkar som finns i gruppen.     
{
    public int Id { get; set; } //Här får in ett id så vi vet id på länken vi klickat
    public string Name { get; set; } = string.Empty; //Här får in namnet på länken 
    public bool IsSelected { get; set; } //Här har vi en property som antingen är sant eller falsk ifall den är markerad (en kryssruta tex)
}
