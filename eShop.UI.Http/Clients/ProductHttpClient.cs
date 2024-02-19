
using eShop.API.DTO.DTOs;
using System.Text.Json;

namespace eShop.UI.Http.Clients;

public class ProductHttpClient // Vi skapar denna för att komma åt sökvägarna via vår applikation istället för att använda swagger.
                                //  // Vi skapar den för att kunna ha specifika inställningar för just Category APIet.
{
    private readonly HttpClient _httpClient;
    string _baseAddress = "https://localhost:5500/api/"; // vår adress/url för att nå APIet

    public ProductHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient; 
        _httpClient.BaseAddress = new Uri($"{_baseAddress}products"); // Säger att "Du ska ha en specifik basadress(BaseAddress)
    }

    public async Task<List<ProductGetDTO>> GetProductsAsync(int categoryId)
    {
        try
        {
            // Use the relative path, not the base address here
            string relativePath = $"productsbycategory/{categoryId}"; //productsbycategory är den nya sökvägen vi lägger till i ProductAPI för att kunna hämta med categoryId, vilket vi annars inte hade kunnat göra
            using HttpResponseMessage response = await _httpClient.GetAsync(relativePath); // skapar ett respons objekt där vi får tillbaka svaret från APIet, att den har lyckats koppla upp sig. Vi kör await med GetAsync där vi skickar in den relativa sökvägen. 
            response.EnsureSuccessStatusCode(); // om den inte lyckas koppla upp sig när vi hämtar datat, hamnar vi en catch som returnerar en tom lista så vi slipper hantera fel. 

            var resultStream = await response.Content.ReadAsStreamAsync(); //här hämtar vi med en stream. 
            var result = await JsonSerializer.DeserializeAsync<List<ProductGetDTO>>(resultStream, // vi använder JsonSerializer för att deseriliserar json data till DTOer
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); //Måste matcha json data med DTO ananrs kan det bli fel. 

            return result ?? [];
        }
        catch (Exception ex)
        {
            return [];
        }
    }
}