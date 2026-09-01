using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare360.Domain.Entities
{
    [Table("TB_CLINICA")]
    public class Clinica
    {
        [Key]
        [Column("ID_CLINICA")]
        public int IdClinica { get; set; }

        [Required(ErrorMessage = "O nome da clínica é obrigatório")]
        [MaxLength(150)]
        [Column("NM_CLINICA")]
        public string NmClinica { get; set; }

        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        [MaxLength(18)]
        [Column("CNPJ")]
        public string Cnpj { get; set; }

        [MaxLength(200)]
        [Column("ENDERECO")]
        public string Endereco { get; set; }

        [MaxLength(20)]
        [Column("TELEFONE")]
        public string Telefone { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido")]
        [MaxLength(100)]
        [Column("EMAIL")]
        public string Email { get; set; }

        //Relacionamento 1:N - uma clínica realiza várias consultas
        public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
    }
}