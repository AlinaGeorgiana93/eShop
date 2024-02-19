using eShop.API.DTO;
using eShop.UI;
using eShop.UI.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<UIService>(); //AddSingleton är en unik. Vi kommer få ett enda objekt som används i hela applikationen. Lever så länge applikationen är startad. 
builder.Services.AddHttpClient<CategoryHttpClient>(); // här använder typen AddHttpClient för att registrera CategoryHttpClient som en AddHttpClient istället för en scoped.

builder.Services.AddHttpClient<ProductHttpClient>();
ConfigureAutoMapper();

await builder.Build().RunAsync();

void ConfigureAutoMapper() // Här vill vi mappa emallan CategoryGetDTO och LinkOption.
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<CategoryGetDTO, LinkOption>().ReverseMap();
    });
    var mapper = config.CreateMapper();
    builder.Services.AddSingleton(mapper);
}