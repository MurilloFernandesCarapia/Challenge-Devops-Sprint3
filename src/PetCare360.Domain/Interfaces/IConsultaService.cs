using PetCare360.Domain.Entities;

namespace PetCare360.Domain.Interfaces
{
    public interface IConsultaService
    {
        Task<IEnumerable<Consulta>> GetAllAsync();
        Task<Consulta?> GetByIdAsync(int id);
        Task<IEnumerable<Consulta>> GetByPetAsync(int petId);
        Task<IEnumerable<Consulta>> GetByClinicaAsync(int clinicaId);
        Task<Consulta> CreateAsync(Consulta consulta);
        Task<bool> UpdateAsync(int id, Consulta consultaAtualizada);
        Task<bool> DeleteAsync(int id);
    }
}