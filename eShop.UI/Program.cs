using eShop.API.DTO;
using eShop.UI;
using eShop.UI.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<UIService>(); //AddSingleton �r en unik. Vi kommer f� ett enda objekt som anv�nds i hela applikationen. Lever s� l�nge applikationen �r startad. 
builder.Services.AddHttpClient<CategoryHttpClient>(); // h�r anv�nder typen AddHttpClient f�r att registrera CategoryHttpClient som en AddHttpClient ist�llet f�r en scoped.

builder.Services.AddHttpClient<ProductHttpClient>();
ConfigureAutoMapper();

await builder.Build().RunAsync();

void ConfigureAutoMapper() // H�r vill vi mappa emallan CategoryGetDTO och LinkOption.
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<CategoryGetDTO, LinkOption>().ReverseMap();
    });
    var mapper = config.CreateMapper();
    builder.Services.AddSingleton(mapper);
}