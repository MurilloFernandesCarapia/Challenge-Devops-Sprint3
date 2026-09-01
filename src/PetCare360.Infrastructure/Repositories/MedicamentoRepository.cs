using Microsoft.EntityFrameworkCore;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Interfaces;
using PetCare360.Infrastructure.Data;

namespace PetCare360.Infrastructure.Repositories
{
    public class MedicamentoRepository : IMedicamentoRepository
    {
        private readonly AppDbContext _dbContext;

        public MedicamentoRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Medicamento>> GetAllAsync()
        {
            return await _dbContext.Medicamentos.ToListAsync();
        }

        public async Task<Medicamento?> GetByIdAsync(int id)
        {
            return await _dbContext.Medicamentos.FindAsync(id);
        }

        public async Task<IEnumerable<Medicamento>> GetByPetAsync(int petId)
        {
            return await _dbContext.Medicamentos
                .Where(m => m.IdPet == petId)
                .ToListAsync();
        }

        public async Task AddAsync(Medicamento medicamento)
        {
            await _dbContext.Medicamentos.AddAsync(medicamento);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Medicamento medicamento)
        {
            _dbContext.Medicamentos.Update(medicamento);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Medicamento medicamento)
        {
            _dbContext.Medicamentos.Remove(medicamento);
            await _dbContext.SaveChangesAsync();
        }
    }
}