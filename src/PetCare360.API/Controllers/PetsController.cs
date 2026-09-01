using Microsoft.AspNetCore.Mvc;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Exceptions;
using PetCare360.Domain.Interfaces;

namespace PetCare360.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly ILogger<PetsController> _logger;

        public PetsController(IPetService petService, ILogger<PetsController> logger)
        {
            _petService = petService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var pets = await _petService.GetAllAsync();
            return Ok(pets);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var pet = await _petService.GetByIdAsync(id);
            if (pet == null)
            {
                return NotFound("Pet não encontrado.");
            }
            return Ok(pet);
        }

        [HttpGet("tutor/{tutorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByTutor(int tutorId)
        {
            var pets = await _petService.GetByTutorAsync(tutorId);
            return Ok(pets);
        }

        [HttpGet("especie/{especie}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByEspecie(string especie)
        {
            var pets = await _petService.GetByEspecieAsync(especie);
            return Ok(pets);
        }

        [HttpGet("{id}/historico")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHistorico(int id)
        {
            var pet = await _petService.GetHistoricoAsync(id);
            if (pet == null)
            {
                return NotFound("Pet não encontrado.");
            }
            return Ok(pet);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var petCriado = await _petService.CreateAsync(pet);
                return CreatedAtAction(nameof(GetById), new { id = petCriado.IdPet }, petCriado);
            }
            catch (RegraDeNegocioException ex)
            {
                _logger.LogWarning(ex, "Regra de negócio violada ao cadastrar pet.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Pet petAtualizado)
        {
            if (id != petAtualizado.IdPet)
            {
                return BadRequest("O ID da URL não confere com o ID do corpo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool atualizado = await _petService.UpdateAsync(id, petAtualizado);
            if (!atualizado)
            {
                return NotFound("Pet não encontrado.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            bool removido = await _petService.DeleteAsync(id);
            if (!removido)
            {
                return NotFound("Pet não encontrado.");
            }

            return NoContent();
        }
    }
}