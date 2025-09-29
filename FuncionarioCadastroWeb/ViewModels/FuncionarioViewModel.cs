using FuncionarioCadastro.Models;
using System.ComponentModel.DataAnnotations;

namespace FuncionarioCadastroWeb.ViewModels
{
    public class FuncionarioViewModel
    {
        public FuncionarioViewModel() 
        {
            CNH = new FuncionarioCNHViewModel();
            CTPS = new FuncionarioCTPSViewModel();
        }
        public int? Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Data de Nascimento é obrigatório.")]
        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        //[RegularExpression(@"\d{11}", ErrorMessage = "O CPF deve ter 11 dígitos.")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "A UF é obrigatório.")]
        public string UF { get; set; }

        [Required(ErrorMessage = "A Profissão é obrigatório.")]
        public string Profissao { get; set; }
        public FuncionarioCNHViewModel CNH { get; set; }
        public FuncionarioCTPSViewModel CTPS { get; set; }
        public List<FuncionarioCursoViewModel> Curso { get; set; } = new();
        public List<FuncionarioEnderecoViewModel> Endereco { get; set; } = new();
    }
}
