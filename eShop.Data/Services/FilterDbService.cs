using AutoMapper;
using eShop.Data.Context;
using eShop.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace eShop.Data.Services
{
    public class FilterDbService(EShopContext db, IMapper mapper) : DbService(db, mapper)
    {
        public override async Task<List<TDto>> GetAsync<TEntity, TDto>()
        {
           // IncludeNavigationFor<Filter>();
          
            return await base.GetAsync<TEntity, TDto>();
        }
    }
}
