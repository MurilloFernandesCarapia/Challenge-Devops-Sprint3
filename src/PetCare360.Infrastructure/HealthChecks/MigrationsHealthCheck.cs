using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PetCare360.Infrastructure.Data;

namespace PetCare360.Infrastructure.HealthChecks
{
    
    public class MigrationsHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _dbContext;

        public MigrationsHealthCheck(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var pendentes = await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken);

                if (pendentes.Any())
                {
                    return HealthCheckResult.Unhealthy(
                        $"Existem {pendentes.Count()} migrations pendentes. Rode o dotnet ef database update.");
                }

                return HealthCheckResult.Healthy("Banco de dados atualizado.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Não foi possível verificar as migrations.", ex);
            }
        }
    }
}