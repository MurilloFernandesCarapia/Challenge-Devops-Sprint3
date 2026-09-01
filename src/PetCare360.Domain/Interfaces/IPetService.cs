using PetCare360.Domain.Entities;

namespace PetCare360.Domain.Interfaces
{
    public interface IPetService
    {
        Task<IEnumerable<Pet>> GetAllAsync();
        Task<Pet?> GetByIdAsync(int id);
        Task<IEnumerable<Pet>> GetByTutorAsync(int tutorId);
        Task<IEnumerable<Pet>> GetByEspecieAsync(string especie);
        Task<Pet?> GetHistoricoAsync(int id);
        Task<Pet> CreateAsync(Pet pet);
        Task<bool> UpdateAsync(int id, Pet petAtualizado);
        Task<bool> DeleteAsync(int id);
    }
}