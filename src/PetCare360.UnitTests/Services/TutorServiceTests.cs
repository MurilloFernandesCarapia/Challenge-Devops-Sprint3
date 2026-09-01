using Microsoft.Extensions.Logging;
using Moq;
using PetCare360.Application.Services;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Interfaces;
using PetCare360.UnitTests.Fixtures;

namespace PetCare360.UnitTests.Services
{
    [Collection("ServicesCollection")]
    public class TutorServiceTests
    {
        private readonly Mock<ITutorRepository> _mockTutorRepository;
        private readonly Mock<ILogger<TutorService>> _mockLogger;
        private readonly TutorService _tutorService;

        public TutorServiceTests(TelemetryFixture fixture)
        {
            _mockTutorRepository = new Mock<ITutorRepository>();
            _mockLogger = new Mock<ILogger<TutorService>>();

            _tutorService = new TutorService(
                _mockTutorRepository.Object,
                _mockLogger.Object,
                fixture.MeterFactory);
        }

        [Fact]
        public async Task GetAllAsync_ExistemTutores_RetornaListaCompleta()
        {
          
            var tutores = new List<Tutor>
            {
                new Tutor { IdTutor = 1, NmTutor = "Ana", Cpf = "111", Email = "ana@email.com" },
                new Tutor { IdTutor = 2, NmTutor = "Bruno", Cpf = "222", Email = "bruno@email.com" }
            };
            _mockTutorRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(tutores);

           
            var resultado = await _tutorService.GetAllAsync();

           
            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public async Task CreateAsync_DadosValidos_CadastraTutor()
        {
           
            var tutor = new Tutor { NmTutor = "Ana", Cpf = "111", Email = "ana@email.com" };

         
            var resultado = await _tutorService.CreateAsync(tutor);

         
            Assert.Equal("Ana", resultado.NmTutor);
            _mockTutorRepository.Verify(r => r.AddAsync(tutor), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_TutorNaoExiste_RetornaFalse()
        {
           
            _mockTutorRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Tutor?)null);

          
            var resultado = await _tutorService.DeleteAsync(99);

          
            Assert.False(resultado);
            _mockTutorRepository.Verify(r => r.DeleteAsync(It.IsAny<Tutor>()), Times.Never);
        }
    }
}