using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PetCare360.API.Middleware;
using PetCare360.Infrastructure;
using Serilog;
using Serilog.Events;
using PetCare360.Infrastructure.Data;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File("logs/petcare360-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando a PetCare 360 API...");

    var builder = WebApplication.CreateBuilder(args);

    
    builder.Host.UseSerilog();

    //Injeta banco, repositórios, serviços, health checks e OpenTelemetry
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler =
                System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });

    var app = builder.Build();

    
    await DatabaseInitializer.InicializarAsync(app.Services);

    
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetCare360 API v1");
        c.RoutePrefix = "swagger";
    });

    
    app.UseMiddleware<CorrelationIdMiddleware>();

    //Registra uma linha de log por requisição HTTP, com rota, status e duração
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }
    app.UseAuthorization();
    app.MapControllers();

    //Liveness: a aplicação está viva? Responde 200 na hora, sem tocar no banco.
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live")
    });

    //Readiness: a aplicação está pronta para receber tráfego? Depende do Oracle.
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse 
    });

    //Startup: a aplicação terminou de inicializar? Verifica as migrations.
    app.MapHealthChecks("/health/startup", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("startup"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    //Visão geral: roda todos os checks de uma vez
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapGet("/", () => Results.Redirect("/swagger"));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao iniciar.");
}
finally
{
    Log.CloseAndFlush();
}


public partial class Program { }