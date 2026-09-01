using PetCare360.Domain.Entities;

namespace PetCare360.Domain.Interfaces
{
    public interface IPetRepository
    {
        Task<IEnumerable<Pet>> GetAllAsync();
        Task<Pet?> GetByIdAsync(int id);
        Task<IEnumerable<Pet>> GetByTutorAsync(int tutorId);
        Task<IEnumerable<Pet>> GetByEspecieAsync(string especie);
        Task<Pet?> GetHistoricoAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task AddAsync(Pet pet);
        Task UpdateAsync(Pet pet);
        Task DeleteAsync(Pet pet);
    }
}