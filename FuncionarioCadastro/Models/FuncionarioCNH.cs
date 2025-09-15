namespace FuncionarioCadastro.Models
{
    public class FuncionarioCNH
    {
        public int IdFuncionario { get; set; }
        public string CNH { get; set; }
        public string Categoria { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime Validade { get; set; }
        public Funcionario Funcionario { get; set; }
    }
}
