using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare360.Domain.Entities
{
    [Table("TB_PET")]
    public class Pet
    {
        [Key]
        [Column("ID_PET")]
        public int IdPet { get; set; }

        [Required(ErrorMessage = "O nome do pet é obrigatório")]
        [MaxLength(100)]
        [Column("NM_PET")]
        public string NmPet { get; set; }

        [Required(ErrorMessage = "A espécie é obrigatória")]
        [MaxLength(50)]
        [Column("ESPECIE")]
        public string Especie { get; set; }

        [MaxLength(100)]
        [Column("RACA")]
        public string Raca { get; set; }

        [Column("DT_NASCIMENTO")]
        public DateTime? DtNascimento { get; set; }

        [Range(0.1, 500.0, ErrorMessage = "O peso deve estar entre 0,1 e 500 kg")]
        [Column("PESO")]
        public decimal? Peso { get; set; }

        //Chave estrangeira para o tutor
        [Required]
        [Column("ID_TUTOR")]
        public int IdTutor { get; set; }

        [ForeignKey("IdTutor")]
        public Tutor? Tutor { get; set; }

        //Coleções de navegação
        public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
        public ICollection<Vacina> Vacinas { get; set; } = new List<Vacina>();
        public ICollection<Medicamento> Medicamentos { get; set; } = new List<Medicamento>();
    }
}