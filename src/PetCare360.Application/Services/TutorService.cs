using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using PetCare360.Application.Diagnostics;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Interfaces;

namespace PetCare360.Application.Services
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly ILogger<TutorService> _logger;

        private static readonly ActivitySource ActivitySource = new(TelemetryConstants.ServiceName);

        private readonly Counter<int> _tutoresCriadosCounter;

        public TutorService(
            ITutorRepository tutorRepository,
            ILogger<TutorService> logger,
            IMeterFactory meterFactory)
        {
            _tutorRepository = tutorRepository;
            _logger = logger;

            var meter = meterFactory.Create(TelemetryConstants.MeterName);
            _tutoresCriadosCounter = meter.CreateCounter<int>("tutores_created_total", description: "Total de tutores cadastrados");
        }

        public async Task<IEnumerable<Tutor>> GetAllAsync()
        {
            return await _tutorRepository.GetAllAsync();
        }

        public async Task<Tutor?> GetByIdAsync(int id)
        {
            return await _tutorRepository.GetByIdAsync(id);
        }

        public async Task<Tutor> CreateAsync(Tutor tutor)
        {
            using var activity = ActivitySource.StartActivity("CadastrarTutor");
            activity?.SetTag("tutor.nome", tutor.NmTutor);

            await _tutorRepository.AddAsync(tutor);

            _logger.LogInformation("Tutor cadastrado com sucesso: {@Tutor}", tutor);
            _tutoresCriadosCounter.Add(1);

            return tutor;
        }

        public async Task<bool> UpdateAsync(int id, Tutor tutorAtualizado)
        {
            var tutorExistente = await _tutorRepository.GetByIdAsync(id);
            if (tutorExistente == null)
            {
                return false;
            }

            tutorExistente.NmTutor = tutorAtualizado.NmTutor;
            tutorExistente.Cpf = tutorAtualizado.Cpf;
            tutorExistente.Email = tutorAtualizado.Email;
            tutorExistente.Telefone = tutorAtualizado.Telefone;
            tutorExistente.Endereco = tutorAtualizado.Endereco;

            await _tutorRepository.UpdateAsync(tutorExistente);

            _logger.LogInformation("Tutor atualizado. IdTutor: {IdTutor}", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tutor = await _tutorRepository.GetByIdAsync(id);
            if (tutor == null)
            {
                return false;
            }

            await _tutorRepository.DeleteAsync(tutor);

            _logger.LogInformation("Tutor removido. IdTutor: {IdTutor}", id);
            return true;
        }
    }
}