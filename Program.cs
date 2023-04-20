using NLog;
using NLog.Web;

//punkt b.2: Indlæs NLog.config-konfigurationsfilen
//punkt b.4 Tilføj fejl håndtering til Program.cs ved at pakke al koden ind i en try/catch/finally
try
{

    // Indlæs NLog.config-konfigurationsfilen
    var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    logger.Debug("init main");
    logger.Info("jaja");
    // Resten af koden her

}

catch (Exception ex)
{
    var logger = NLog.LogManager.GetCurrentClassLogger();
    logger.Error(ex, "Stopped program because of exception");
    throw;
}

//punkt b.4: I finally blokken kan du frigive eventuelle ressourcer og afslutte loggeren, så den ikke forbliver aktiv efter programmet er afsluttet.
finally
{
    NLog.LogManager.Shutdown();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
