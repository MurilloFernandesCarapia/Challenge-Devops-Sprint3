using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PetCare360.API.Controllers;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Exceptions;
using PetCare360.Domain.Interfaces;

namespace PetCare360.UnitTests.Controllers
{
    public class PetsControllerTests
    {
        private readonly Mock<IPetService> _mockService;
        private readonly Mock<ILogger<PetsController>> _mockLogger;
        private readonly PetsController _controller;

        public PetsControllerTests()
        {
            
            _mockService = new Mock<IPetService>();
            _mockLogger = new Mock<ILogger<PetsController>>();
            _controller = new PetsController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetById_PetExiste_RetornaOk()
        {
           
            var petEsperado = new Pet { IdPet = 1, NmPet = "Rex", Especie = "Cachorro" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(petEsperado);

           
            var resultado = await _controller.GetById(1);

           
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var petRetornado = Assert.IsType<Pet>(okResult.Value);
            Assert.Equal("Rex", petRetornado.NmPet);
        }

        [Fact]
        public async Task GetById_PetNaoExiste_RetornaNotFound()
        {
            
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Pet?)null);

            
            var resultado = await _controller.GetById(99);

           
            Assert.IsType<NotFoundObjectResult>(resultado);
        }

        [Fact]
        public async Task Create_DadosValidos_RetornaCreated()
        {
           
            var pet = new Pet { IdPet = 1, NmPet = "Rex", Especie = "Cachorro", IdTutor = 1 };
            _mockService.Setup(s => s.CreateAsync(It.IsAny<Pet>())).ReturnsAsync(pet);

           
            var resultado = await _controller.Create(pet);

           
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(PetsController.GetById), createdResult.ActionName);
        }

        [Fact]
        public async Task Create_TutorInexistente_RetornaBadRequest()
        {
            
            var pet = new Pet { NmPet = "Rex", Especie = "Cachorro", IdTutor = 999 };
            _mockService.Setup(s => s.CreateAsync(It.IsAny<Pet>()))
                .ThrowsAsync(new RegraDeNegocioException("O tutor informado não existe."));

            
            var resultado = await _controller.Create(pet);

           
            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal("O tutor informado não existe.", badRequest.Value);
        }

        [Fact]
        public async Task Update_IdDaUrlDiferenteDoCorpo_RetornaBadRequest()
        {
            
            var pet = new Pet { IdPet = 2, NmPet = "Rex", Especie = "Cachorro" };

           
            var resultado = await _controller.Update(1, pet);

            
            Assert.IsType<BadRequestObjectResult>(resultado);
            
            _mockService.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<Pet>()), Times.Never);
        }

        [Fact]
        public async Task Delete_PetNaoExiste_RetornaNotFound()
        {
           
            _mockService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

            
            var resultado = await _controller.Delete(99);

            
            Assert.IsType<NotFoundObjectResult>(resultado);
        }
    }
}