using System;

namespace Snebur.Utilidade
{
    public static class CnpjUtil
    {
        private static int[] sequencia1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        private static int[] sequencia2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        public static string CalcularDV(string cnpjBase)
        {
            // Função para calcular dígito verificador
            string CalcularDigito(string baseCNPJ, int[] sequencia)
            {
                int soma = 0;
                for (int i = 0; i < baseCNPJ.Length; i++)
                {
                    soma += (baseCNPJ[i] - '0') * sequencia[i]; // '0' para converter char para int
                }
                int resto = soma % 11;
                return (resto < 2) ? "0" : (11 - resto).ToString();
            }

            // Calcula o primeiro dígito verificador
            string primeiroDigito = CalcularDigito(cnpjBase, sequencia1);

            // Calcula o segundo dígito verificador
            string segundoDigito = CalcularDigito(cnpjBase + primeiroDigito, sequencia2);

            return primeiroDigito + segundoDigito;
        }

        public static string GenerateFakeCnpj()
        {
            var random = new Random();
            var cnpjBase = random.Next(10000000, 99999999).ToString("00000000") +
                           random.Next(1000, 9999).ToString("0000");
            return cnpjBase + CalcularDV(cnpjBase); 
        }
    }
}