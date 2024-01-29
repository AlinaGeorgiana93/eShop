
namespace eShop.Data.Services;

public class ProductDbService(EShopContext db, IMapper mapper) : DbService(db, mapper)
{
    public override async Task<List<TDto>> GetAsync<TEntity, TDto>()
    {
        IncludeNavigationsFor<Category>();
        IncludeNavigationsFor<Color>();
        IncludeNavigationsFor<Size>();
        return await base.GetAsync<TEntity, TDto>();
    }
}
