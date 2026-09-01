using Microsoft.AspNetCore.Mvc;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Exceptions;
using PetCare360.Domain.Interfaces;

namespace PetCare360.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacinasController : ControllerBase
    {
        private readonly IVacinaService _vacinaService;
        private readonly ILogger<VacinasController> _logger;

        public VacinasController(IVacinaService vacinaService, ILogger<VacinasController> logger)
        {
            _vacinaService = vacinaService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var vacinas = await _vacinaService.GetAllAsync();
            return Ok(vacinas);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var vacina = await _vacinaService.GetByIdAsync(id);
            if (vacina == null)
            {
                return NotFound("Vacina não encontrada.");
            }
            return Ok(vacina);
        }

        [HttpGet("pet/{petId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPet(int petId)
        {
            var vacinas = await _vacinaService.GetByPetAsync(petId);
            return Ok(vacinas);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Vacina vacina)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var vacinaCriada = await _vacinaService.CreateAsync(vacina);
                return CreatedAtAction(nameof(GetById), new { id = vacinaCriada.IdVacina }, vacinaCriada);
            }
            catch (RegraDeNegocioException ex)
            {
                _logger.LogWarning(ex, "Regra de negócio violada ao registrar vacina.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Vacina vacinaAtualizada)
        {
            if (id != vacinaAtualizada.IdVacina)
            {
                return BadRequest("O ID da URL não confere com o ID do corpo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool atualizada = await _vacinaService.UpdateAsync(id, vacinaAtualizada);
            if (!atualizada)
            {
                return NotFound("Vacina não encontrada.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            bool removida = await _vacinaService.DeleteAsync(id);
            if (!removida)
            {
                return NotFound("Vacina não encontrada.");
            }

            return NoContent();
        }
    }
}