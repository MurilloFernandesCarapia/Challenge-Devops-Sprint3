using Microsoft.EntityFrameworkCore;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Interfaces;
using PetCare360.Infrastructure.Data;

namespace PetCare360.Infrastructure.Repositories
{
    public class TutorRepository : ITutorRepository
    {
        private readonly AppDbContext _dbContext;

        public TutorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Tutor>> GetAllAsync()
        {
            return await _dbContext.Tutores.ToListAsync();
        }

        public async Task<Tutor?> GetByIdAsync(int id)
        {
            return await _dbContext.Tutores.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Tutores.AnyAsync(t => t.IdTutor == id);
        }

        public async Task AddAsync(Tutor tutor)
        {
            await _dbContext.Tutores.AddAsync(tutor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tutor tutor)
        {
            _dbContext.Tutores.Update(tutor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Tutor tutor)
        {
            _dbContext.Tutores.Remove(tutor);
            await _dbContext.SaveChangesAsync();
        }
    }
}