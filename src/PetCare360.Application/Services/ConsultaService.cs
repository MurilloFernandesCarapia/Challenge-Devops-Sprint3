using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using PetCare360.Application.Diagnostics;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Exceptions;
using PetCare360.Domain.Interfaces;

namespace PetCare360.Application.Services
{
    public class ConsultaService : IConsultaService
    {
        private readonly IConsultaRepository _consultaRepository;
        private readonly IPetRepository _petRepository;
        private readonly IClinicaRepository _clinicaRepository;
        private readonly ILogger<ConsultaService> _logger;

        private static readonly ActivitySource ActivitySource = new(TelemetryConstants.ServiceName);

        private readonly Counter<int> _consultasCriadasCounter;

        public ConsultaService(
            IConsultaRepository consultaRepository,
            IPetRepository petRepository,
            IClinicaRepository clinicaRepository,
            ILogger<ConsultaService> logger,
            IMeterFactory meterFactory)
        {
            _consultaRepository = consultaRepository;
            _petRepository = petRepository;
            _clinicaRepository = clinicaRepository;
            _logger = logger;

            var meter = meterFactory.Create(TelemetryConstants.MeterName);
            _consultasCriadasCounter = meter.CreateCounter<int>("consultas_created_total", description: "Total de consultas registradas");
        }

        public async Task<IEnumerable<Consulta>> GetAllAsync()
        {
            return await _consultaRepository.GetAllAsync();
        }

        public async Task<Consulta?> GetByIdAsync(int id)
        {
            return await _consultaRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Consulta>> GetByPetAsync(int petId)
        {
            return await _consultaRepository.GetByPetAsync(petId);
        }

        public async Task<IEnumerable<Consulta>> GetByClinicaAsync(int clinicaId)
        {
            return await _consultaRepository.GetByClinicaAsync(clinicaId);
        }

        public async Task<Consulta> CreateAsync(Consulta consulta)
        {
            using var activity = ActivitySource.StartActivity("RegistrarConsulta");
            activity?.SetTag("consulta.pet", consulta.IdPet);
            activity?.SetTag("consulta.clinica", consulta.IdClinica);

            //REGRA DE NEGÓCIO: consulta só existe para um pet cadastrado
            bool petExiste = await _petRepository.ExistsAsync(consulta.IdPet);
            if (!petExiste)
            {
                _logger.LogWarning("Tentativa de registrar consulta com pet inexistente. IdPet: {IdPet}", consulta.IdPet);
                throw new RegraDeNegocioException("O pet informado não existe.");
            }

            //REGRA DE NEGÓCIO: e sempre em uma clínica cadastrada
            bool clinicaExiste = await _clinicaRepository.ExistsAsync(consulta.IdClinica);
            if (!clinicaExiste)
            {
                _logger.LogWarning("Tentativa de registrar consulta com clínica inexistente. IdClinica: {IdClinica}", consulta.IdClinica);
                throw new RegraDeNegocioException("A clínica informada não existe.");
            }

            await _consultaRepository.AddAsync(consulta);

            _logger.LogInformation("Consulta registrada com sucesso: {@Consulta}", consulta);
            _consultasCriadasCounter.Add(1);

            return consulta;
        }

        public async Task<bool> UpdateAsync(int id, Consulta consultaAtualizada)
        {
            var consultaExistente = await _consultaRepository.GetByIdAsync(id);
            if (consultaExistente == null)
            {
                return false;
            }

            consultaExistente.DtConsulta = consultaAtualizada.DtConsulta;
            consultaExistente.Descricao = consultaAtualizada.Descricao;
            consultaExistente.Diagnostico = consultaAtualizada.Diagnostico;
            consultaExistente.IdPet = consultaAtualizada.IdPet;
            consultaExistente.IdClinica = consultaAtualizada.IdClinica;

            await _consultaRepository.UpdateAsync(consultaExistente);

            _logger.LogInformation("Consulta atualizada. IdConsulta: {IdConsulta}", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var consulta = await _consultaRepository.GetByIdAsync(id);
            if (consulta == null)
            {
                return false;
            }

            await _consultaRepository.DeleteAsync(consulta);

            _logger.LogInformation("Consulta removida. IdConsulta: {IdConsulta}", id);
            return true;
        }
    }
}