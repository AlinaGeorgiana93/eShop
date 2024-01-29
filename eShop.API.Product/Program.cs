using eShop.Data.Context;
using eShop.Data.Services;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using eShop.Data.Entities;
using eShop.API.DTO.DTOs;
using eShop.API.Extensions.Extensions;


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


app.UseCors("Policy");
app.Run();
void RegisterServices(IServiceCollection services)
{
    ConfigureAutoMapper();
    services.AddScoped<IDbService, ProductDbService>();
}
void RegisterEndpoints()
{
    app.AddEndpoint<Product, ProductPostDTO, ProductPutDTO, ProductGetDTO>();
    app.AddEndpoint<Size, SizePostDTO, SizePutDTO, SizeGetDTO>();
    app.AddEndpoint<Color, ColorPostDTO, ColorPutDTO, ColorGetDTO>();

    app.AddEndpoint<ProductCategory, ProductCategoryPostDTO>();
    app.AddEndpoint<ProductSize, ProductSizePostDTO, ProductSizeGetDTO>();
    app.AddEndpoint<ProductColor,  ProductColorPostDTO, ProductColorGetDTO>();

    /*app.MapGet($"/api/productsbycategory/" + "{categoryId}", async (IDbService db, int categoryId) =>

    {
        try
        {
            return Results.Ok(await ((ProductDbService)db).GetProductsByCategoryAsync(categoryId));
        }
        catch
        {
        }

        return Results.BadRequest($"Couldn't get the requested products of type {typeof(Product).Name}.");
    
    });
    */
    
   
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
        cfg.CreateMap<ProductCategory, ProductCategoryPostDTO>().ReverseMap();
        cfg.CreateMap<ProductCategory, ProductCategoryDeleteDTO>().ReverseMap();
        cfg.CreateMap<ProductSize, ProductSizePostDTO>().ReverseMap();
        cfg.CreateMap<ProductSize, ProductSizeDeleteDTO>().ReverseMap();
        cfg.CreateMap<ProductColor, ProductColorPostDTO>().ReverseMap();
        cfg.CreateMap<ProductColor, ProductColorDeleteDTO>().ReverseMap();
       
    });
    var mapper = config.CreateMapper();
    builder.Services.AddSingleton(mapper);
}