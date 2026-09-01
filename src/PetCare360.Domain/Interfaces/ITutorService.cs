using PetCare360.Domain.Entities;

namespace PetCare360.Domain.Interfaces
{
    public interface ITutorService
    {
        Task<IEnumerable<Tutor>> GetAllAsync();
        Task<Tutor?> GetByIdAsync(int id);
        Task<Tutor> CreateAsync(Tutor tutor);
        Task<bool> UpdateAsync(int id, Tutor tutorAtualizado);
        Task<bool> DeleteAsync(int id);
    }
}