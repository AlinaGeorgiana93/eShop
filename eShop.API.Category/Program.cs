using eShop.Data.Context;
using Microsoft.EntityFrameworkCore;
using eShop.Data;
using eShop.Data.Entities;
using Microsoft.AspNetCore;
using Microsoft.Extensions;
using Microsoft.AspNetCore.Builder;
using eShop.API.Extensions;
using eShop.API.DTO;
using eShop.Data.Services;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EShopContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("EShopConnection")));

RegisterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


RegisterEndpoints(app);
app.Run();
app.UseCors("Policy");

void RegisterServices()
{
    ConfigureAutoMapper();
    builder.Services.AddScoped<IDbService, CategoryDbService>();
}
void RegisterEndpoints (WebApplication app)
{
    app.AddEndpoint<Category,CategoryPostDTO, CategoryPutDTO, CategoryGetDTO>();
    
}
void ConfigureAutoMapper()
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<Category, CategoryPostDTO>().ReverseMap();
        cfg.CreateMap<Category, CategoryPutDTO>().ReverseMap();
        cfg.CreateMap<Category, CategoryGetDTO>().ReverseMap();
        cfg.CreateMap<Category, CategorySmallGetDTO>().ReverseMap();
        //cfg.CreateMap<Filter, FilterGetDTO>().ReverseMap();
        //cfg.CreateMap<Size, OptionDTO>().ReverseMap();
        //cfg.CreateMap<Color, OptionDTO>().ReverseMap();
    });
    var mapper = config.CreateMapper();
    builder.Services.AddSingleton(mapper);
}

