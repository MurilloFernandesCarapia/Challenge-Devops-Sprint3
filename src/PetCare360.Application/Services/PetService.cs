using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using PetCare360.Application.Diagnostics;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Exceptions;
using PetCare360.Domain.Interfaces;

namespace PetCare360.Application.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly ILogger<PetService> _logger;

        
        private static readonly ActivitySource ActivitySource = new(TelemetryConstants.ServiceName);

        
        private readonly Counter<int> _petsCriadosCounter;

        public PetService(
            IPetRepository petRepository,
            ITutorRepository tutorRepository,
            ILogger<PetService> logger,
            IMeterFactory meterFactory)
        {
            _petRepository = petRepository;
            _tutorRepository = tutorRepository;
            _logger = logger;

            var meter = meterFactory.Create(TelemetryConstants.MeterName);
            _petsCriadosCounter = meter.CreateCounter<int>("pets_created_total", description: "Total de pets cadastrados");
        }

        public async Task<IEnumerable<Pet>> GetAllAsync()
        {
            return await _petRepository.GetAllAsync();
        }

        public async Task<Pet?> GetByIdAsync(int id)
        {
            return await _petRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Pet>> GetByTutorAsync(int tutorId)
        {
            return await _petRepository.GetByTutorAsync(tutorId);
        }

        public async Task<IEnumerable<Pet>> GetByEspecieAsync(string especie)
        {
            return await _petRepository.GetByEspecieAsync(especie);
        }

        public async Task<Pet?> GetHistoricoAsync(int id)
        {
            
            using var activity = ActivitySource.StartActivity("ConsultarHistoricoPet");
            activity?.SetTag("pet.id", id);

            return await _petRepository.GetHistoricoAsync(id);
        }

        public async Task<Pet> CreateAsync(Pet pet)
        {
            using var activity = ActivitySource.StartActivity("CadastrarPet");
            activity?.SetTag("pet.nome", pet.NmPet);
            activity?.SetTag("pet.especie", pet.Especie);

            //REGRA DE NEGÓCIO: não existe pet sem tutor responsável
            bool tutorExiste = await _tutorRepository.ExistsAsync(pet.IdTutor);
            if (!tutorExiste)
            {
                _logger.LogWarning("Tentativa de cadastrar pet com tutor inexistente. IdTutor: {IdTutor}", pet.IdTutor);
                throw new RegraDeNegocioException("O tutor informado não existe.");
            }

            await _petRepository.AddAsync(pet);

            //2. LOG ESTRUTURADO: o {@Pet} serializa o objeto inteiro
            _logger.LogInformation("Pet cadastrado com sucesso: {@Pet}", pet);

            //3. MÉTRICA: incrementa o contador com a espécie como tag
            _petsCriadosCounter.Add(1, new KeyValuePair<string, object?>("especie", pet.Especie));

            return pet;
        }

        public async Task<bool> UpdateAsync(int id, Pet petAtualizado)
        {
            var petExistente = await _petRepository.GetByIdAsync(id);
            if (petExistente == null)
            {
                return false;
            }

            petExistente.NmPet = petAtualizado.NmPet;
            petExistente.Especie = petAtualizado.Especie;
            petExistente.Raca = petAtualizado.Raca;
            petExistente.DtNascimento = petAtualizado.DtNascimento;
            petExistente.Peso = petAtualizado.Peso;
            petExistente.IdTutor = petAtualizado.IdTutor;

            await _petRepository.UpdateAsync(petExistente);

            _logger.LogInformation("Pet atualizado. IdPet: {IdPet}", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pet = await _petRepository.GetByIdAsync(id);
            if (pet == null)
            {
                return false;
            }

            await _petRepository.DeleteAsync(pet);

            _logger.LogInformation("Pet removido. IdPet: {IdPet}", id);
            return true;
        }
    }
}