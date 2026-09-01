using Microsoft.Extensions.Logging;
using Moq;
using PetCare360.Application.Services;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Exceptions;
using PetCare360.Domain.Interfaces;
using PetCare360.UnitTests.Fixtures;

namespace PetCare360.UnitTests.Services
{
    [Collection("ServicesCollection")]
    public class PetServiceTests
    {
        private readonly Mock<IPetRepository> _mockPetRepository;
        private readonly Mock<ITutorRepository> _mockTutorRepository;
        private readonly Mock<ILogger<PetService>> _mockLogger;
        private readonly PetService _petService;

        public PetServiceTests(TelemetryFixture fixture)
        {
            
            _mockPetRepository = new Mock<IPetRepository>();
            _mockTutorRepository = new Mock<ITutorRepository>();
            _mockLogger = new Mock<ILogger<PetService>>();

            _petService = new PetService(
                _mockPetRepository.Object,
                _mockTutorRepository.Object,
                _mockLogger.Object,
                fixture.MeterFactory);
        }

        [Fact]
        public async Task CreateAsync_TutorExiste_CadastraPet()
        {
            
            var pet = new Pet { NmPet = "Rex", Especie = "Cachorro", IdTutor = 1 };
            _mockTutorRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

           
            var resultado = await _petService.CreateAsync(pet);

         
            Assert.Equal("Rex", resultado.NmPet);
            _mockPetRepository.Verify(r => r.AddAsync(pet), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_TutorNaoExiste_LancaRegraDeNegocioException()
        {
           
            var pet = new Pet { NmPet = "Mia", Especie = "Gato", IdTutor = 999 };
            _mockTutorRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

         
            var excecao = await Assert.ThrowsAsync<RegraDeNegocioException>(
                () => _petService.CreateAsync(pet));

           
            Assert.Equal("O tutor informado não existe.", excecao.Message);
        
            _mockPetRepository.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_PetExiste_RetornaPet()
        {
         
            var petEsperado = new Pet { IdPet = 1, NmPet = "Rex", Especie = "Cachorro" };
            _mockPetRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(petEsperado);

           
            var resultado = await _petService.GetByIdAsync(1);

           
            Assert.NotNull(resultado);
            Assert.Equal("Rex", resultado.NmPet);
        }

        [Fact]
        public async Task GetByIdAsync_PetNaoExiste_RetornaNull()
        {
           
            _mockPetRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Pet?)null);

           
            var resultado = await _petService.GetByIdAsync(99);

         
            Assert.Null(resultado);
        }

        [Fact]
        public async Task UpdateAsync_PetExiste_AtualizaDadosERetornaTrue()
        {
           
            var petExistente = new Pet { IdPet = 1, NmPet = "Rex", Especie = "Cachorro", IdTutor = 1 };
            var petAtualizado = new Pet { IdPet = 1, NmPet = "Rex Junior", Especie = "Cachorro", Raca = "Labrador", IdTutor = 1 };

            _mockPetRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(petExistente);

          
            var resultado = await _petService.UpdateAsync(1, petAtualizado);

           
            Assert.True(resultado);
            Assert.Equal("Rex Junior", petExistente.NmPet);
            Assert.Equal("Labrador", petExistente.Raca);
            _mockPetRepository.Verify(r => r.UpdateAsync(petExistente), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_PetNaoExiste_RetornaFalse()
        {
           
            _mockPetRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Pet?)null);

           
            var resultado = await _petService.UpdateAsync(99, new Pet { IdPet = 99 });

           
            Assert.False(resultado);
            _mockPetRepository.Verify(r => r.UpdateAsync(It.IsAny<Pet>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_PetExiste_RemoveERetornaTrue()
        {
           
            var pet = new Pet { IdPet = 1, NmPet = "Rex", Especie = "Cachorro" };
            _mockPetRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pet);

           
            var resultado = await _petService.DeleteAsync(1);

          
            Assert.True(resultado);
            _mockPetRepository.Verify(r => r.DeleteAsync(pet), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_PetNaoExiste_RetornaFalse()
        {
           
            _mockPetRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Pet?)null);

           
            var resultado = await _petService.DeleteAsync(99);

          
            Assert.False(resultado);
            _mockPetRepository.Verify(r => r.DeleteAsync(It.IsAny<Pet>()), Times.Never);
        }

        [Fact]
        public async Task GetHistoricoAsync_PetExiste_RetornaPetComRelacionamentos()
        {
           
            var pet = new Pet { IdPet = 1, NmPet = "Rex", Especie = "Cachorro" };
            pet.Consultas.Add(new Consulta { IdConsulta = 1, IdPet = 1 });
            pet.Vacinas.Add(new Vacina { IdVacina = 1, NmVacina = "V10", IdPet = 1 });

            _mockPetRepository.Setup(r => r.GetHistoricoAsync(1)).ReturnsAsync(pet);

           
            var resultado = await _petService.GetHistoricoAsync(1);

           
            Assert.NotNull(resultado);
            Assert.Single(resultado.Consultas);
            Assert.Single(resultado.Vacinas);
        }
    }
}