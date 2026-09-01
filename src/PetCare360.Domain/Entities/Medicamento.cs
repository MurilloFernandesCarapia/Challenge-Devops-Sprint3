using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare360.Domain.Entities
{
    [Table("TB_MEDICAMENTO")]
    public class Medicamento
    {
        [Key]
        [Column("ID_MEDICAMENTO")]
        public int IdMedicamento { get; set; }

        [Required(ErrorMessage = "O nome do medicamento é obrigatório")]
        [MaxLength(100)]
        [Column("NM_MEDICAMENTO")]
        public string NmMedicamento { get; set; }

        [MaxLength(50)]
        [Column("DOSAGEM")]
        public string Dosagem { get; set; }

        [MaxLength(100)]
        [Column("FREQUENCIA")]
        public string Frequencia { get; set; }

        [Required(ErrorMessage = "A data de início é obrigatória")]
        [Column("DT_INICIO")]
        public DateTime DtInicio { get; set; }

        [Column("DT_FIM")]
        public DateTime? DtFim { get; set; }

        //chave estrangeira para o pet
        [Required]
        [Column("ID_PET")]
        public int IdPet { get; set; }

        [ForeignKey("IdPet")]
        public Pet? Pet { get; set; }

        //chave estrangeira para a consulta
        [Column("ID_CONSULTA")]
        public int? IdConsulta { get; set; }

        [ForeignKey("IdConsulta")]
        public Consulta? Consulta { get; set; }
    }
}