using System.Net;
using System.Net.Http.Json;
using PetCare360.Domain.Entities;
using PetCare360.IntegrationTests.FactoryFixture;

namespace PetCare360.IntegrationTests.Integration
{
    [Collection("ApiCollection")]
    public class PetsControllerIntegrationTests
    {
        private readonly HttpClient _client;

        public PetsControllerIntegrationTests(ApiFactoryFixture factory)
        {
            _client = factory.CreateClient();
        }

        
        private async Task<int> CriarTutorAsync(string email)
        {
            var tutor = new
            {
                nmTutor = "Tutor de Teste",
                cpf = $"000.000.000-{Random.Shared.Next(10, 99)}",
                email,
                telefone = "(11) 90000-0000",
                endereco = "Rua de Teste, 1"
            };

            var response = await _client.PostAsJsonAsync("/api/Tutores", tutor);
            var criado = await response.Content.ReadFromJsonAsync<Tutor>();
            return criado!.IdTutor;
        }

        [Fact]
        public async Task CriarPet_TutorExistente_RetornaCreated()
        {
            
            var idTutor = await CriarTutorAsync("tutor.pet.ok@petcare360.com");
            var novoPet = new
            {
                nmPet = "Amora",
                especie = "Cachorro",
                raca = "Beagle",
                peso = 12.4,
                idTutor
            };

            
            var response = await _client.PostAsJsonAsync("/api/Pets", novoPet);

            
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var petCriado = await response.Content.ReadFromJsonAsync<Pet>();
            Assert.NotNull(petCriado);
            Assert.Equal("Amora", petCriado.NmPet);
            Assert.Equal(idTutor, petCriado.IdTutor);
        }

        [Fact]
        public async Task CriarPet_TutorInexistente_RetornaBadRequest()
        {
            
            var petSemTutor = new
            {
                nmPet = "Fantasma",
                especie = "Gato",
                raca = "Siamês",
                idTutor = 999999
            };

            
            var response = await _client.PostAsJsonAsync("/api/Pets", petSemTutor);

            
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var mensagem = await response.Content.ReadAsStringAsync();
            Assert.Contains("tutor informado não existe", mensagem);
        }

        [Fact]
        public async Task BuscarHistorico_PetComVacina_RetornaPetComRelacionamentos()
        {
            
            var idTutor = await CriarTutorAsync("tutor.historico@petcare360.com");

            var petResponse = await _client.PostAsJsonAsync("/api/Pets", new
            {
                nmPet = "Thor",
                especie = "Cachorro",
                raca = "Pastor Alemão",
                idTutor
            });
            var pet = await petResponse.Content.ReadFromJsonAsync<Pet>();

            await _client.PostAsJsonAsync("/api/Vacinas", new
            {
                nmVacina = "Antirrábica",
                fabricante = "Zoetis",
                dtAplicacao = "2026-08-01T00:00:00",
                idPet = pet!.IdPet
            });

            
            var response = await _client.GetAsync($"/api/Pets/{pet.IdPet}/historico");

            
            response.EnsureSuccessStatusCode();
            var historico = await response.Content.ReadFromJsonAsync<Pet>();
            Assert.NotNull(historico);
            Assert.Contains(historico.Vacinas, v => v.NmVacina == "Antirrábica");
        }

        [Fact]
        public async Task AtualizarPet_IdDaUrlDiferenteDoCorpo_RetornaBadRequest()
        {
            
            var idTutor = await CriarTutorAsync("tutor.update@petcare360.com");
            var petResponse = await _client.PostAsJsonAsync("/api/Pets", new
            {
                nmPet = "Nina",
                especie = "Gato",
                raca = "Persa",
                idTutor
            });
            var pet = await petResponse.Content.ReadFromJsonAsync<Pet>();

            
            var response = await _client.PutAsJsonAsync($"/api/Pets/{pet!.IdPet}", new
            {
                idPet = pet.IdPet + 500,
                nmPet = "Nina Editada",
                especie = "Gato",
                raca = "Persa",
                idTutor
            });

            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeletarPet_PetInexistente_RetornaNotFound()
        {
            
            var response = await _client.DeleteAsync("/api/Pets/999999");

            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CicloCompleto_CriarAtualizarEDeletar_FunicionaDePontaAPonta()
        {
            
            var idTutor = await CriarTutorAsync("tutor.ciclo@petcare360.com");

            
            var criacao = await _client.PostAsJsonAsync("/api/Pets", new
            {
                nmPet = "Bidu",
                especie = "Cachorro",
                raca = "Vira-lata",
                idTutor
            });
            var pet = await criacao.Content.ReadFromJsonAsync<Pet>();

            var atualizacao = await _client.PutAsJsonAsync($"/api/Pets/{pet!.IdPet}", new
            {
                idPet = pet.IdPet,
                nmPet = "Bidu Segundo",
                especie = "Cachorro",
                raca = "Vira-lata",
                idTutor
            });

            var consulta = await _client.GetAsync($"/api/Pets/{pet.IdPet}");
            var petAtualizado = await consulta.Content.ReadFromJsonAsync<Pet>();

            var exclusao = await _client.DeleteAsync($"/api/Pets/{pet.IdPet}");
            var buscaAposExclusao = await _client.GetAsync($"/api/Pets/{pet.IdPet}");

            
            Assert.Equal(HttpStatusCode.Created, criacao.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, atualizacao.StatusCode);
            Assert.Equal("Bidu Segundo", petAtualizado!.NmPet);
            Assert.Equal(HttpStatusCode.NoContent, exclusao.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, buscaAposExclusao.StatusCode);
        }
    }
}