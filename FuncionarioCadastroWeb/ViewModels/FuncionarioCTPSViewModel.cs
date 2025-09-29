using System.ComponentModel.DataAnnotations;

namespace FuncionarioCadastroWeb.ViewModels
{
    public class FuncionarioCTPSViewModel
    {
        public int IdFuncionario { get; set; }

        [Required(ErrorMessage = "O CTPS é obrigatório.")]
        public string CTPS { get; set; }

        [Required(ErrorMessage = "O tipo de CTPS é obrigatório.")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "A data de emissão da CTPS é obrigatório.")]
        public DateTime? DataEmissao { get; set; }
        //public FuncionarioViewModel Funcionario { get; set; }
    }
}
