using PetCare360.Domain.Entities;

namespace PetCare360.Domain.Interfaces
{
    public interface ITutorRepository
    {
        Task<IEnumerable<Tutor>> GetAllAsync();
        Task<Tutor?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task AddAsync(Tutor tutor);
        Task UpdateAsync(Tutor tutor);
        Task DeleteAsync(Tutor tutor);
    }
}