using eShop.Data.Context;
using eShop.Data.Services;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using eShop.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EShopContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("EShopConnection")));

RegisterServices((builder.Services));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();




app.Run();
app.UseCors("Policy");
void RegisterServices(IServiceCollection services)
{
    ConfigureAutoMapper();
    services.AddScoped<IDbService, ProductDbService>();
}

void ConfigureAutoMapper()
{
    var config = new MapperConfiguration(cfg =>
    {
       // cfg.CreateMap<Category, CategoryPostDTO>().ReverseMap();
       // cfg.CreateMap<Category, CategoryPutDTO>().ReverseMap();
       // cfg.CreateMap<Category, CategoryGetDTO>().ReverseMap();
       //cfg.CreateMap<Category, CategorySmallGetDTO>().ReverseMap();
       //cfg.CreateMap<ProductCategory, ProductCategoryDTO>().ReverseMap();

        //cfg.CreateMap<Filter, FilterGetDTO>().ReverseMap();
        //cfg.CreateMap<Size, OptionDTO>().ReverseMap();
        //cfg.CreateMap<Color, OptionDTO>().ReverseMap();
    });
    var mapper = config.CreateMapper();
    builder.Services.AddSingleton(mapper);
}