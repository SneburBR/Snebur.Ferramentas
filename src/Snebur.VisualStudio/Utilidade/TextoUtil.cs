using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.VisualStudio.Utilidade
{
    public class TextoUtil
    {

        private const string NUMEROS = "0123456789";
        private const string LETRAS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefhgijklmnopqrstuvwxyz";
        private const string CARACTERES_PADRAO = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789ÀÁÂÃÈÉÊÌÍÎÒÓÔÕÚÛÜÇÑàáâãäèéêëìíîòóôõùúûüçñ-_.@ ";

        public static string RemoverAcentos(string texto)
        {
            string[] letras = { "a", "e", "i", "o", "u", "c", "n", "A", "E", "I", "O", "U", "C", "N" };
            string[] acentos = { "(à|á|â|ã|ä){1}", "(è|é|ê|ë){1}", "(ì|í|î|ï){1}", "(ò|ó|ô|õ|ö){1}", "(ù|ú|û|ü){1}", "ç{1}", "ñ{1}", "(À|Á|Â|Ã){1}", "(È|É|Ê){1}", "(Ì|Í|Î){1}", "(Ò|Ó|Ô|Õ){1}", "(Ù|Ú|Û|Ü){1}", "Ç{1}", "Ñ{1}" };

            for (int i = 0; i < letras.Length; i++)
            {
                texto = System.Text.RegularExpressions.Regex.Replace(texto, acentos[i], letras[i]);
            }
            return texto;
        }

        public static string RemoverCaracterEsquisitos(string texto)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < texto.Length; i++)
            {
                var c = texto[i];
                if (TextoUtil.CARACTERES_PADRAO.Contains(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string RetornarSomenteLetras(string texto)
        {
            return TextoUtil.RetornarTextoCaracteresPermitido(texto, LETRAS);
        }

        public static string RetornarSomenteNumeros(string texto)
        {
            return TextoUtil.RetornarTextoCaracteresPermitido(texto, NUMEROS);
        }

        public static string RetornarSomentesLetrasNumeros(string texto, bool espaco = true)
        {
            var caracteres = string.Concat(LETRAS, NUMEROS);
            if (espaco)
            {
                caracteres = string.Concat(caracteres, " ");
            }

            texto = TextoUtil.RemoverAcentos(texto);

            return TextoUtil.RetornarTextoCaracteresPermitido(texto, caracteres);
        }

        public static string RetornarTextoCaracteresPermitido(string texto, string caracterPermitidos)
        {

            var resultado = "";
            for (int i = 0; i < texto.Length; i++)
            {
                var caracter = texto[i];
                if (caracterPermitidos.Contains(caracter))
                {
                    resultado += caracter;
                }

            }
            return resultado;
        }

        public static int RetornaOcorrenciasLetraTexto(string texto, char letra)
        {

            int cont = 0;
            int quant = 0;
            int pos = -1;
            int pos_ant = -1;

            while (cont < texto.Length)
            {
                pos = texto.IndexOf(letra, cont);
                if (pos != pos_ant & pos != -1)
                {
                    quant += 1;
                }
                cont += 1;
                pos_ant = pos;

            }

            return quant;
        }


        public static string RetornarPrimeiroNome(string nomeCompleto)
        {
            var nomes = nomeCompleto.Trim().Split(" ".ToCharArray());
            if (nomes.Any(x => x.Length > 2))
            {
                return TextoUtil.RetornaNomeFormatadoPrimeiraLetraMaiuscula(nomes.Where(x => x.Length > 2).First());
            }
            else
            {
                return nomes.First();
            }
        }

        public static string RetornaNomeFormatadoPrimeiraLetraMaiuscula(string nomeCompleto)
        {

            if (nomeCompleto != null & nomeCompleto.Trim().Length > 0)
            {
                nomeCompleto = nomeCompleto.Trim().ToLower();
                if (nomeCompleto.Contains(" "))
                {
                    var partes = nomeCompleto.Split(" ".ToCharArray());

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (string nome in partes)
                    {

                        if (nome.Trim().Length > 0)
                        {
                            sb.Append(" ");
                            if (nome.Length > 2)
                            {
                                sb.Append(TextoUtil.RetornarPrimeiraLetraMaiuscula(nome));
                            }
                            else if (nome.Length == 2 & nome.Last() == Convert.ToChar("."))
                            {
                                sb.Append(TextoUtil.RetornarPrimeiraLetraMaiuscula(nome));
                            }
                            else if (nome.Length == 1)
                            {
                                sb.Append(TextoUtil.RetornarPrimeiraLetraMaiuscula(nome));
                            }
                            else
                            {
                                sb.Append(nome);
                            }
                        }
                    }
                    return sb.ToString().Trim();
                }
                else
                {
                    return RetornarPrimeiraLetraMaiuscula(nomeCompleto);
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string RetornarPrimeiraLetraMaiuscula(string texto)
        {
            var primeiraLetra = texto.Substring(0, 1).ToUpper();
            var restante = texto.Substring(1, texto.Length - 1);
            return string.Concat(primeiraLetra, restante);
        }

        public static string RetornarPrimeiraLetraMinusculo(string texto)
        {
            //var primeiraLetra = texto.Substring(0, 1).ToLower();
            //var restante = texto.Substring(1, texto.Length - 1);
            //return string.Concat(primeiraLetra, restante);
            return RetornarMinusculo(texto, 0, 1);
        }

        public static string RetornarMinusculo(string texto, int inicio, int fim)
        {
            if (fim > texto.Length)
            {
                fim = texto.Length;
            }

            var parteMinuscula = texto.Substring(0, fim).ToLower();
            var restante = texto.Substring(fim, texto.Length - fim);
            return string.Concat(parteMinuscula, restante);
        }


        public static string RemoverTodosEspacos(string texto)
        {
            throw new NotImplementedException();
            //texto = string.Concat(texto, string.Empty);
            //texto = texto.Replace(Constants.vbCrLf, string.Empty);
            //texto = texto.Replace(Constants.vbCr, string.Empty);
            //texto = texto.Replace(Constants.vbLf, string.Empty);
            //texto = texto.Replace(System.Environment.NewLine, string.Empty);
            //texto = texto.Replace(Constants.vbTab, string.Empty);
            //texto = texto.Replace(" ", string.Empty);
            //return texto;
        }
    }
}
