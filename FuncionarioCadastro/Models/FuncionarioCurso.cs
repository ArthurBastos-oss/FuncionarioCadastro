namespace FuncionarioCadastro.Models
{
    public class FuncionarioCurso
    {
        public int Id { get; set; }
        public int IdFuncionario { get; set; }
        public string Curso { get; set; }
        public string TipoCurso { get; set; }
        public Funcionario Funcionario { get; set; }
    }
}
