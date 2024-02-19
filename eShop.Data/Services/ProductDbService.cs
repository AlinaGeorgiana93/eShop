using eShop.Data.Shared;
using eShop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using eShop.API.DTO.DTOs;


namespace eShop.Data.Services;

public class ProductDbService(EShopContext db, IMapper mapper) : DbService(db, mapper)
{
    public async Task<List<ProductGetDTO>> GetProductsByCategoryAsync(int categoryId)
    {
        IncludeNavigationsFor<Color>(); //inkluderar färger och storlekar från tabellerna
        IncludeNavigationsFor<Size>();
        var productIds = GetAsync<ProductCategory>(pc => pc.CategoryId.Equals(categoryId)) //här säger vi "hämta kategorierna med kategoriId. men hämta inte datat än. 
            .Select(pc => pc.ProductId); //Select är en link metod som specificerar vilken data vi vill få ut ur resultatet ovan, ProductId. 
        var products = await GetAsync<Product>(p => productIds.Contains(p.Id)).ToListAsync(); //hämtar ut datat från produkter, alla produkter som har productId. Contains jämför id för varje produkt med productIds. (tex alla barnprodukter)
        return MapList<Product, ProductGetDTO>(products); //vi mappar vi och gör om dom till DTOer via automapper
    }
    public new List<TDto> MapList<TEntity, TDto>(List<TEntity> entities)
    where TEntity : class
    where TDto : class
    {
        return mapper.Map<List<TDto>>(entities); //säger att jag vill få ut produktDTOer från mina produkter. 
    }
}
