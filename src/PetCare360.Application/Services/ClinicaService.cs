using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using PetCare360.Application.Diagnostics;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Interfaces;

namespace PetCare360.Application.Services
{
    public class ClinicaService : IClinicaService
    {
        private readonly IClinicaRepository _clinicaRepository;
        private readonly ILogger<ClinicaService> _logger;

        private static readonly ActivitySource ActivitySource = new(TelemetryConstants.ServiceName);

        private readonly Counter<int> _clinicasCriadasCounter;

        public ClinicaService(
            IClinicaRepository clinicaRepository,
            ILogger<ClinicaService> logger,
            IMeterFactory meterFactory)
        {
            _clinicaRepository = clinicaRepository;
            _logger = logger;

            var meter = meterFactory.Create(TelemetryConstants.MeterName);
            _clinicasCriadasCounter = meter.CreateCounter<int>("clinicas_created_total", description: "Total de clínicas cadastradas");
        }

        public async Task<IEnumerable<Clinica>> GetAllAsync()
        {
            return await _clinicaRepository.GetAllAsync();
        }

        public async Task<Clinica?> GetByIdAsync(int id)
        {
            return await _clinicaRepository.GetByIdAsync(id);
        }

        public async Task<Clinica?> GetByCnpjAsync(string cnpj)
        {
            return await _clinicaRepository.GetByCnpjAsync(cnpj);
        }

        public async Task<Clinica> CreateAsync(Clinica clinica)
        {
            using var activity = ActivitySource.StartActivity("CadastrarClinica");
            activity?.SetTag("clinica.nome", clinica.NmClinica);

            await _clinicaRepository.AddAsync(clinica);

            _logger.LogInformation("Clínica cadastrada com sucesso: {@Clinica}", clinica);
            _clinicasCriadasCounter.Add(1);

            return clinica;
        }

        public async Task<bool> UpdateAsync(int id, Clinica clinicaAtualizada)
        {
            var clinicaExistente = await _clinicaRepository.GetByIdAsync(id);
            if (clinicaExistente == null)
            {
                return false;
            }

            clinicaExistente.NmClinica = clinicaAtualizada.NmClinica;
            clinicaExistente.Cnpj = clinicaAtualizada.Cnpj;
            clinicaExistente.Endereco = clinicaAtualizada.Endereco;
            clinicaExistente.Telefone = clinicaAtualizada.Telefone;
            clinicaExistente.Email = clinicaAtualizada.Email;

            await _clinicaRepository.UpdateAsync(clinicaExistente);

            _logger.LogInformation("Clínica atualizada. IdClinica: {IdClinica}", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var clinica = await _clinicaRepository.GetByIdAsync(id);
            if (clinica == null)
            {
                return false;
            }

            await _clinicaRepository.DeleteAsync(clinica);

            _logger.LogInformation("Clínica removida. IdClinica: {IdClinica}", id);
            return true;
        }
    }
}