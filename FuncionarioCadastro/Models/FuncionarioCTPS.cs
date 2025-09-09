namespace FuncionarioCadastro.Models
{
    public class FuncionarioCTPS
    {
        public int IdFuncionario { get; set; }
        public string CTPS { get; set; }
        public string Tipo { get; set; }
        public DateTime DataEmissao { get; set; }
        public Funcionario Funcionario { get; set; }

    }
}
