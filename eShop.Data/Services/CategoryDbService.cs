using AutoMapper;
using eShop.Data.Context;

namespace eShop.Data.Services;

public class CategoryDbService(EShopContext db, IMapper mapper) : DbService(db, mapper)
{
    public override async Task<List<TDto>> GetAsync<TEntity, TDto>()
    {
        //IncludeNavigationFor<Filter>();
        //IncludeNavigationFor<Product>();
        return await base.GetAsync<TEntity, TDto>();
    }
}
