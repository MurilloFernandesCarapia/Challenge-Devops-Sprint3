using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PetCare360.Application.Diagnostics;
using PetCare360.Application.Services;
using PetCare360.Domain.Interfaces;
using PetCare360.Infrastructure.Data;
using PetCare360.Infrastructure.Repositories;
using PetCare360.Infrastructure.HealthChecks;

namespace PetCare360.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //1. Banco de dados Oracle da FIAP
            var connectionString = configuration.GetConnectionString("OracleConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseOracle(connectionString,
                    b => b.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)));

            //2. Repositórios - quem conversa com o banco
            services.AddScoped<ITutorRepository, TutorRepository>();
            services.AddScoped<IPetRepository, PetRepository>();
            services.AddScoped<IClinicaRepository, ClinicaRepository>();
            services.AddScoped<IConsultaRepository, ConsultaRepository>();
            services.AddScoped<IVacinaRepository, VacinaRepository>();
            services.AddScoped<IMedicamentoRepository, MedicamentoRepository>();

            //3. Serviços - onde ficam as regras de negócio
            services.AddScoped<ITutorService, TutorService>();
            services.AddScoped<IPetService, PetService>();
            services.AddScoped<IClinicaService, ClinicaService>();
            services.AddScoped<IConsultaService, ConsultaService>();
            services.AddScoped<IVacinaService, VacinaService>();
            services.AddScoped<IMedicamentoService, MedicamentoService>();

            //4. Health Checks de infraestrutura
            services.AddHealthChecks()
                //Liveness: a aplicação está de pé? Não toca em dependência nenhuma.
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })

                //Readiness: o Oracle responde? Se não, a API não deve receber tráfego.
                .AddDbContextCheck<AppDbContext>(
                    name: "oracle-database",
                    tags: new[] { "ready" })

                //Startup: as migrations já foram aplicadas neste banco?
                .AddCheck<MigrationsHealthCheck>(
                    name: "migrations",
                    tags: new[] { "startup" });

            //5. OpenTelemetry - tracing e métricas
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(TelemetryConstants.ServiceName);

            services.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation() // Captura requisições HTTP de entrada
                        .AddHttpClientInstrumentation() // Captura requisições HTTP de saída
                        .AddSource(TelemetryConstants.ServiceName) // Assina os traces dos nossos services
                        .AddConsoleExporter();
                })
                .WithMetrics(meterProviderBuilder =>
                {
                    meterProviderBuilder
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation() // Tempo de resposta e taxa de erro por endpoint
                        .AddHttpClientInstrumentation()
                        .AddMeter(TelemetryConstants.MeterName) // Nossos contadores customizados
                        .AddConsoleExporter();
                });

            return services;
        }
    }
}