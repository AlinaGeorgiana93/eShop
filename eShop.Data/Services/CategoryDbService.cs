using AutoMapper;
using eShop.Data.Context;

namespace eShop.Data.Services;

public class CategoryDbService(EShopContext db, IMapper mapper) : DbService(db, mapper) //CategoryDbService är en egen klass som ärver in från
                                                                                        //DbService eftersom vi vill kunna göra speciell funktionallitet för CDbS.
{
    public override async Task<List<TDto>> GetAsync<TEntity, TDto>() //override körs så vi hamnar i denna metod.
    {   
        IncludeNavigationsFor<Filter>();   //Dessa ska inkludera kopplingsdata.(Går från Category-CategoryProducts-Products och hämtar produkterna)
                                           //tanken är att i en DTO kunde vi ha en lista vi kunde fylla med filter, eller om vi hämtar en categori
                                           //så vill vi hämta alla produkter med den kategorin. 

        IncludeNavigationsFor<Product>();// Dessa metod talar om vilka relaterade entiteter vi vill laddda. Filter och produkter samtidigt t ex.
        return await base.GetAsync<TEntity, TDto>(); //base är nyckelord för att anropa funktionallitet i basklassen DbS
    }

    /*public async Task<List<CategoryGetDTO>> GetCategoriesWithAllRelatedDataAsync()
    {
        IncludeNavigationsFor<Filter>();
        IncludeNavigationsFor<Product>();
        var categories = await base.GetAsync<Category, CategoryGetDTO>();

        foreach (var category in categories)
        {
            if (category is null || category.Filters is null) continue; //Om någon av dessa är null, kan vi inte göra något 
                                                                        //eftersom vi inte har någon lista att arbeta med. 
                                                                        //Continue = hoppar över och fortsätter med nästa från listan.
                                                                        //Hade det varit Break, hade den hoppat ut ur hela loopen.

            foreach (var filter in category.Filters)
            {
                filter.Options = []; //hämtar filter och säger att vi vill ha en tom array.

                var dbSetProperty = db.GetType().GetProperties()
                    .FirstOrDefault(p => p.Name.Equals(filter.Name, StringComparison.OrdinalIgnoreCase) &&
                        p.PropertyType.IsGenericType &&
                        p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

                if (dbSetProperty is null) continue;

                // Retrieve the DbSet and cast it to IQueryable<object>
                var dbSet = (IQueryable<object>?)dbSetProperty.GetValue(db);

                if (dbSet is null) continue;

                var data = await dbSet.ToListAsync();

                filter.Options = _mapper.Map<List<OptionDTO>>(data);
            }
        }

        return categories;
    }
    */
    
}



