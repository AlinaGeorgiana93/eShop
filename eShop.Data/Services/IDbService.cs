namespace eShop.Data.Services;

public interface IDbService
{
    
Task<TEntity> AddAsync<TEntity, TDto>(TDto dto) //En Add kan det ta tid att skapa resursen i minnet, därför Asyncron
            where TEntity : class
            where TDto : class;

bool Delete<TEntity, TDto>(TDto dto) ////Denna Delete metod fungerar för våra kopplingstabbeller
            where TEntity : class
            where TDto : class;

Task<bool> DeleteAsync<TEntity>(int id)//Denna metod skickar in ett id och entitet 
            where TEntity : class, IEntity; //Denna Delete metod fungerar för våra huvudentiteter.
                                            //Entiteten måste vara en class och innehålla IEntity
Task<List<TDto>> GetAsync<TEntity, TDto>()
            where TEntity : class
            where TDto : class;

void IncludeNavigationsFor<TEntity>() 
            where TEntity : class;

Task<TDto> SingleAsync<TEntity, TDto>(int id)
            where TEntity : class, IEntity
            where TDto : class;

void Update<TEntity, TDto>(TDto dto) //Void är Synkront eftersom objektet finns redan i minnet 
            where TEntity : class, IEntity   //och behöver ingen tid att ändra ett objekt. Om den hade 
            where TDto : class;              //varit Asyncron hade det tagit längre tid och därför har en negativ effekt.

Task<bool> SaveChangesAsync();
}

    

