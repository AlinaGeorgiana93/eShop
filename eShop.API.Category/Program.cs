
var builder = WebApplication.CreateBuilder(args); //Rad 2-7 startar vårat API och registrera swagger så man får tillgång tillhemsidan att testa.

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQL Server Service Registration --         //Den registrera databasfunktionallitet åt oss så vi kan komma åt databasen.
builder.Services.AddDbContext<EShopContext>(  //denna tjänst som vi lagt till använder sig utav ett biblotek
                                              //som har en metod som heter AddDbContext för att kunna lägga
                                              //till informationen som finns i klassen EShopContex,
                                              //vi vill komma åt våra Db-sets och vår konfiguration i
                                              //OnModelCreating(den styr vad som finns tillgängligt eller
                                              //som ska skapas i databasen, Rad 9 till 19 är en SQL-server
                                              //connection möjligheten till databasen där vi använder oss
                                              //av vår connectionstring för att tala om vart vår databas ligger och dess namn.
    options =>
        options.UseSqlServer( //använder MS.FWC.SQL
            builder.Configuration.GetConnectionString("EShopConnection")));

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsAllAccessPolicy", opt =>
        opt.AllowAnyOrigin() //en policytjänst som talar om vilka domäner/subdomäner som ska få tillgång att köra APIet
           .AllowAnyHeader() 
           .AllowAnyMethod()
    );
});

RegisterServices(); //registrerar automapper och IDBservice så att den ska arbeta med CategoryDBservive ( inte basklassen som vi ärvt in till CategoryDBservice.)

                           
var app = builder.Build();  //vår pipeline, lägger till servicerna och bygger den lösningen för att kunna lägga till pipelines till den färdigbyggda lösningen(säkerhet,redirection)
                            //ovanför denna rad konfiguerar man egna tjänster/klasser man behöver,
                            //så det är som biblotek som vi behöver ha tillgång till för att utföra något. 
                            //Under den här raden görs ett anrop för att komma åt APIet,
                            //när vi gör det kan man skicka med ett säkerhetsinformation i form av en token eller annan information.
                            //Här startas applikationen upp så tjänsterna kan användas.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) //vår pipeline där vi kan lägga till funktionallitet, saker som vi vill göra med vår request/respons 
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}
                           //SÄKERHET SKA VARA FÖRE DEN HÄR RADEN!
app.UseHttpsRedirection(); //Under denna rad konfiguerar man funktionallitet
                           //som vi vill skjuta in i en pipeline. Säkerhet ska ske ovanför den här. 
                           //Om vi försöker komma åt nått som vi inte har tillång till ska den
                           //rapportera tillbaka ett fel i form av "du har inte rättigheter och måste logga in".
                           //Även hanterar så vi kommer till nod i vårat API.
                           //(Lägga till/ta bort en produkt, lägga till/läsa en kategori.)
                           //Den ger möjligheten att komma till rätt ställe beroende på vad man skriver i URL.

RegisterEndpoints(app);

// Configure CORRS
app.UseCors("CorsAllAccessPolicy"); //Här används Cors som kontrollerar vem som har rättigheter att anropa. Hänvisar till tjänsten på rad 22

app.Run();

app.UseCors("Policy");
void RegisterServices()
{
    ConfigureAutoMapper();
    builder.Services.AddScoped<IDbService, CategoryDbService>();
    //builder.Services tillåter oss registrera en klass för att få Objekt automatiskt när vi gör en kontruktor injektion.
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

