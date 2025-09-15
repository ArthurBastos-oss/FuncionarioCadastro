namespace FuncionarioCadastroWeb.Servicos
{
    public static class SvcFormatacao
    {
        public static string MascaraCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return string.Empty;

            cpf = new string(cpf.Where(char.IsDigit).ToArray()); // remove caracteres não numéricos

            if (cpf.Length != 11) return cpf; // retorna sem formatar se não tiver 11 dígitos

            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }

        public static string MascaraCEP(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep)) return string.Empty;

            cep = new string(cep.Where(char.IsDigit).ToArray());

            if (cep.Length != 8) return cep;

            return Convert.ToUInt64(cep).ToString(@"00000\-000");
        }
    }
}
