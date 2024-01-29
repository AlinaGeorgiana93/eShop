
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
        options.UseSqlServer( //anv�nder MS.FWC.SQL
            builder.Configuration.GetConnectionString("EShopConnection")));

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsAllAccessPolicy", opt =>
        opt.AllowAnyOrigin() //en policytj�nst som talar om vilka dom�ner/subdom�ner som ska f� tillg�ng att k�ra APIet
           .AllowAnyHeader() 
           .AllowAnyMethod()
    );
});

RegisterServices(); //registrerar automapper och IDBservice s� att den ska arbeta med CategoryDBservive ( inte basklassen som vi �rvt in till CategoryDBservice.)

                           
var app = builder.Build();  //v�r pipeline, l�gger till servicerna och bygger den l�sningen f�r att kunna l�gga till pipelines till den f�rdigbyggda l�sningen(s�kerhet,redirection)
                            //ovanf�r denna rad konfiguerar man egna tj�nster/klasser man beh�ver,
                            //s� det �r som biblotek som vi beh�ver ha tillg�ng till f�r att utf�ra n�got. 
                            //Under den h�r raden g�rs ett anrop f�r att komma �t APIet,
                            //n�r vi g�r det kan man skicka med ett s�kerhetsinformation i form av en token eller annan information.
                            //H�r startas applikationen upp s� tj�nsterna kan anv�ndas.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) //v�r pipeline d�r vi kan l�gga till funktionallitet, saker som vi vill g�ra med v�r request/respons 
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

RegisterEndpoints(app);

// Configure CORRS
app.UseCors("CorsAllAccessPolicy"); //H�r anv�nds Cors som kontrollerar vem som har r�ttigheter att anropa. H�nvisar till tj�nsten p� rad 22

app.Run();

app.UseCors("Policy");
void RegisterServices()
{
    ConfigureAutoMapper();
    builder.Services.AddScoped<IDbService, CategoryDbService>();
    //builder.Services till�ter oss registrera en klass f�r att f� Objekt automatiskt n�r vi g�r en kontruktor injektion.
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

