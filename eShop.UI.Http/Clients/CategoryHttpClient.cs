
using eShop.API.DTO;

using System.Text.Json;

namespace eShop.UI.Http.Clients;

public class CategoryHttpClient // Vi skapar denna för att komma åt sökvägarna via vår applikation istället för att använda swagger.
                                //  // Vi skapar den för att kunna ha specifika inställningar för just Category APIet.
{
    private readonly HttpClient _httpClient;
    string _baseAddress = "https://localhost:5000/api/"; // vår adress/url för att nå APIet

    public CategoryHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient; 
        _httpClient.BaseAddress = new Uri($"{_baseAddress}categorys"); // Säger att "Du ska ha en specifik basadress(BaseAddress)
    }


    public async Task<List<CategoryGetDTO>> GetCategoriesAsync() //dess uppgift är att anropa APIets Get-metod, hämta data som returneras och visas i en lista. 
    {
        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(""); //här skapar vi ett anrop/request till vårat API. vi använder _httpClient, och metoden GetAsync som anropar APIet asyncront. Som skickar tillbaka response. Using används så den stängs efter den är färdig.
            response.EnsureSuccessStatusCode(); //en kontroll som säger "fortsätt bara med koden under, om du har fått ett OK-meddelande, annars hoppa ner till catch"

            var result = JsonSerializer.Deserialize<List<CategoryGetDTO>>(await response.Content.ReadAsStreamAsync(), //JsonSerializer är en klass som tar json-data och konvertera om det till DTO-listor. Deserialize gör om datat till objekt. response.Content.ReadAsStreamAsync() = hoppa in i datat och läs det som en ström (INTE STRÄNG) så den inte behöver hämta allt på en gång.
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); //json har camelcasing, liten bokstav på fösta ordet, stor på resterabde orden. Men eftersom vi använder Pascalcasing på våra propertys. PropertyNameCaseInsensitive = tar hänsyn till stora och små bokstäver när den matchar propertys, när den hämtar datat och konvertera json till c#. 

            return result ?? []; //?? säger att om du är null ska du returnera en tom lista, annars returnera resultatet.
        }
        catch (Exception ex)
        {
            return [];
        }
    }
}