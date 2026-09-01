using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare360.Domain.Entities
{
    [Table("TB_TUTOR")]
    public class Tutor
    {
        [Key]
        [Column("ID_TUTOR")]
        public int IdTutor { get; set; }

        [Required(ErrorMessage = "O nome do tutor é obrigatório")]
        [MaxLength(100)]
        [Column("NM_TUTOR")]
        public string NmTutor { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [MaxLength(14)]
        [Column("CPF")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [MaxLength(100)]
        [Column("EMAIL")]
        public string Email { get; set; }

        [MaxLength(20)]
        [Column("TELEFONE")]
        public string Telefone { get; set; }

        [MaxLength(200)]
        [Column("ENDERECO")]
        public string Endereco { get; set; }

        //Relacionamento 1:N - um tutor pode ter vários pets
        public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    }
}