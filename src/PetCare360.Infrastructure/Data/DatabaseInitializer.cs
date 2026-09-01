using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetCare360.Domain.Entities;

namespace PetCare360.Infrastructure.Data
{
    /// <summary>
    /// </summary>
    public static class DatabaseInitializer
    {
        public static async Task InicializarAsync(
            IServiceProvider services,
            int maxTentativas = 30,
            int esperaSegundos = 10)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            // O Oracle XE demora para aceitar conexao. No ACI os dois containers sobem
            // ao mesmo tempo, entao a API precisa insistir ate o banco responder.
            for (var tentativa = 1; tentativa <= maxTentativas; tentativa++)
            {
                try
                {
                    logger.LogInformation(
                        "[startup] Tentativa {Tentativa}/{Max} de conectar no Oracle...",
                        tentativa, maxTentativas);

                    await db.Database.MigrateAsync();
                    await CarregarDadosIniciaisAsync(db);

                    logger.LogInformation(
                        "[startup] Banco pronto: migrations aplicadas e carga inicial concluida.");
                    return;
                }
                catch (Exception ex) when (tentativa < maxTentativas)
                {
                    logger.LogWarning(
                        "[startup] Falha na tentativa {Tentativa}: {Mensagem}. Nova tentativa em {Espera}s.",
                        tentativa, ex.Message, esperaSegundos);

                    await Task.Delay(TimeSpan.FromSeconds(esperaSegundos));
                }
            }
        }

        /// <summary>
        /// </summary>
        private static async Task CarregarDadosIniciaisAsync(AppDbContext db)
        {
            if (await db.Tutores.AnyAsync()) return;

            var tutor1 = new Tutor
            {
                NmTutor = "Murillo Fernandes Carapia",
                Cpf = "123.456.789-00",
                Email = "murillo@petcare360.com.br",
                Telefone = "(11) 99999-0001",
                Endereco = "Av. Lins de Vasconcelos, 1222 - Cambuci, Sao Paulo/SP"
            };

            var tutor2 = new Tutor
            {
                NmTutor = "Ana Beatriz Souza",
                Cpf = "987.654.321-00",
                Email = "ana.souza@petcare360.com.br",
                Telefone = "(11) 99999-0002",
                Endereco = "R. Augusta, 1500 - Consolacao, Sao Paulo/SP"
            };

            var clinica1 = new Clinica
            {
                NmClinica = "CLYVO VET - Unidade Paulista",
                Cnpj = "12.345.678/0001-99",
                Endereco = "Av. Paulista, 1500 - Bela Vista, Sao Paulo/SP",
                Telefone = "(11) 3000-1000",
                Email = "paulista@clyvovet.com.br"
            };

            var clinica2 = new Clinica
            {
                NmClinica = "PetCare 360 - Unidade Vila Mariana",
                Cnpj = "98.765.432/0001-11",
                Endereco = "R. Domingos de Morais, 800 - Vila Mariana, Sao Paulo/SP",
                Telefone = "(11) 3000-2000",
                Email = "vilamariana@petcare360.com.br"
            };

            db.Tutores.AddRange(tutor1, tutor2);
            db.Clinicas.AddRange(clinica1, clinica2);
            await db.SaveChangesAsync();

            var pet1 = new Pet
            {
                NmPet = "Rex",
                Especie = "Cachorro",
                Raca = "Labrador Retriever",
                DtNascimento = new DateTime(2020, 5, 10),
                Peso = 28.50m,
                IdTutor = tutor1.IdTutor
            };

            var pet2 = new Pet
            {
                NmPet = "Mel",
                Especie = "Gato",
                Raca = "Siames",
                DtNascimento = new DateTime(2022, 8, 3),
                Peso = 4.20m,
                IdTutor = tutor2.IdTutor
            };

            db.Pets.AddRange(pet1, pet2);
            await db.SaveChangesAsync();

            var consulta1 = new Consulta
            {
                DtConsulta = new DateTime(2026, 3, 12, 9, 30, 0),
                Descricao = "Consulta de rotina anual com aplicacao de vacina",
                Diagnostico = "Animal saudavel, peso adequado para a raca",
                IdPet = pet1.IdPet,
                IdClinica = clinica1.IdClinica
            };

            var consulta2 = new Consulta
            {
                DtConsulta = new DateTime(2026, 4, 2, 14, 0, 0),
                Descricao = "Retorno por episodios de vomito e apatia",
                Diagnostico = "Gastrite alimentar leve, tratamento medicamentoso por 7 dias",
                IdPet = pet2.IdPet,
                IdClinica = clinica2.IdClinica
            };

            db.Consultas.AddRange(consulta1, consulta2);
            await db.SaveChangesAsync();

            db.Vacinas.AddRange(
                new Vacina
                {
                    NmVacina = "V10 (Polivalente Canina)",
                    Fabricante = "Zoetis",
                    DtAplicacao = new DateTime(2026, 3, 12),
                    DtProximaDose = new DateTime(2027, 3, 12),
                    IdPet = pet1.IdPet,
                    IdConsulta = consulta1.IdConsulta
                },
                new Vacina
                {
                    NmVacina = "Antirrabica Felina",
                    Fabricante = "MSD Saude Animal",
                    DtAplicacao = new DateTime(2026, 4, 2),
                    DtProximaDose = new DateTime(2027, 4, 2),
                    IdPet = pet2.IdPet,
                    IdConsulta = consulta2.IdConsulta
                });

            db.Medicamentos.AddRange(
                new Medicamento
                {
                    NmMedicamento = "Vermifugo Drontal Plus",
                    Dosagem = "1 comprimido para cada 10 kg",
                    Frequencia = "Dose unica, repetir em 15 dias",
                    DtInicio = new DateTime(2026, 3, 12),
                    DtFim = new DateTime(2026, 3, 27),
                    IdPet = pet1.IdPet,
                    IdConsulta = consulta1.IdConsulta
                },
                new Medicamento
                {
                    NmMedicamento = "Omeprazol 10mg",
                    Dosagem = "1 comprimido ao dia",
                    Frequencia = "A cada 24 horas, em jejum",
                    DtInicio = new DateTime(2026, 4, 2),
                    DtFim = new DateTime(2026, 4, 9),
                    IdPet = pet2.IdPet,
                    IdConsulta = consulta2.IdConsulta
                });

            await db.SaveChangesAsync();
        }
    }
}