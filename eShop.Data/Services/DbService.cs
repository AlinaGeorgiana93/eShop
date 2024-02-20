using AutoMapper;
using eShop.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace eShop.Data.Services;

public class DbService : IDbService //den här klassen ska implementera interfacet.
                                    //De andra klasserna ärver från DbService och implementerar interfacet via arvet.
                                    //DbSercive är basklassen vi skapat med grundfunktionallitet för t ex "GetAsync".
{
    private readonly EShopContext _db;
    public readonly IMapper _mapper;

    public DbService(EShopContext db, IMapper mapper) //Hoppar in här från CDbS och kör metoden nedan
    {
        _db = db;
        _mapper = mapper; //a.här sparar vi undan mapper i variabel _mapper som är tillgänglig i hela klassen...
    }

    public virtual async Task<List<TDto>> GetAsync<TEntity, TDto>()
        where TEntity : class
        where TDto : class
    {
        //IncludeNavigationsFor<TEntity>(); //Den kommer gå in och titta vad TEntity innehåller för navigeringsproperties och plocka ut de som finns, navigeringsproperties är listor tex i Dto (List<CategorySmallGetDTO>)

        var entities = await _db.Set<TEntity>().ToListAsync(); //här hämtar vi listan med entiteter med hjälp av EnFrWo.
                                                               //Den kollar våra kopplingar i OnModelCreating(Data.Context)
                                                               //När den kört färdigt får vi en lista med Entiteter

        return _mapper.Map<List<TDto>>(entities); //När vi fått entiteterna vill vi konverterar dom till en lista DTOer vi kan skicka tillbaka till anroparen
                                                  // _mapper = är automapper. 
                                                  //"(entities)" = är vad vi vill konvertera från.
                                                  //"<List<TDto>>" = är vad vi vill konvertera till, en lista av DTOer
    }
    public IQueryable<TEntity> GetAsync<TEntity>( //En GetAsync metod som tar emot en entitet, och returnerar ut en entitet tillbaks. istället för DTO. IQueryable = det som vi hämtar ut är inte datat. Utan möjligheten att fortsätta bygga vidare på uttrycket som redan finns. Istället för att använda list där man hämtar datat direkt och gör något. 
        Expression<Func<TEntity, bool>> expression) // Expression är ett sätt för oss att skicka in/ta emot LANDA uttryck med Id. Måste vara en funktion(Func), eftersom LANDA uttryck är en funktion. 
        where TEntity : class
    {
        return _db.Set<TEntity>().Where(expression); //Hämta ifrån databasen i den entitet vi angivit. DÄr matchingen är det LANDA uttryck som kommer in(expression). På så sätt får vi ut de som matchar LANDA uttryck. (Deligate)
    }

    public virtual async Task<TDto> SingleAsync<TEntity, TDto>(int id)
        where TEntity : class, IEntity where TDto : class
    {
        var entity = await _db.Set<TEntity>().SingleOrDefaultAsync(e => e.Id == id);
        return _mapper.Map<TDto>(entity); //a...så vi kan anropa Map-metoden.
    }

    public List<TDto> MapList<TEntity, TDto>(List<TEntity> entities) //denna metod mappar entiteter med DTOer, en generisk metod
        where TEntity : class
        where TDto : class
    {
        return _mapper.Map<List<TDto>>(entities); 
    }
    public async Task<TEntity> AddAsync<TEntity, TDto>(TDto dto) //AddSync den ska ha en Entitet och DTO, och den tar emot
                                                                 //en DTO som kommer från användargränssnittet(t ex Swagger)
        where TEntity : class where TDto : class //Här säger vi att TEntity och DTO ska vara klasser,
                                                 //QUIZ - Eftersom kopplingstabbeller(entitetsklasser) inte har IEntity
                                                 //använder vi class istället för IEntity. med IEntity kan vi inte använda
                                                 //denna Add-metoden till att lägga till kopplingstabellsdata
                                                 //class tillåter oss skicka in vilken entitet vi vill Oavsett IEntity
                                                 //IEntity är bara i de tillfällen vi behöver komma åt ID.(SingleAsync,Delete)
    {
        var entity = _mapper.Map<TEntity>(dto);    //En dto kommer in och konverteras till entitet hjälp av AutoMapper
        await _db.Set<TEntity>().AddAsync(entity); //Här använder vi databasens funktionallitet via EntityFrameWork och säga
        return entity;                             //"jag vill använda mig utav den här entiteten(tabellen). Set-metoden
                                                   //måste användas eftersom den är generisk.
                                                   //Sen Anropas AddAsync-metoden som lägger till den i EntityFrameWorks
                                                   //minnesrepresentation av databasen. Den sparas inte i den fysiska SQL-servern.
                                                   //utan den ligger fortfarande i minnet. När den väl kör SaveChangesAsync sparas
                                                   //den permanent i databasen.
    }
    public void Update<TEntity, TDto>(TDto dto) //Tar emot entiteten och DTO
        where TEntity : class, IEntity where TDto : class //begränsning där vi säger TEntity måste vara en klass
                                                          //och måste innehålla IEntity. Samma sak gällande DTOn
    {
        // Note that this method isn't asynchronous because Update modifies
        // an already exisiting object in memory, which is very fast.
        var entity = _mapper.Map<TEntity>(dto); //Här konverterar vi en dto till en entitet.
        _db.Set<TEntity>().Update(entity); //Här uppdaterar vi entiteten som vi konverterat. 
    }
    public async Task<bool> DeleteAsync<TEntity>(int id) //När vi kommer in i denna tjänst får vi ett id. 
        where TEntity : class, IEntity //Entiteten måste vara en klass och innehålla IEntity
    {
        try
        {
            var entity = await _db.Set<TEntity>().SingleOrDefaultAsync(e => e.Id == id); //SingleOrDefaultAsync hämtar 
            if (entity is null) return false; //om den är null returnerar vi false från metoden
            _db.Remove(entity); //Annars tar vi bort från databasen
        }
        catch { return false; } //Skulle något gå fel returnerar den false

        return true; //Körs om det tas bort något från databasen.
    }
    public async Task<bool> SaveChangesAsync() => await _db.SaveChangesAsync() >= 0; //Den här koden kontrollerar om du är noll
                                                                                     //eller större har den lyckats göra det den ska utföra.
    public void IncludeNavigationsFor<TEntity>() //IncludeNavigationsFor tar entitet som vi vill koppla till en annan entitet. Vi talar om vilken entitet som vi vill hämta som relaterat data till våra produkter. talar om vilken extra data vi vill hämta för våra kopplingstabeller som vi lägger till i vår ProductGetDTO(API.DTO). Tar Color och Size Property fylls genom IncludeNavigationsFor. 
        where TEntity : class                    //Via Reflection som Innebär att vi kan ta en generisk datatyp och titta vad den innehåller, så när vi skickar in color tex får vi in den som en entitet/klass. Den är som en ritning som vi kan gå in och titta på, men eftersom den är generisk måste vi läsa för att ta reda på vad den innehåller. Reflection tillåter oss att gå in och läsa va den har för propertys 
    {
        // Skip Navigation Properties are used for many-to-many = Skip Navigation Pr är en koppling där vi har en kopplingstabell mellan tex Category o Product.
        // relationsips (List or ICollection) and Navigation Properties
        // are used for one-to-many relationsips.

        var propertyNames = _db.Model.FindEntityType(typeof(TEntity))?.GetNavigations().Select(e => e.Name);                //FindEntityType anropar vi på databasservicen för att hitta       //här tar vi reda på vilka propertys finns det //GetNavigations betyder hämta navigerinspropertys som är ett till många.
                                                                                                                            //Entiteten i modellen (Där vi har alla våra DBsets så den vet
                                                                                                                            //hur den ska kommunicera med databasen). Model = hämta ritningen.
                                                                                                                            //? = om den inte är null ska vi gå vidare och anropa GetNavigations
                                                                                                                            //som hämtar navigeringsproperties där vi kan gå direkt mellan två Entiteter (One-To-Many)
                                                                                                                            //Select = låter oss hämta en delmängd av properties (Name hämtar namnet på navigeringsproperty)

        var navigationPropertyNames = _db.Model.FindEntityType(typeof(TEntity))?.GetSkipNavigations().Select(e => e.Name); //Här vill vi hämta våra navigeringspropertynamn för SkipNavigation,      //navigationPropertyNames betyder dom som är många till många.      //GetSkipNavigations betyder att det finns en kopplingstabell emellan, den skippar över en för att hämta datat.
                                                                                                                           //Vi vill hämta listorna som har kopplingstabeller.
                                                                                                                           //Här har vi enbart hämtat ut namnen på våra kopplingsproperties. Nästa steg är att använda namnen för att ladda datat 
        if (propertyNames is not null) //Om du inte är null kör vi en foreach och hämtar alla en-till-många-kopplingar
            foreach (var name in propertyNames)
                _db.Set<TEntity>().Include(name).Load(); //Vi säger att Db.Set hämtar entiteten (Data.Context)  // här säger vi "använd dig utav entitetstypen som kommit in och inkludera dom som finns i name
                                                         //Include (extensionmetod) talar om namnet på den property vars värden vi vill ladda.
                                                         //"När du laddar produkten, ska du även ladda datat för (name) Color.

        if (navigationPropertyNames is not null) //samma fast för Skip Navigation Properties
            foreach (var name in navigationPropertyNames)
                _db.Set<TEntity>().Include(name).Load(); //samma som loopen ovan fast för många till många.

        //Reflection (typeof(TEntity) = när vi går in och läser i en typ/hämtar ritningen av en typ,
        //ser vad klassen innehåller för properties metoder och konstruktorer, Med andra ord hur den är uppbygd.
    }
    public bool Delete<TEntity, TDto>(TDto dto) //bool = sant/falskt. Metoden tar in en TEntity och en DTO
        where TEntity : class where TDto : class //begränsningar, entiteten och DTO ska vara klasser, 
    {
        try
        {
            var entity = _mapper.Map<TEntity>(dto); //Här gör vi om dto till en entitet och mappar den
            if (entity is null) return false;//Kontrollerera Om entiteten är null = return false
            _db.Remove(entity); //Tar bort entieten 
            return true;
        }
        catch
        {
            return false; //om entiteten är null 
        }
    }
    
}