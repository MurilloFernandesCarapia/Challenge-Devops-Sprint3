using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;

namespace PetCare360.UnitTests.Fixtures
{
    
    public class TelemetryFixture : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;

        public IMeterFactory MeterFactory { get; }

        public TelemetryFixture()
        {
            var services = new ServiceCollection();
            services.AddMetrics();

            _serviceProvider = services.BuildServiceProvider();
            MeterFactory = _serviceProvider.GetRequiredService<IMeterFactory>();
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }
    }

    
    [CollectionDefinition("ServicesCollection")]
    public class ServicesCollection : ICollectionFixture<TelemetryFixture>
    {
       
    }
}