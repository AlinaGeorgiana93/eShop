
using eShop.API.DTO.DTOs;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<EShopContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("EShopConnection")));

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsAllAccessPolicy", opt =>
        opt.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod()
    );
});


RegisterServices((builder.Services));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

RegisterEndpoints();


app.UseCors("CorsAllAccessPolicy");

app.Run();

void RegisterEndpoints()
{
    app.AddEndpoint<Product, ProductPostDTO, ProductPutDTO, ProductGetDTO>();
    app.AddEndpoint<Size, SizePostDTO, SizePutDTO, SizeGetDTO>();
    app.AddEndpoint<Color, ColorPostDTO, ColorPutDTO, ColorGetDTO>();

    app.AddEndpoint<ProductCategory, ProductCategoryDTO>();
    app.AddEndpoint<ProductSize, ProductSizeDTO>();
    app.AddEndpoint<ProductColor,  ProductColorDTO>();

    app.MapGet($"/api/productsbycategory/" + "{{categoryId}}", async (IDbService db, int categoryId) => //Vi lägger till en extra nod för att kunna hämta från categoryId. Dom övre sökvägarna AddEndpoint hämtar bara i form av lista, dom tar inte emot id. 
                                        
    {
        try
        {
            var result = await ((ProductDbService)db).GetProductsByCategoryAsync(categoryId);
            return Results.Ok(result);  //TypeCasting, vi tar IDbService och säger jag vill komma åt och göra om så jag kommer åt saker i ProductDbService. Vi castar den. Metoden GetProductsByCategoryAsync hämtar produkter via categoryId. 
        }
        catch
        {
        }

        return Results.BadRequest($"Couldn't get the requested products of type {typeof(Product).Name}.");
    
    });
}
void RegisterServices(IServiceCollection services)
{
    ConfigureAutoMapper();
    services.AddScoped<IDbService, ProductDbService>();
}

void ConfigureAutoMapper()
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<Product, ProductPostDTO>().ReverseMap();
        cfg.CreateMap<Product, ProductPutDTO>().ReverseMap();
        cfg.CreateMap<Product, ProductGetDTO>().ReverseMap();

        cfg.CreateMap<Size, SizePostDTO>().ReverseMap();
        cfg.CreateMap<Size, SizePutDTO>().ReverseMap();
        cfg.CreateMap<Size, SizeGetDTO>().ReverseMap();

        cfg.CreateMap<Color, ColorPostDTO>().ReverseMap();
        cfg.CreateMap<Color, ColorPutDTO>().ReverseMap();
        cfg.CreateMap<Color, ColorGetDTO>().ReverseMap();

        cfg.CreateMap<ProductCategory, ProductCategoryDTO>().ReverseMap();
        cfg.CreateMap<ProductSize, ProductSizeDTO>().ReverseMap();
        cfg.CreateMap<ProductColor, ProductColorDTO>().ReverseMap();
       
    });
    var mapper = config.CreateMapper();
    builder.Services.AddSingleton(mapper);
}