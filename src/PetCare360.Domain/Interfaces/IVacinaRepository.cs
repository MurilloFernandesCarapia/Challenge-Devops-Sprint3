using PetCare360.Domain.Entities;

namespace PetCare360.Domain.Interfaces
{
    public interface IVacinaRepository
    {
        Task<IEnumerable<Vacina>> GetAllAsync();
        Task<Vacina?> GetByIdAsync(int id);
        Task<IEnumerable<Vacina>> GetByPetAsync(int petId);
        Task AddAsync(Vacina vacina);
        Task UpdateAsync(Vacina vacina);
        Task DeleteAsync(Vacina vacina);
    }
}