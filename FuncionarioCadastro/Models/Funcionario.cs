namespace FuncionarioCadastro.Models
{
    public class Funcionario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string CPF { get; set; }
        public string UF { get; set; }
        public string Profissao { get; set; }
        public FuncionarioCNH CNH { get; set; }
        public FuncionarioCTPS CTPS { get; set; }
        public List<FuncionarioCurso> Curso { get; set; } = new();
        public List<FuncionarioEndereco> Endereco { get; set; } = new();
    }
}
