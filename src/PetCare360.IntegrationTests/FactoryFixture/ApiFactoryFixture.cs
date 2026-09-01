using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetCare360.Infrastructure.Data;

namespace PetCare360.IntegrationTests.FactoryFixture
{
    
    public class ApiFactoryFixture : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:OracleConnection"] =
                        "User Id=teste;Password=teste;Data Source=localhost:1521/XEPDB1;"
                });
            });

            builder.ConfigureServices(services =>
            {
                
                var descritores = services
                    .Where(d => d.ServiceType.FullName != null &&
                                (d.ServiceType.FullName.Contains("DbContextOptions") ||
                                 d.ServiceType == typeof(AppDbContext)))
                    .ToList();

                foreach (var descritor in descritores)
                {
                    services.Remove(descritor);
                }

                
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("PetCare360TestDb"));
            });
        }
    }

    
    [CollectionDefinition("ApiCollection")]
    public class ApiCollection : ICollectionFixture<ApiFactoryFixture>
    {
        
    }
}