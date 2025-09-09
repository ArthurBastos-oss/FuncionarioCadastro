namespace FuncionarioCadastro.Models
{
    public class FuncionarioEndereco
    {
        public int Id { get; set; }
        public int IdFuncionario { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string CEP { get; set; }
        public Funcionario Funcionario { get; set; }
    }
}
