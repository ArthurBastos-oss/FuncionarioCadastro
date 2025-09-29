namespace FuncionarioCadastroWeb.Servicos
{
    public static class SvcValidacao
    {

        // CPF
        public static bool ValidarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return false;

            if (cpf.All(d => d == cpf[0]))
                return false;

            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * (10 - i);

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            if (digito1 != (cpf[9] - '0'))
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * (11 - i);

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return digito2 == (cpf[10] - '0');
        }

        // CNH
        public static bool ValidarCNH(string cnh)
        {
            if (string.IsNullOrWhiteSpace(cnh))
                return false;

            cnh = new string(cnh.Where(char.IsDigit).ToArray());

            if (cnh.Length != 11)
                return false;

            if (cnh.All(d => d == cnh[0]))
                return false;

            int soma = 0;
            for (int i = 0, j = 9; i < 9; i++, j--)
                soma += (cnh[i] - '0') * j;

            int resto = soma % 11;
            int digito1 = resto >= 10 ? 0 : resto;

            if (digito1 != (cnh[9] - '0'))
                return false;

            soma = 0;
            for (int i = 0, j = 1; i < 9; i++, j++)
                soma += (cnh[i] - '0') * j;

            resto = soma % 11;
            int digito2 = resto >= 10 ? 0 : resto;

            return digito2 == (cnh[10] - '0');
        }

        // CEP
        public static bool ValidarCEP(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return false;

            cep = new string(cep.Where(char.IsDigit).ToArray());

            return cep.Length == 8;
        }

        // CTPS
        public static bool ValidarCTPS(string ctps)
        {
            if (string.IsNullOrWhiteSpace(ctps))
                return false;

            var apenasDigitos = new string(ctps.Where(char.IsDigit).ToArray());
            return apenasDigitos.Length == 11;
        }

    }

}
