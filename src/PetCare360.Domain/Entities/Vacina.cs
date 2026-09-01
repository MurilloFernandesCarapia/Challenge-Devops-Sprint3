using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare360.Domain.Entities
{
    [Table("TB_VACINA")]
    public class Vacina
    {
        [Key]
        [Column("ID_VACINA")]
        public int IdVacina { get; set; }

        [Required(ErrorMessage = "É obrigatório o nome da vacina")]
        [MaxLength(100)]
        [Column("NM_VACINA")]
        public string NmVacina { get; set; }

        [MaxLength(100)]
        [Column("FABRICANTE")]
        public string? Fabricante { get; set; }

        [Required(ErrorMessage = "É obrigatório a data de aplicação")]
        [Column("DT_APLICACAO")]
        public DateTime DtAplicacao { get; set; }

        [Column("DT_PROXIMA_DOSE")]
        public DateTime? DtProximaDose { get; set; }

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