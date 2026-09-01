using System.Net;
using PetCare360.IntegrationTests.FactoryFixture;

namespace PetCare360.IntegrationTests.Integration
{
    [Collection("ApiCollection")]
    public class HealthCheckIntegrationTests
    {
        private readonly HttpClient _client;

        public HealthCheckIntegrationTests(ApiFactoryFixture factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task HealthLive_AplicacaoNoAr_RetornaHealthy()
        {
            
            var response = await _client.GetAsync("/health/live");

            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var conteudo = await response.Content.ReadAsStringAsync();
            Assert.Equal("Healthy", conteudo);
        }

        [Fact]
        public async Task HealthReady_BancoDisponivel_RetornaHealthy()
        {
            
            var response = await _client.GetAsync("/health/ready");

            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            
            var conteudo = await response.Content.ReadAsStringAsync();
            Assert.Contains("oracle-database", conteudo);
            Assert.Contains("Healthy", conteudo);
        }
    }
}