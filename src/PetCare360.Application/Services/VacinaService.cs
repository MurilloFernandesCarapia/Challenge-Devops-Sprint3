using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using PetCare360.Application.Diagnostics;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Exceptions;
using PetCare360.Domain.Interfaces;

namespace PetCare360.Application.Services
{
    public class VacinaService : IVacinaService
    {
        private readonly IVacinaRepository _vacinaRepository;
        private readonly IPetRepository _petRepository;
        private readonly ILogger<VacinaService> _logger;

        private static readonly ActivitySource ActivitySource = new(TelemetryConstants.ServiceName);

        private readonly Counter<int> _vacinasAplicadasCounter;

        public VacinaService(
            IVacinaRepository vacinaRepository,
            IPetRepository petRepository,
            ILogger<VacinaService> logger,
            IMeterFactory meterFactory)
        {
            _vacinaRepository = vacinaRepository;
            _petRepository = petRepository;
            _logger = logger;

            var meter = meterFactory.Create(TelemetryConstants.MeterName);
            _vacinasAplicadasCounter = meter.CreateCounter<int>("vacinas_created_total", description: "Total de vacinas registradas");
        }

        public async Task<IEnumerable<Vacina>> GetAllAsync()
        {
            return await _vacinaRepository.GetAllAsync();
        }

        public async Task<Vacina?> GetByIdAsync(int id)
        {
            return await _vacinaRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Vacina>> GetByPetAsync(int petId)
        {
            return await _vacinaRepository.GetByPetAsync(petId);
        }

        public async Task<Vacina> CreateAsync(Vacina vacina)
        {
            using var activity = ActivitySource.StartActivity("RegistrarVacina");
            activity?.SetTag("vacina.nome", vacina.NmVacina);
            activity?.SetTag("vacina.pet", vacina.IdPet);

            //REGRA DE NEGÓCIO: vacina sempre pertence a um pet cadastrado
            bool petExiste = await _petRepository.ExistsAsync(vacina.IdPet);
            if (!petExiste)
            {
                _logger.LogWarning("Tentativa de registrar vacina com pet inexistente. IdPet: {IdPet}", vacina.IdPet);
                throw new RegraDeNegocioException("O pet informado não existe.");
            }

            await _vacinaRepository.AddAsync(vacina);

            _logger.LogInformation("Vacina registrada com sucesso: {@Vacina}", vacina);
            _vacinasAplicadasCounter.Add(1);

            return vacina;
        }

        public async Task<bool> UpdateAsync(int id, Vacina vacinaAtualizada)
        {
            var vacinaExistente = await _vacinaRepository.GetByIdAsync(id);
            if (vacinaExistente == null)
            {
                return false;
            }

            vacinaExistente.NmVacina = vacinaAtualizada.NmVacina;
            vacinaExistente.Fabricante = vacinaAtualizada.Fabricante;
            vacinaExistente.DtAplicacao = vacinaAtualizada.DtAplicacao;
            vacinaExistente.DtProximaDose = vacinaAtualizada.DtProximaDose;
            vacinaExistente.IdPet = vacinaAtualizada.IdPet;
            vacinaExistente.IdConsulta = vacinaAtualizada.IdConsulta;

            await _vacinaRepository.UpdateAsync(vacinaExistente);

            _logger.LogInformation("Vacina atualizada. IdVacina: {IdVacina}", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vacina = await _vacinaRepository.GetByIdAsync(id);
            if (vacina == null)
            {
                return false;
            }

            await _vacinaRepository.DeleteAsync(vacina);

            _logger.LogInformation("Vacina removida. IdVacina: {IdVacina}", id);
            return true;
        }
    }
}