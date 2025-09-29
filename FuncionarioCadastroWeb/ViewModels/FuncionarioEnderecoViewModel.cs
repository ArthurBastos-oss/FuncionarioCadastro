using System.ComponentModel.DataAnnotations;

namespace FuncionarioCadastroWeb.ViewModels
{
    public class FuncionarioEnderecoViewModel
    {
        public int Id { get; set; }
        public int IdFuncionario { get; set; }

        [Required(ErrorMessage = "A Cidade é obrigatório.")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "O Bairro é obrigatório.")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "O Logradouro é obrigatório.")]
        public string Logradouro { get; set; }

        [Required(ErrorMessage = "O Número é obrigatório.")]
        public string? Numero { get; set; }
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        public string CEP { get; set; }
        //public FuncionarioViewModel Funcionario { get; set; }
    }
}
