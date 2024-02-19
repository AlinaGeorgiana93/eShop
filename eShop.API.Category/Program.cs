

using eShop.API.DTO.DTOs;

var builder = WebApplication.CreateBuilder(args); //Rad 2-7 startar v�rat API och registrera swagger s� man f�r tillg�ng tillhemsidan att testa.

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQL Server Service Registration --         //Den registrera databasfunktionallitet �t oss s� vi kan komma �t databasen.
builder.Services.AddDbContext<EShopContext>(  //denna tj�nst som vi lagt till anv�nder sig utav ett biblotek
                                              //som har en metod som heter AddDbContext f�r att kunna l�gga
                                              //till informationen som finns i klassen EShopContex,
                                              //vi vill komma �t v�ra Db-sets och v�r konfiguration i
                                              //OnModelCreating(den styr vad som finns tillg�ngligt eller
                                              //som ska skapas i databasen, Rad 9 till 19 �r en SQL-server
                                              //connection m�jligheten till databasen d�r vi anv�nder oss
                                              //av v�r connectionstring f�r att tala om vart v�r databas ligger och dess namn.
    options =>
        options.UseSqlServer( //V�r databasmotor, anv�nder MS.FWC.SQL
            builder.Configuration.GetConnectionString("EShopConnection")));//Vi h�mtar v�r connectionstring fr�n appsettings.json

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsAllAccessPolicy", opt =>
        opt.AllowAnyOrigin() //en policytj�nst som talar om vilka dom�ner/subdom�ner som ska f� tillg�ng att k�ra APIet
           .AllowAnyHeader() 
           .AllowAnyMethod()
    );
});

RegisterServices(); //registrerar automapper och IDBservice s� att den ska arbeta med CategoryDBservive
                    //( inte basklassen som vi �rvt in till CategoryDBservice.)

                           
var app = builder.Build();  //L�gger till servicerna och bygger den l�sningen f�r att
                            //kunna l�gga till pipelines till den f�rdigbyggda l�sningen(s�kerhet,redirection)
                            //Ovanf�r denna rad konfiguerar man egna tj�nster/klasser man beh�ver,
                            //s� det �r som biblotek som vi beh�ver ha tillg�ng till f�r att utf�ra n�got. 
                            //Under den h�r raden g�rs ett anrop f�r att komma �t APIet, n�r vi g�r det kan
                            //man skicka med ett s�kerhetsinformation i form av en token eller annan information.
                            //H�r tar den alla tj�nster och bygger ihop dom s� den
                            //f�rst�r vad den ska ha tillg�ng till och vilka tj�nster som finns. 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) //v�r pipeline d�r vi kan l�gga till funktionallitet,
                                     //saker som vi vill g�ra med v�r request/respons 
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}
                           //S�KERHET SKA VARA F�RE DEN H�R RADEN!
app.UseHttpsRedirection(); //Under denna rad konfiguerar man funktionallitet
                           //som vi vill skjuta in i en pipeline. S�kerhet ska ske ovanf�r den h�r. 
                           //Om vi f�rs�ker komma �t n�tt som vi inte har till�ng till ska den
                           //rapportera tillbaka ett fel i form av "du har inte r�ttigheter och m�ste logga in".
                           //�ven hanterar s� vi kommer till nod i v�rat API.
                           //(L�gga till/ta bort en produkt, l�gga till/l�sa en kategori.)
                           //Den ger m�jligheten att komma till r�tt st�lle beroende p� vad man skriver i URL.

RegisterEndpoints(app); //Via denna metod g�r den in i AddEndPoint som registrerar Category(HttpExtensions)

// Configure CORRS
app.UseCors("CorsAllAccessPolicy"); //H�r anv�nds Cors som kontrollerar vem som har
                                    //r�ttigheter att anropa. H�nvisar till tj�nsten p� rad 22

app.Run(); //H�r startas APIet s� vi kan anropa den 


void RegisterServices() //H�r registreras en AutoMapper-Tj�nst
{
    ConfigureAutoMapper(); //detta �r v�r konfiguration f�r Automapper p� rad 77.
    builder.Services.AddScoped<IDbService, CategoryDbService>(); //Registrerar v�r databas-service CategoryDbService
                                                                 //tillsammans med DbService (CategoryDbService som
                                                                 //�rver fr�n DbService) via interface (IDbService)
                                                                 //som talar om vilka metoder CategoryDbService m�ste inneh�lla.
                                                                 //builder.Services till�ter oss registrera en klass
                                                                 //f�r att f� Objekt automatiskt n�r vi g�r en kontruktor injektion.
}
void RegisterEndpoints (WebApplication app) 
{
    //h�r s�ger vi att n�r vi arbetar med Category, vill jag den ska k�nna till Post,Put,Get-DTO. 
    //Post = har inget Id bara name,  Put = har Id och name,  Get = har Id, name och lista t ex.
    app.AddEndpoint<Category,CategoryPostDTO, CategoryPutDTO, CategoryGetDTO>(); //Vi s�ger att vi vill registrerar
                                                                                 //en Endpoint t ex Category,
                                                                                 //vi g�r ett anrop. Ordningen
                                                                                 //av anropet �r viktig s� det inte
                                                                                 //blir en missmatch i HttpExtensions. 
}
void ConfigureAutoMapper() //v�r konfigueration av AutoMapper d�r vi s�ger att
                           //vi vill kunna konvertera fr�n Category till en PPGS-DTO
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<Category, CategoryPostDTO>().ReverseMap(); //Dessa rader talar om konverteringarna som AutoMapper ska 
        cfg.CreateMap<Category, CategoryPutDTO>().ReverseMap();  //k�nna till, Ska man kunna konvertera fr�n en Category 
        cfg.CreateMap<Category, CategoryGetDTO>().ReverseMap();  //till en PostDTO, PutDTO och tv�rtom
        cfg.CreateMap<Category, CategorySmallGetDTO>().ReverseMap();
        cfg.CreateMap<ProductCategory, ProductCategoryDTO>().ReverseMap();

        //cfg.CreateMap<Filter, FilterGetDTO>().ReverseMap();
        //cfg.CreateMap<Size, OptionDTO>().ReverseMap();
        //cfg.CreateMap<Color, OptionDTO>().ReverseMap();
    });
    var mapper = config.CreateMapper();
    builder.Services.AddSingleton(mapper); //1.vi registrerar den som en Singleton-service
                                           //tj�nst efter samma ska anv�ndas genom hela applikationen.
                                           //2.N�r registreringen �r utf�rd k�nner ASP.NetCore�s runtime
                                           //att denna tj�nst existerar och h�ller det i minnet. S� n�r
                                           //n�n beg�r att f� anv�nda Automapper, ska den ge objektet ovan.

}

