namespace eShop.Data.Services;

public class CategoryDbService(EShopContext db, IMapper mapper) : DbService(db, mapper)
{
    public override async Task<List<TDto>> GetAsync<TEntity, TDto>()
    {
        //IncludeNavigationFor<Filter>();    //tanken är att i en DTO kunde vi ha en lista vi kunde fylla med filter, eller om vi hämtar en categori så vill vi hämta alla produkter med den kategorin. 
        //IncludeNavigationFor<Product>();   // denna metod talar om vilka relaterade entiteter vi vill laddda. Filter och produkter samtidigt t ex. 
        return await base.GetAsync<TEntity, TDto>();
    }
}
