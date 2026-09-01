using Microsoft.EntityFrameworkCore;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Interfaces;
using PetCare360.Infrastructure.Data;

namespace PetCare360.Infrastructure.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly AppDbContext _dbContext;

        public PetRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Pet>> GetAllAsync()
        {
            return await _dbContext.Pets.ToListAsync();
        }

        public async Task<Pet?> GetByIdAsync(int id)
        {
            return await _dbContext.Pets.FindAsync(id);
        }

        public async Task<IEnumerable<Pet>> GetByTutorAsync(int tutorId)
        {
            return await _dbContext.Pets
                .Where(p => p.IdTutor == tutorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pet>> GetByEspecieAsync(string especie)
        {
            return await _dbContext.Pets
                .Where(p => p.Especie.ToLower() == especie.ToLower())
                .ToListAsync();
        }

        public async Task<Pet?> GetHistoricoAsync(int id)
        {
            return await _dbContext.Pets
                .Include(p => p.Consultas)
                .Include(p => p.Vacinas)
                .Include(p => p.Medicamentos)
                .FirstOrDefaultAsync(p => p.IdPet == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Pets.AnyAsync(p => p.IdPet == id);
        }

        public async Task AddAsync(Pet pet)
        {
            await _dbContext.Pets.AddAsync(pet);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pet pet)
        {
            _dbContext.Pets.Update(pet);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Pet pet)
        {
            _dbContext.Pets.Remove(pet);
            await _dbContext.SaveChangesAsync();
        }
    }
}