

using eShop.API.DTO;
using eShop.API.DTO.DTOs;

namespace eShop.UI.Services;

public class UIService(CategoryHttpClient categoryHttp, 
    ProductHttpClient productHttp, IMapper mapper) //Här initioerar vi CategoryHttpClient när vi anropar UIService.
                                                        //Eftersom vi inisierar CategoryHttpClient här i standardkonstruktorn,
                                                        //kommer categoryHttp fungera som en variabel inne i klassen.                                               
{
    List<CategoryGetDTO> Categories { get; set; } = []; //vi vill kunna hämta kategorier, listan sparar undan kategorierna,
                                                        //När vi lägger till Categories property, använder vi INTE entiteten Category
                                                        //utan DTO eftersom datat som kommer från APIet kommer som DTO(från APIet till gränssnittet)

    public List<ProductGetDTO> Products { get; private set; } = [];

    public List<LinkGroup> CategoryLinkGroups { get; private set; } = //vår kategorilista med våra boxar med länkar i(Rubrik)
    [
        new LinkGroup
        {
            Name = "Categories",
            /*,LinkOptions = new(){
                new LinkOption { Id = 1, Name = "Women", IsSelected = true },
                new LinkOption { Id = 2, Name = "Men", IsSelected = false },
                new LinkOption { Id = 3, Name = "Children", IsSelected = false }
            }*/
        }
    ];
    public int CurrentCategoryId { get; set; } //vårat aktuella CategoryId så att vi vet vilken kategori som vi arbetar med.

    public async Task GetLinkGroup() //metod för att ta reda på vart CategoryLinkGroups anvönds någonstans, måste vara asynkront eftersom vårat API är asynkront. 
    {
        Categories = await categoryHttp.GetCategoriesAsync();  // categoryHttp-instansen som är initierad i konstruktorn används för att anropa GetCategoriesAsync.
        CategoryLinkGroups[0].LinkOptions = mapper.Map<List<LinkOption>>(Categories); //Ur resultatet ovan, mappar vi om kategorier som kommer tillbaks till en lista av LinkOptions, så vi får ut korrekt data. 
        var linkOption = CategoryLinkGroups[0].LinkOptions.FirstOrDefault();  // denna rad hämtar ut den första LinkOptions för när vi ska hämta tex produkter. 
        linkOption!.IsSelected = true; 
    }

    public async Task OnCategoryLinkClick(int id)
    {
        CurrentCategoryId = id; // här sparar vi undan kategori-id lokalt först.
        await GetProductsAsync(); //sedan hämtar vi våra produkter
        //Products.ForEach(p => p.Colors!.First().IsSelected = true); //Vi ska för varje produkt som vi hämtar ut säga att Colors ska vara markerade eller ej. // Samma scenario som för kategorier med kryssruta och Trekanten som följer med det man väljer att klicka. 
                                                                      //Vi loopar igenom varje produkt, och för varje produkt så hämtar vi ut första färgen och säger den ska vara markerad.

        CategoryLinkGroups[0].LinkOptions.ForEach(l => l.IsSelected = false); //TREKANTEN. Här loopar vi igenom våra LinkOptions och sätter alla till false, för att visa vilken som är aktiv. 
        CategoryLinkGroups[0].LinkOptions.Single(l => l.Id.Equals(CurrentCategoryId)).IsSelected = true; //TREKANTEN. Sedan hämtar vi ut den LInkOptions som motsvarar kategori-id. Vi har potetionellt valt en ny kategori. Ifall vi väljer en annan kategori ska trekanten följa med. 
    }

    public async Task GetProductsAsync() =>
        Products = await productHttp.GetProductsAsync(CurrentCategoryId); //här anropar vi GetproduktAsync i productHttp med id och kategori som vi vill hämta. Hämtar våra produkter och lägger dom i produktlistan. Dess uppgift är att hämta produkter som är kopplade till id.

}