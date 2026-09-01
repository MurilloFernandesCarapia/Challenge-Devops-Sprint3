using Microsoft.EntityFrameworkCore;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Interfaces;
using PetCare360.Infrastructure.Data;

namespace PetCare360.Infrastructure.Repositories
{
    public class VacinaRepository : IVacinaRepository
    {
        private readonly AppDbContext _dbContext;

        public VacinaRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Vacina>> GetAllAsync()
        {
            return await _dbContext.Vacinas.ToListAsync();
        }

        public async Task<Vacina?> GetByIdAsync(int id)
        {
            return await _dbContext.Vacinas.FindAsync(id);
        }

        public async Task<IEnumerable<Vacina>> GetByPetAsync(int petId)
        {
            return await _dbContext.Vacinas
                .Where(v => v.IdPet == petId)
                .ToListAsync();
        }

        public async Task AddAsync(Vacina vacina)
        {
            await _dbContext.Vacinas.AddAsync(vacina);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vacina vacina)
        {
            _dbContext.Vacinas.Update(vacina);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Vacina vacina)
        {
            _dbContext.Vacinas.Remove(vacina);
            await _dbContext.SaveChangesAsync();
        }
    }
}