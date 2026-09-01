using System.Net;
using System.Net.Http.Json;
using PetCare360.Domain.Entities;
using PetCare360.IntegrationTests.FactoryFixture;

namespace PetCare360.IntegrationTests.Integration
{
    [Collection("ApiCollection")] 
    public class TutoresControllerIntegrationTests
    {
        private readonly HttpClient _client;

        public TutoresControllerIntegrationTests(ApiFactoryFixture factory)
        {
            
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CriarTutor_DadosValidos_RetornaCreated()
        {
            
            var novoTutor = new
            {
                nmTutor = "Carla Menezes",
                cpf = "321.654.987-11",
                email = "carla.menezes@petcare360.com",
                telefone = "(11) 97777-1122",
                endereco = "Rua das Palmeiras, 45"
            };

            
            var response = await _client.PostAsJsonAsync("/api/Tutores", novoTutor);

            
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var tutorCriado = await response.Content.ReadFromJsonAsync<Tutor>();
            Assert.NotNull(tutorCriado);
            Assert.Equal("Carla Menezes", tutorCriado.NmTutor);
            Assert.True(tutorCriado.IdTutor > 0);
        }

        [Fact]
        public async Task CriarTutor_SemNome_RetornaBadRequest()
        {
            
            var tutorInvalido = new
            {
                cpf = "111.222.333-44",
                email = "sem.nome@petcare360.com"
            };

            
            var response = await _client.PostAsJsonAsync("/api/Tutores", tutorInvalido);

            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CriarTutor_EmailInvalido_RetornaBadRequest()
        {
            
            var tutorInvalido = new
            {
                nmTutor = "Email Errado",
                cpf = "555.666.777-88",
                email = "isso-nao-e-email",
                telefone = "(11) 95555-0000",
                endereco = "Rua Teste, 10"
            };

            
            var response = await _client.PostAsJsonAsync("/api/Tutores", tutorInvalido);

            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task BuscarTutor_IdInexistente_RetornaNotFound()
        {
            
            var response = await _client.GetAsync("/api/Tutores/999999");

            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ListarTutores_AposCadastro_RetornaTutorNaLista()
        {
            
            var novoTutor = new
            {
                nmTutor = "Diego Fontes",
                cpf = "999.888.777-66",
                email = "diego.fontes@petcare360.com",
                telefone = "(11) 96666-3344",
                endereco = "Av. Central, 900"
            };
            await _client.PostAsJsonAsync("/api/Tutores", novoTutor);

            
            var response = await _client.GetAsync("/api/Tutores");

            
            response.EnsureSuccessStatusCode(); // Falha se não for 200-299
            var tutores = await response.Content.ReadFromJsonAsync<List<Tutor>>();
            Assert.NotNull(tutores);
            Assert.Contains(tutores, t => t.Email == "diego.fontes@petcare360.com");
        }
    }
}