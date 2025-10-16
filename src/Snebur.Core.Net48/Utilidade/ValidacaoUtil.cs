using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Snebur.Utilidade
{
    public static partial class ValidacaoUtil
    {
        private static readonly Regex RegexHasSpace = new Regex(@"\s", RegexOptions.Compiled);
        private static readonly Regex RegexValidacaoEmail = new Regex(@"^[a-zA-Z0-9][a-zA-Z0-9\\._-]+@([a-zA-Z0-9\._-]+\.)[a-zA-Z-0-9]{2}", RegexOptions.Compiled);
        private static readonly Regex RegexCorHexa = new Regex("^#+([a-fA-F0-9]{6}|[a-fA-F0-9]{3}|[a-fA-F0-9]{8})$", RegexOptions.Compiled);
        private static readonly Regex RegexCorRgba = new Regex(@"^rgba?\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*(?:,\s*(\d+(?:\.\d+)?)\s*)?\)$", RegexOptions.Compiled);
        private static readonly Regex RegexMd5 = new Regex("^[a-f0-9]{32}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexSha1 = new Regex("^[a-f0-9]{40}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexSha256 = new Regex("^[a-f0-9]{64}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexGuid = new Regex("^[a-f0-9]{8}(-?[a-f0-9]{4}){3}-?[a-f0-9]{12}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexCep = new Regex(@"^\d{5}-?\d{3}$", RegexOptions.Compiled);
        public static List<ValidationResult> RetornarPendencias(object instancia)
        {
            var pendencias = new List<ValidationResult>();
            Validator.TryValidateObject(instancia, new ValidationContext(instancia, null, null), pendencias, true);
            return pendencias;
        }

        public static bool IsFlagsEnumDefinida<TEnum>(Enum soma)
        {
            return IsFlagsEnumDefinida(typeof(TEnum), soma);
        }

        public static bool IsFlagsEnumDefinida(Type tipoEnum, Enum soma)
        {
            return EnumUtil.IsFlagsEnumDefinida(tipoEnum, soma);
        }

        public static bool IsEmailOuTelefone(string emailOuTelefone)
        {
            return IsEmail(emailOuTelefone) || IsTelefone(emailOuTelefone);
        }

        public static bool IsEmail(string email)
        {
            if (!String.IsNullOrEmpty(email))
            {
                if (RegexHasSpace.IsMatch(email))
                {
                    return false;
                }
                return RegexValidacaoEmail.IsMatch(email.Trim());
            }
            return false;
        }

        public static bool IsCep(string cep)
        {
            cep = cep?.Trim();
            if (!String.IsNullOrEmpty(cep))
            {
                if (RegexCep.IsMatch(cep))
                {
                    return true;
                }
                var numeros = TextoUtil.RetornarSomenteNumeros(cep);
                return (numeros.Length == 5 || numeros.Length == 8);
            }
            return false;
        }

        public static bool IsCnpj(string cnpj)
        {
            if (!String.IsNullOrWhiteSpace(cnpj))
            {
                cnpj = TextoUtil.RetornarSomenteNumeros(cnpj);
                if (cnpj.Length < 14)
                {
                    return false;
                }
                if (cnpj == "00000000000000" || cnpj == "11111111111111" || cnpj == "22222222222222" || cnpj == "33333333333333" || cnpj == "44444444444444" || cnpj == "55555555555555"
                           || cnpj == "66666666666666" || cnpj == "77777777777777" || cnpj == "88888888888888" || cnpj == "99999999999999")
                {
                    return false;
                }
                var tamanho = cnpj.Length - 2;
                var numeros = cnpj.Substring(0, tamanho);
                var digitos = cnpj.Substring(tamanho);
                var soma = 0;
                var pos = tamanho - 7;

                for (var i = tamanho; i >= 1; i--)
                {
                    soma += Int32.Parse(numeros[tamanho - i].ToString()) * pos--;
                    if (pos < 2)
                    {
                        pos = 9;
                    }
                }
                var resultado = soma % 11 < 2 ? 0 : 11 - (soma % 11);
                if (resultado != Int32.Parse(digitos[0].ToString()))
                {
                    return false;
                }
                tamanho += 1;
                numeros = cnpj.Substring(0, tamanho);
                soma = 0;
                pos = tamanho - 7;

                for (var i = tamanho; i >= 1; i--)
                {
                    soma += Int32.Parse(numeros[tamanho - i].ToString()) * pos--;
                    if (pos < 2)
                    {
                        pos = 9;
                    }
                }
                resultado = soma % 11 < 2 ? 0 : 11 - (soma % 11);
                if (resultado != Int32.Parse(digitos[1].ToString()))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool IsTelefone(string telefone)
        {
            if (!String.IsNullOrEmpty(telefone))
            {
                var letras = TextoUtil.RetornarSomenteLetras(telefone);
                var numeros = TextoUtil.RetornarSomenteNumeros(telefone);

                numeros = TextoUtil.RemoverCaracteresInicial(numeros, "0");

                var isNacional = (letras.Length == 0 && (numeros.Length == 10 ||
                                                       numeros.Length == 11));
                if (isNacional)
                {
                    var ddd = Convert.ToInt32(numeros.Substring(0, 2));
                    return TelefoneUtil.DicionariosDDD.ContainsKey(ddd);
                }
                //var isInternacional = (letras.Length == 0 && (numeros.Length == 10 ||
                //                                              numeros.Length == 11));
                return false;
            }
            return false;
        }

        public static bool IsCpf(string cpf)
        {
            if (String.IsNullOrWhiteSpace(cpf))
            {
                return false;
            }
            cpf = TextoUtil.RetornarSomenteNumeros(cpf);
            if (cpf.Length != 11)
            {
                return false;
            }
            var primeiroNumero = Convert.ToChar(cpf.Substring(0, 1));
            if (cpf.All(x => x == primeiroNumero))
            {
                return false;
            }
            var soma = 0;
            int resto;
            for (var i = 1; i <= 9; i++)
            {
                soma += Convert.ToInt32(cpf.Substring(i - 1, 1)) * (11 - i);
            }
            resto = soma * 10 % 11;

            if ((resto == 10) || (resto == 11))
            {
                resto = 0;
            }
            if (resto != Convert.ToInt32(cpf.Substring(9, 1)))
            {
                return false;
            }
            soma = 0;

            for (var i = 1; i <= 10; i++)
            {
                soma += Convert.ToInt32(cpf.Substring(i - 1, 1)) * (12 - i);
            }
            resto = soma * 10 % 11;
            if ((resto == 10) || (resto == 11))
            {
                resto = 0;
            }
            if (resto != Convert.ToInt32(cpf.Substring(10, 1)))
            {
                return false;
            }
            return true;
        }

        public static bool IsCpfOuCpj(string cpfOuCnpj)
        {
            var numeros = TextoUtil.RetornarSomenteNumeros(cpfOuCnpj);
            if (numeros.Count() == 11)
            {
                return IsCpf(cpfOuCnpj);
            }
            return IsCnpj(cpfOuCnpj);
        }

        public static bool IsIp(string ip)
        {
            if (ip == IpUtil.Empty)
            {
                return false;
            }
            if (!String.IsNullOrWhiteSpace(ip))
            {
                if (IPAddress.TryParse(ip, out _))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsDominioDns(string dominio)
        {
            var url = new Regex(@"^[\w\-_]+((\.[\w\-_]+)+([a-z]))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (url.IsMatch(dominio))
            {
                return Uri.CheckHostName(dominio) == UriHostNameType.Dns;
            }
            var re = new Regex(@"/^[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9]\.[a-zA-Z]{2,}$/");
            if (re.IsMatch(dominio))
            {
                return Uri.CheckHostName(dominio) == UriHostNameType.Dns;
            }
            return false;
        }
        /// <summary>
        /// Resolve o dns dominio
        /// </summary>
        /// <param name="dominio"></param>
        /// <param name="tempoMaximo"> em milisegundos padrao 3000</param>
        /// <returns></returns>
        public static bool IsResolverDominioDns(string dominio, int tempoMaximo = 3000)
        {
            if (IsDominioDns(dominio))
            {
                try
                {
                    var isValido = false;
                    ThreadUtil.TenteExecute(() =>
                    {
                        var endereco = Dns.GetHostEntry(dominio);
                        isValido = endereco.AddressList.Any(x => x.AddressFamily == AddressFamily.InterNetwork);

                    }, 3000);
                    return isValido;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public static bool IsNumero(object valor)
        {
            return Double.TryParse(valor.ToString(), out var resultado) && !Double.IsNaN(resultado);
        }

        public static bool IsInteiro(object valor)
        {
            return Int32.TryParse(valor.ToString(), out _);
        }

        public static bool IsVersao(string versao)
        {
            if (!String.IsNullOrEmpty(versao))
            {
                var partes = versao.Split('.');
                if (partes.Length == 4)
                {
                    return partes.All(x => Int32.TryParse(x, out var n) && n >= 0);
                }
            }
            return false;
        }

        public static void ValidarIntervalo(double opacidade, double inicio, double fim)
        {
            if (!IsIntervaloValido(opacidade, inicio, fim))
            {
                throw new Erro($"O valor {opacidade} está fora do intervalo {inicio}, {fim}");
            }
        }

        public static void ValidarReferenciaNulaOuVazia(string referencia, string nomeReferencia,
                                                        [CallerMemberName] string nomeMetodo = "",
                                                        [CallerFilePath] string caminhoArquivo = "",
                                                        [CallerLineNumber] int linhaDoErro = 0)
        {
            if (String.IsNullOrWhiteSpace(referencia))
            {
                var mensagem = String.Format("A referencia '{0}' não foi definida", nomeReferencia);
                throw new ErroNaoDefinido(mensagem, null, nomeMetodo, caminhoArquivo, linhaDoErro);
            }
        }

        public static void ValidarReferenciaNula(object referencia, string nomeReferencia,
                                                 [CallerMemberName] string nomeMetodo = "",
                                                 [CallerFilePath] string caminhoArquivo = "",
                                                 [CallerLineNumber] int linhaDoErro = 0)
        {
            if (referencia == null)
            {
                var mensagem = String.Format("A referencia '{0}' não foi definida", nomeReferencia);
                throw new ErroNaoDefinido(mensagem, null, nomeMetodo, caminhoArquivo, linhaDoErro);
            }
        }

        public static void ValidarEnumDefinido<TEnum>(TEnum valorEnum) where TEnum : struct
        {
            var tipoEnum = valorEnum.GetType();
            if (!tipoEnum.IsEnum)
            {
                throw new Erro($"O tipo '{tipoEnum.Name}' não é um Enum");
            }
            if (!Enum.IsDefined(tipoEnum, valorEnum))
            {
                throw new Erro($"O valor '{valorEnum.ToString()}' não está definido no Enum '{tipoEnum.Name}'");
            }
        }

        public static void ValidarExisteArquivo(string caminhoArquivo)
        {
            if (!File.Exists(caminhoArquivo))
            {
                throw new ErroArquivoNaoEncontrado(caminhoArquivo);
            }
        }

        public static bool IsSomenteNumeros(string texto)
        {
            return TextoUtil.IsSomenteNumeros(texto);
        }

        public static bool IsSomenteNumerosPontosSinaisSimbolos(string texto)
        {
            return TextoUtil.IsSomenteNumerosPontosSinaisSimbolos(texto);
        }

        public static bool IsSomenteNumerosPontosSinais(string texto)
        {
            return TextoUtil.IsSomenteNumerosPontosSinais(texto);
        }

        #region Nome

        public static bool IsNomeCompleto(string nomeCompleto)
        {
            if (nomeCompleto != null)
            {
                var (nome, sobrenome) = FormatacaoNomeUtil.FormatarNomeSobrenome(nomeCompleto);

                return !String.IsNullOrEmpty(nome) &&
                       !String.IsNullOrEmpty(sobrenome);

                //var partes = valorPropriedade.Trim().Split(' ');
                //return partes.Count() >= 1 && partes.Where(x => x.Length >= 2).Count() >= 2;
            }
            return false;
        }

        public static bool IsPossuiSobrenome(string nomeCompleto)
        {
            var (nome, sobrenome) = FormatacaoNomeUtil.FormatarNomeSobrenome(nomeCompleto);
            return !String.IsNullOrEmpty(sobrenome);
        }

        public static bool IsPossuiPrimeiroNome(string nomeCompleto)
        {
            var (nome, sobrenome) = FormatacaoNomeUtil.FormatarNomeSobrenome(nomeCompleto);
            var letras = TextoUtil.RetornarSomenteLetras(nome);
            return letras.Length >= 2;
            //return !TextoUtil.IsSomenteNumerosPontosSinais(nome);
        }
        #endregion

        public static bool IsCorHexa(string value)
        {
            if (value != null)
            {
                return RegexCorHexa.IsMatch(value.Trim());
            }
            return false;
        }

        public static bool IsCorRgbOuRgba(string value)
        {
            if (value != null)
            {
                return RegexCorRgba.IsMatch(value.Trim());
            }
            return false;
        }

        public static bool IsRota(string rota)
        {
            if (rota?.StartsWith("/") == true && rota.Length < 512)
            {
                return new Regex("^/[A-Za-z0-9-_/]+/$").IsMatch(rota);
            }
            return false;
        }

        public static bool IsExisteContaEmail(string email)
        {
            return ValidacaoEmailUtil.IsExisteEmail(email);
        }

        public static bool IsMd5(string value)
        {
            if (value != null)
            {
                return RegexMd5.IsMatch(value.Trim());
            }
            return false;
        }

        public static bool IsGuid(string value)
        {
            if (value != null)
            {
                return RegexGuid.IsMatch(value.Trim());
            }
            return false;
        }

        public static bool IsPossuiEspacoEmBranco(string value)
        {
            if (value == null) return false;
            var reg = new Regex(@"\s");
            return reg.IsMatch(value);
        }

        public static void ValidarMaiorZero<T>(T value, string nome) where T : struct, IComparable<T>
        {
            if (!(value.CompareTo(default) > 0))
            {
                throw new Exception($"O valor {nome} deve ser maior que zero");
            }
        }
    }
}