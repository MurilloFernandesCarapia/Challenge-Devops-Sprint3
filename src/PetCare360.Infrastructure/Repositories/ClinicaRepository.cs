using Microsoft.EntityFrameworkCore;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Interfaces;
using PetCare360.Infrastructure.Data;

namespace PetCare360.Infrastructure.Repositories
{
    public class ClinicaRepository : IClinicaRepository
    {
        private readonly AppDbContext _dbContext;

        public ClinicaRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Clinica>> GetAllAsync()
        {
            return await _dbContext.Clinicas.ToListAsync();
        }

        public async Task<Clinica?> GetByIdAsync(int id)
        {
            return await _dbContext.Clinicas.FindAsync(id);
        }

        public async Task<Clinica?> GetByCnpjAsync(string cnpj)
        {
            return await _dbContext.Clinicas
                .FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Clinicas.AnyAsync(c => c.IdClinica == id);
        }

        public async Task AddAsync(Clinica clinica)
        {
            await _dbContext.Clinicas.AddAsync(clinica);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Clinica clinica)
        {
            _dbContext.Clinicas.Update(clinica);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Clinica clinica)
        {
            _dbContext.Clinicas.Remove(clinica);
            await _dbContext.SaveChangesAsync();
        }
    }
}