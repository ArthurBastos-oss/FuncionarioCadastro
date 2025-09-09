namespace FuncionarioCadastroWeb.ViewModels
{
    public class FuncionarioCNHViewModel
    {
        public int Id { get; set; }
        public int IdFuncionario { get; set; }
        public string CNH { get; set; }
        public string Categoria { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime Validade { get; set; }
        public FuncionarioViewModel Funcionario { get; set; }
    }
}
