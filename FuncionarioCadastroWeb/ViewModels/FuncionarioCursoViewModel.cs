using System.ComponentModel.DataAnnotations;

namespace FuncionarioCadastroWeb.ViewModels
{
    public class FuncionarioCursoViewModel
    {
        public int Id { get; set; }
        public int IdFuncionario { get; set; }

        [Required(ErrorMessage = "Deve ser registrado pelo menos 1 Curso.")]
        public string Curso { get; set; }

        [Required(ErrorMessage = "O tipo de curso feito é obrigatório.")]
        public string TipoCurso { get; set; }
        //public FuncionarioViewModel Funcionario { get; set; }
    }
}
