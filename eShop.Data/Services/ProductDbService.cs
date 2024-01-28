using AutoMapper;
using eShop.Data.Context;
using eShop.Data.Entities;
namespace eShop.Data.Services;

public class ProductDbService(EShopContext db, IMapper mapper) : DbService(db, mapper)
{
    public override async Task<List<TDto>> GetAsync<TEntity, TDto>()
    {
       //IncludeNavigationFor<Category>();
        //IncludeNavigationFor<Color>();
        //IncludeNavigationFor<Size>();
        return await base.GetAsync<TEntity, TDto>();
    }
}
