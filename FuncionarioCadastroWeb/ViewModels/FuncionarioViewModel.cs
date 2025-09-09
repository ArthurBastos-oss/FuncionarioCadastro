using FuncionarioCadastro.Models;

namespace FuncionarioCadastroWeb.ViewModels
{
    public class FuncionarioViewModel
    {
        public FuncionarioViewModel() 
        {
            CNH = new FuncionarioCNHViewModel();
            CTPS = new FuncionarioCTPSViewModel();
            //Curso = new FuncionarioCursoViewModel();
            //Endereco = new FuncionarioEnderecoViewModel();
        }
        public string Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string CPF { get; set; }
        public string UF { get; set; }
        public string Profissao { get; set; }
        public FuncionarioCNHViewModel CNH { get; set; }
        public FuncionarioCTPSViewModel CTPS { get; set; }
        public List<FuncionarioCursoViewModel> Curso { get; set; } = new();
        public List<FuncionarioEnderecoViewModel> Endereco { get; set; } = new();
    }
}
