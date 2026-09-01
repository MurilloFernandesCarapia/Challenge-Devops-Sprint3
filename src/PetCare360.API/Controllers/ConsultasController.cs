using Microsoft.AspNetCore.Mvc;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Exceptions;
using PetCare360.Domain.Interfaces;

namespace PetCare360.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultasController : ControllerBase
    {
        private readonly IConsultaService _consultaService;
        private readonly ILogger<ConsultasController> _logger;

        public ConsultasController(IConsultaService consultaService, ILogger<ConsultasController> logger)
        {
            _consultaService = consultaService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var consultas = await _consultaService.GetAllAsync();
            return Ok(consultas);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var consulta = await _consultaService.GetByIdAsync(id);
            if (consulta == null)
            {
                return NotFound("Consulta não encontrada.");
            }
            return Ok(consulta);
        }

        [HttpGet("pet/{petId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPet(int petId)
        {
            var consultas = await _consultaService.GetByPetAsync(petId);
            return Ok(consultas);
        }

        [HttpGet("clinica/{clinicaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByClinica(int clinicaId)
        {
            var consultas = await _consultaService.GetByClinicaAsync(clinicaId);
            return Ok(consultas);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Consulta consulta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var consultaCriada = await _consultaService.CreateAsync(consulta);
                return CreatedAtAction(nameof(GetById), new { id = consultaCriada.IdConsulta }, consultaCriada);
            }
            catch (RegraDeNegocioException ex)
            {
                _logger.LogWarning(ex, "Regra de negócio violada ao registrar consulta.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Consulta consultaAtualizada)
        {
            if (id != consultaAtualizada.IdConsulta)
            {
                return BadRequest("O ID da URL não confere com o ID do corpo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool atualizada = await _consultaService.UpdateAsync(id, consultaAtualizada);
            if (!atualizada)
            {
                return NotFound("Consulta não encontrada.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            bool removida = await _consultaService.DeleteAsync(id);
            if (!removida)
            {
                return NotFound("Consulta não encontrada.");
            }

            return NoContent();
        }
    }
}