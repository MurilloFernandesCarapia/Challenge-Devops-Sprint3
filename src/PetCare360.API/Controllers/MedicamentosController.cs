using Microsoft.AspNetCore.Mvc;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Exceptions;
using PetCare360.Domain.Interfaces;

namespace PetCare360.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicamentosController : ControllerBase
    {
        private readonly IMedicamentoService _medicamentoService;
        private readonly ILogger<MedicamentosController> _logger;

        public MedicamentosController(IMedicamentoService medicamentoService, ILogger<MedicamentosController> logger)
        {
            _medicamentoService = medicamentoService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var medicamentos = await _medicamentoService.GetAllAsync();
            return Ok(medicamentos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var medicamento = await _medicamentoService.GetByIdAsync(id);
            if (medicamento == null)
            {
                return NotFound("Medicamento não encontrado.");
            }
            return Ok(medicamento);
        }

        [HttpGet("pet/{petId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPet(int petId)
        {
            var medicamentos = await _medicamentoService.GetByPetAsync(petId);
            return Ok(medicamentos);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Medicamento medicamento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var medicamentoCriado = await _medicamentoService.CreateAsync(medicamento);
                return CreatedAtAction(nameof(GetById), new { id = medicamentoCriado.IdMedicamento }, medicamentoCriado);
            }
            catch (RegraDeNegocioException ex)
            {
                _logger.LogWarning(ex, "Regra de negócio violada ao prescrever medicamento.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Medicamento medicamentoAtualizado)
        {
            if (id != medicamentoAtualizado.IdMedicamento)
            {
                return BadRequest("O ID da URL não confere com o ID do corpo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool atualizado = await _medicamentoService.UpdateAsync(id, medicamentoAtualizado);
            if (!atualizado)
            {
                return NotFound("Medicamento não encontrado.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            bool removido = await _medicamentoService.DeleteAsync(id);
            if (!removido)
            {
                return NotFound("Medicamento não encontrado.");
            }

            return NoContent();
        }
    }
}