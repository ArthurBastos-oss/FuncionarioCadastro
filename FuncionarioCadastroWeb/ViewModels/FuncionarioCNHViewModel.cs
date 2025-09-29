using System.ComponentModel.DataAnnotations;

namespace FuncionarioCadastroWeb.ViewModels
{
    public class FuncionarioCNHViewModel
    {
        public int IdFuncionario { get; set; }
        [Required(ErrorMessage = "Declare o número da sua CNH")]
        public string CNH { get; set; }

        [Required(ErrorMessage = "Selecione pelo menos uma categoria.")]
        public string[] Categoria { get; set; } = Array.Empty<string>();

        [Required(ErrorMessage = "A data de emissão não é valida.")]
        public DateTime? DataEmissao { get; set; }

        [Required(ErrorMessage = "A data de validade não é valida.")]
        public DateTime? Validade { get; set; }
        //public FuncionarioViewModel Funcionario { get; set; }
    }
}
