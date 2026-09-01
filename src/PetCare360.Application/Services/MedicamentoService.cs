using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using PetCare360.Application.Diagnostics;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Exceptions;
using PetCare360.Domain.Interfaces;

namespace PetCare360.Application.Services
{
    public class MedicamentoService : IMedicamentoService
    {
        private readonly IMedicamentoRepository _medicamentoRepository;
        private readonly IPetRepository _petRepository;
        private readonly ILogger<MedicamentoService> _logger;

        private static readonly ActivitySource ActivitySource = new(TelemetryConstants.ServiceName);

        private readonly Counter<int> _medicamentosPrescritosCounter;

        public MedicamentoService(
            IMedicamentoRepository medicamentoRepository,
            IPetRepository petRepository,
            ILogger<MedicamentoService> logger,
            IMeterFactory meterFactory)
        {
            _medicamentoRepository = medicamentoRepository;
            _petRepository = petRepository;
            _logger = logger;

            var meter = meterFactory.Create(TelemetryConstants.MeterName);
            _medicamentosPrescritosCounter = meter.CreateCounter<int>("medicamentos_created_total", description: "Total de medicamentos prescritos");
        }

        public async Task<IEnumerable<Medicamento>> GetAllAsync()
        {
            return await _medicamentoRepository.GetAllAsync();
        }

        public async Task<Medicamento?> GetByIdAsync(int id)
        {
            return await _medicamentoRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Medicamento>> GetByPetAsync(int petId)
        {
            return await _medicamentoRepository.GetByPetAsync(petId);
        }

        public async Task<Medicamento> CreateAsync(Medicamento medicamento)
        {
            using var activity = ActivitySource.StartActivity("PrescreverMedicamento");
            activity?.SetTag("medicamento.nome", medicamento.NmMedicamento);
            activity?.SetTag("medicamento.pet", medicamento.IdPet);

            //REGRA DE NEGÓCIO: medicamento sempre pertence a um pet cadastrado
            bool petExiste = await _petRepository.ExistsAsync(medicamento.IdPet);
            if (!petExiste)
            {
                _logger.LogWarning("Tentativa de prescrever medicamento com pet inexistente. IdPet: {IdPet}", medicamento.IdPet);
                throw new RegraDeNegocioException("O pet informado não existe.");
            }

            await _medicamentoRepository.AddAsync(medicamento);

            _logger.LogInformation("Medicamento prescrito com sucesso: {@Medicamento}", medicamento);
            _medicamentosPrescritosCounter.Add(1);

            return medicamento;
        }

        public async Task<bool> UpdateAsync(int id, Medicamento medicamentoAtualizado)
        {
            var medicamentoExistente = await _medicamentoRepository.GetByIdAsync(id);
            if (medicamentoExistente == null)
            {
                return false;
            }

            medicamentoExistente.NmMedicamento = medicamentoAtualizado.NmMedicamento;
            medicamentoExistente.Dosagem = medicamentoAtualizado.Dosagem;
            medicamentoExistente.Frequencia = medicamentoAtualizado.Frequencia;
            medicamentoExistente.DtInicio = medicamentoAtualizado.DtInicio;
            medicamentoExistente.DtFim = medicamentoAtualizado.DtFim;
            medicamentoExistente.IdPet = medicamentoAtualizado.IdPet;
            medicamentoExistente.IdConsulta = medicamentoAtualizado.IdConsulta;

            await _medicamentoRepository.UpdateAsync(medicamentoExistente);

            _logger.LogInformation("Medicamento atualizado. IdMedicamento: {IdMedicamento}", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var medicamento = await _medicamentoRepository.GetByIdAsync(id);
            if (medicamento == null)
            {
                return false;
            }

            await _medicamentoRepository.DeleteAsync(medicamento);

            _logger.LogInformation("Medicamento removido. IdMedicamento: {IdMedicamento}", id);
            return true;
        }
    }
}