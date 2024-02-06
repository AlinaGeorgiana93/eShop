using Microsoft.VisualBasic;

namespace eShop.API.Extensions.Extensions;

//1.Klassen måste vara publik och statisk
//2. Metoden måste vara publik statik 
//3. Första parametern måste vara deklarerad som this för att den ska kunna arbeta
//på objekt. Den är har datatypen av den objektstyp vi vill anropa metoden på. 

public static class HttpExtensions
{
    //AddEndPoint lägger till/registrerar metoderna så vi kan anropa de.
    //När gränssnittet anropar APIet ska den kunna köra dessa metoder.
    public static void AddEndpoint<TEntity, TPostDto, TPutDto, TGetDto>(this WebApplication app)  //<-----//Vi vill registrera entiteter som har IEntity , Vi får in Category-Post-Put-Get från Program    
   where TEntity : class, IEntity where TPostDto : class where TPutDto : class where TGetDto : class      //Raden ovan skickar vi in olika DTOs för att dom innehåller
    {                                                                                                     //olika typer av information(TPostDto, TPutDto, TGetDto)
        var node = typeof(TEntity).Name.ToLower(); //Här den hämtar namnet på entiteten 
        app.MapGet($"/api/{node}s/" + "{id}", HttpSingleAsync<TEntity, TGetDto>); 
        app.MapGet($"/api/{node}s", HttpGetAsync<TEntity, TGetDto>);
        app.MapPost($"/api/{node}s", HttpPostAsync<TEntity, TPostDto>); 
        app.MapPut($"/api/{node}s/" + "{id}", HttpPutAsync<TEntity, TPutDto>);
        app.MapDelete($"/api/{node}s/" + "{id}", HttpDeleteAsync<TEntity>); //har ingen Dto, enbart TEntity som talar om vilken entitet(category,products,color) vi vill ta bort.

        //1.MapGet skapar metoden så att vi kan göra anropen från sidan.Runtimen att vi kan komma åt den
        //2."($"/api/{node}s" är vår sökväg vi bygger upp
        //3."HttpGetAsync<TEntity, TGetDto>)" = är vår extensionmetod nedanför som anropas som i sin
        // tur anropar vår Get-metod i databasen.
        //4.Dessa kodrader kommer att registrera t ex Get-sökvägen så den blir tillgänglig,
        //inte bara från swagger, swagger kommer att visa den visuellt för oss som utvecklare,
        //och andra beroende på vilka rättigheter vi satt i CURS
    }

