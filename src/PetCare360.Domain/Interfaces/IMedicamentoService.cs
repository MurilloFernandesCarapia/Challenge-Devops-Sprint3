using PetCare360.Domain.Entities;

namespace PetCare360.Domain.Interfaces
{
    public interface IMedicamentoService
    {
        Task<IEnumerable<Medicamento>> GetAllAsync();
        Task<Medicamento?> GetByIdAsync(int id);
        Task<IEnumerable<Medicamento>> GetByPetAsync(int petId);
        Task<Medicamento> CreateAsync(Medicamento medicamento);
        Task<bool> UpdateAsync(int id, Medicamento medicamentoAtualizado);
        Task<bool> DeleteAsync(int id);
    }
}