using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare360.Domain.Entities
{
    [Table("TB_CONSULTA")]
    public class Consulta
    {
        [Key]
        [Column("ID_CONSULTA")]
        public int IdConsulta { get; set; }

        [Required(ErrorMessage = "A data da consulta é obrigatória")]
        [Column("DT_CONSULTA")]
        public DateTime DtConsulta { get; set; }

        [MaxLength(500)]
        [Column("DESCRICAO")]
        public string Descricao { get; set; }

        [MaxLength(500)]
        [Column("DIAGNOSTICO")]
        public string Diagnostico { get; set; }

        //Chave estrangeira para o pet
        [Required]
        [Column("ID_PET")]
        public int IdPet { get; set; }

        [ForeignKey("IdPet")]
        public Pet? Pet { get; set; }

        //Chave estrangeira para a clínica
        [Required]
        [Column("ID_CLINICA")]
        public int IdClinica { get; set; }

        [ForeignKey("IdClinica")]
        public Clinica? Clinica { get; set; }

        //Coleções derivadas da consulta
        public ICollection<Vacina> Vacinas { get; set; } = new List<Vacina>();
        public ICollection<Medicamento> Medicamentos { get; set; } = new List<Medicamento>();
    }
}