    public static void AddEndpoint<TEntity, TDto>(this WebApplication app) //Körs när vi vill ha våra kopplingstabeller och när vi behöver  
     where TEntity : class where TDto : class                              //bara ha en Post o Delete, vi behöver aldrig uppdatera två id,
    {                                                                      //Istället kan vi lägga till och ta bort en ny koppling
                                                                           //När vi använder kopplingstabell har den alltid enbart två id
                                                                           //och det kommer vara samma oavsett om vi gör en post eller delete
                                                                           //för den är identisk. Därför behövs bara en DTO istället för
                                                                           //TPostDto, TPutDto, TGetDto eftersom det skulle ge samma resultat.
                                                                           //Istället använder vi en DTO som kan antingen vara en post eller delete t ex. 

        var node = typeof(TEntity).Name.ToLower(); //här tar vi in entitetsnamnet
        app.MapPost($"/api/{node}s", HttpPostReferenceAsync<TEntity, TDto>); //Vi gör en Post för att lägga till en koppling med sökvägen ($"/api/{node}s"

        app.MapDelete($"/api/{node}s", async (IDbService db, [FromBody] TDto dto) => //Här skapar vi en metod, Är till för att vi ska kunna ta bort samma sökväg som ovan, 
        {                                                                            //Tar emot två parametrar (IDbService och TDto)
                                                                                     //[FromBody] = Är ett attribut som styr hur datat ska hämtas.
                                                                                     //FromBody betyder att den inte ska hämta från URL, Utan det data som skickas med
                                                                                     //i våran request/anropet(LÄSER JSON DATA SOM SKICKAS MED)
            try
            {                                                                        
                if (!db.Delete<TEntity, TDto>(dto)) return Results.NotFound();//Om deleten misslyckas return NotFound

                if (await db.SaveChangesAsync()) return Results.NoContent(); //Om den lyckas sparas den permanent genom metoden och 
            }                                                                //returnerar NoContent eftersom det finns inget att skicka tillbaka
            catch
            {
            }
            return Results.BadRequest($"Couldn't delete the {typeof(TEntity).Name} entity.");
        });
    }
    public static async Task<IResult> HttpSingleAsync<TEntity, TDto>(this IDbService db, int id) //Här hämtar vi en entitet
    where TEntity : class, IEntity where TDto : class // Här säger vi att entiteten som kommer in måste implementera IEntity så vi kan komma åt Id.
    {   // using of id for each category , it's shows just one category . ex. id = 1 is Men , id=2 Women , id = 3 Kids 
        var result = await db.SingleAsync<TEntity, TDto>(id); //id måste ha samma parameter som rad 16. 
        if (result is null) return Results.NotFound();
        return Results.Ok(result);
    } 
    public static async Task<IResult> HttpGetAsync<TEntity, TDto>(this IDbService db) //hämtar en lista av entiteter från databasen (CategoryDbService)
    where TEntity : class where TDto : class =>
        Results.Ok(await db.GetAsync<TEntity, TDto>());
    public static async Task<IResult> HttpPostAsync<TEntity, TPostDto>(this IDbService db, TPostDto dto) //en asyncron metod HttpPostAsync, vi får in en entitet, och postDTO, 
    where TEntity : class, IEntity where TPostDto : class //Här säger vi att TEntity måste vara en klass och Interface, vår DTO måste vara en klass.
    {
        try
        {
            var entity = await db.AddAsync<TEntity, TPostDto>(dto); //Vi vill anropa AddAsync som måste läggas till i vårat
                                                                    //datalager, vi skickar en förfrågan till Add-metoden 
            if (await db.SaveChangesAsync()) //SaveChangesAsync är utanför och körs efter vi kommit tillbaks från
                                             //AddSync-metoden för att vi vill göra flera saker (add,delete,change)
                                             //innan allt ska sparas. 
            {
                var node = typeof(TEntity).Name.ToLower();  // Här tittar vi på TEntity och kontrollerar namnet(category)
                                                            // så vi får ut namnet på entiteten. 
                return Results.Created($"/{node}s/{entity.Id}", entity); //När vi gör en Created, så vill vi ta ut
                                                                         //namnet på entiteten, dess id och skapa
                                                                         //motsvarande sökväg. Created vill skicka
                                                                         //tillbaks sökvägen och objektet vi skapat
                                                                         //så man vet vart resurserna hamnat i databasen
            }
        }
        catch
        {
        }

        return Results.BadRequest($"Couldn't add the {typeof(TEntity).Name} entity.");
    }
    public static async Task<IResult> HttpPutAsync<TEntity, TPutDto>(this IDbService db, TPutDto dto) //Vår Update-metod, IResult som returnerar om vi lyckats eller misslyckats.
    where TEntity : class, IEntity where TPutDto : class
    {
        try
        {
            db.Update<TEntity, TPutDto>(dto); //Vår Update-metod låter oss göra pm PutDto till entitet i DbService.
            if (await db.SaveChangesAsync()) return Results.NoContent();//NoContent(lyckats men har ingen data att rapportera)
                                                                        //används för att vi inte vill ha något data tillbaka,
                                                                        //eftersom vi redan har datat och behöver inte få den igen.
                                                                        //efter DbService återvänter den och rapporterar om
                                                                        //den lyckats(93) eller misslyckats.(102)
        }
        catch                                                           
        {
        }  
        return Results.BadRequest($"Couldn't update the {typeof(TEntity).Name} entity."); 
                                                                                          
    }
    public static async Task<IResult> HttpDeleteAsync<TEntity>(this IDbService db, int id)
    where TEntity : class, IEntity //här finns ingen Dto vi begränsar, vi begränsar enbart entiteter (id) som kommer in.
    {
        try
        {       //om du är falsk ska du returnera NotFound.
            if (!await db.DeleteAsync<TEntity>(id)) return Results.NotFound(); //Kör DeleteAsync metoden som hämtar en tabell av entiteter med id och sedan tar bort den.
                                                                               //Om den misslyckas och inte hittar entiteten skriver den "NotFound"

            if (await db.SaveChangesAsync()) return Results.NoContent(); //om den lyckas tas den bort och sparas permanent i databasen och returnerar NoContent
        }
        catch
        {
        }

        return Results.BadRequest($"Couldn't delete the {typeof(TEntity).Name} entity.");
    }
    public static async Task<IResult> HttpPostReferenceAsync<TEntity, TPostDto>(this IDbService db, TPostDto dto) //Här vill vi få tillbaka en task av IReuslt och vara asynkron. 
    where TEntity : class where TPostDto : class   //Entiteten och Dto måste vara en klass.                       //Metoden heter HttpPostReferenceAsync, Den ska ta emot TEntity och en DTO,
    {                                                                                                             //IDbservice låter oss komma åt databasen, och dto blir inskickad från ASP.NetCore när vi gör anropet
        try
        {
            var entity = await db.AddAsync<TEntity, TPostDto>(dto); // add extra categories, vi försöker anropa databasservicen och lägga till. 
            if (await db.SaveChangesAsync()) //om den lyckas ska det sparas permanent. 
            {
                var node = typeof(TEntity).Name.ToLower(); //här hämtar vi namnet på vår entitet.
                return Results.Created($"/{node}s/", entity); //anger huvudsökvägen till tabellen.
            }
        }
        catch
        {
        }
        return Results.BadRequest($"Couldn't add the {typeof(TEntity).Name} entity."); //typeof = hämta klassritningen TEntity och fråga vad heter (.Name)
    }
    
}
