using Microsoft.EntityFrameworkCore;
using PetCare360.Domain.Entities;


namespace PetCare360.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Tutor> Tutores { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Clinica> Clinicas { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<Vacina> Vacinas { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //aqui embaixo são indices unicos, não podem ter dois tutores com o mesmo cpf e o mesmo email
            modelBuilder.Entity<Tutor>().HasIndex(t => t.Cpf).IsUnique();
            modelBuilder.Entity<Tutor>().HasIndex(t => t.Email).IsUnique();
            //tambem nao pode ter duas clinicas com o mesmo cnpj
            modelBuilder.Entity<Clinica>().HasIndex(c => c.Cnpj).IsUnique();
            //Aqui serve para a precisãodo peso do pet
            modelBuilder.Entity<Pet>().Property(p => p.Peso).HasPrecision(6, 2);
            //Um tutor que tenha pets vinculados nao pode ser deletado
            modelBuilder.Entity<Pet>().HasOne(p => p.Tutor).WithMany(t => t.Pets).HasForeignKey(p => p.IdTutor).OnDelete(DeleteBehavior.Restrict);
            //tambem nao podemos deletar clinicas que tem historicos de consultas
            modelBuilder.Entity<Consulta>().HasOne(c => c.Clinica).WithMany(cl => cl.Consultas).HasForeignKey(c => c.IdClinica).OnDelete(DeleteBehavior.Restrict);



            base.OnModelCreating(modelBuilder);
        }
    }
}
