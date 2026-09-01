using Microsoft.EntityFrameworkCore;
using PetCare360.Domain.Entities;
using PetCare360.Domain.Interfaces;
using PetCare360.Infrastructure.Data;

namespace PetCare360.Infrastructure.Repositories
{
    public class ConsultaRepository : IConsultaRepository
    {
        private readonly AppDbContext _dbContext;

        public ConsultaRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Consulta>> GetAllAsync()
        {
            return await _dbContext.Consultas.ToListAsync();
        }

        public async Task<Consulta?> GetByIdAsync(int id)
        {
            return await _dbContext.Consultas.FindAsync(id);
        }

        public async Task<IEnumerable<Consulta>> GetByPetAsync(int petId)
        {
            return await _dbContext.Consultas
                .Where(c => c.IdPet == petId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Consulta>> GetByClinicaAsync(int clinicaId)
        {
            return await _dbContext.Consultas
                .Where(c => c.IdClinica == clinicaId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Consultas.AnyAsync(c => c.IdConsulta == id);
        }

        public async Task AddAsync(Consulta consulta)
        {
            await _dbContext.Consultas.AddAsync(consulta);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Consulta consulta)
        {
            _dbContext.Consultas.Update(consulta);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Consulta consulta)
        {
            _dbContext.Consultas.Remove(consulta);
            await _dbContext.SaveChangesAsync();
        }
    }
}