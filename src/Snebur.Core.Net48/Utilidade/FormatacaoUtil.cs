using Snebur.Dominio;
using Snebur.UI;
using System;
using System.Globalization;
using System.IO;

namespace Snebur.Utilidade
{
    public static partial class FormatacaoUtil
    {
        private const int CASAS_DECIMAL_PADRAO = 2;
        public static string Formatar(object valor, EnumFormatacao formatar)
        {
            if (valor == null)
            {
                return String.Empty;
            }

            switch (formatar)
            {
                case EnumFormatacao.Bytes:

                    if (valor is long || valor is int)
                    {
                        return FormatarBytes((long)valor);
                    }
                    return FormatarBytes(valor.ToString());

                case EnumFormatacao.Cep:
                    return FormatarCep(valor.ToString());
                case EnumFormatacao.Cpf:
                case EnumFormatacao.Cnpj:

                    return FormatarCpfCnpj(valor.ToString());

                case EnumFormatacao.Telefone:
                    return FormatarTelefone(valor.ToString());
                case EnumFormatacao.Moeda:
                    return FormatarMoeda(ConverterUtil.ParaDecimal(valor));

                case EnumFormatacao.MoedaComSinal:
                case EnumFormatacao.Inteiro:
                    return Convert.ToInt32(valor).ToString();

                case EnumFormatacao.Decimal:

                    return FormatarDecimal(ConverterUtil.ParaDecimal(valor));
                case EnumFormatacao.Decimal1:
                    return FormatarDecimal(ConverterUtil.ParaDecimal(valor), 1);
                case EnumFormatacao.Decimal3:
                    return FormatarDecimal(ConverterUtil.ParaDecimal(valor), 3);
                case EnumFormatacao.Data:
                    return FormatarData(valor, 3);
                case EnumFormatacao.Hora:
                    return FormatarHora(valor, 3);
                case EnumFormatacao.Dimensao:
                    return FormatarDimensao(ConverterUtil.ParaDimensao(valor));
                case EnumFormatacao.DimensaoCm:
                    return FormatarDimensaoCm(ConverterUtil.ParaDimensao(valor));
                case EnumFormatacao.DimensaoPixels:
                    return FormatarDimensaoPixels(ConverterUtil.ParaDimensao(valor));
                case EnumFormatacao.DataHora:

                case EnumFormatacao.HoraDescricao:

                case EnumFormatacao.HoraDescricaoMin:
                    break;
                case EnumFormatacao.DataSemantica:
                    break;
                case EnumFormatacao.DataHoraSemantica:
                    break;
                case EnumFormatacao.DataSemanticaHora:
                    break;
                case EnumFormatacao.SimNao:
                    break;
                case EnumFormatacao.Trim:
                    break;
                case EnumFormatacao.TamanhoArquivo:
                    break;
                case EnumFormatacao.Porcentagem:
                    break;
                case EnumFormatacao.Porcentagem1:
                    break;
                case EnumFormatacao.Porcentagem2:
                    break;
                case EnumFormatacao.NaoQuebrar:
                    break;
                case EnumFormatacao.Pixel:
                    break;
                case EnumFormatacao.Tempo:
                    break;
                case EnumFormatacao.TempoSemantico:
                    break;
                case EnumFormatacao.Centimetro:
                    break;
                case EnumFormatacao.Grau:
                    break;
                case EnumFormatacao.Grau1:
                    break;
                case EnumFormatacao.Grau2:
                    break;
                case EnumFormatacao.PrimeiraLetraMaiuscula:
                    break;
                case EnumFormatacao.CaixaAlta:
                    break;
                case EnumFormatacao.CaixaBaixa:
                    break;
                case EnumFormatacao.Nome:
                    break;
                case EnumFormatacao.PositivoNegativo:
                    break;
                case EnumFormatacao.PositivoNegativoDecimal:
                    break;
                case EnumFormatacao.PortentagemPositivoNegativo:
                    break;
                case EnumFormatacao.Portentagem1PositivoNegativo:
                    break;
                case EnumFormatacao.Absoluto:
                    break;
                case EnumFormatacao.EntreParentes:
                    break;
                case EnumFormatacao.QuebrarLinhasHtml:
                    break;
                case EnumFormatacao.Peso:
                    break;
                case EnumFormatacao.Prazo:
                    break;
                case EnumFormatacao.Proteger:
                    return Proteger(valor.ToString());

                default:
                    break;
            }

            throw new NotImplementedException();
        }

        public static string FormatarCep(string cep)
        {
            var cepNumeros = TextoUtil.RetornarSomenteNumeros(cep);
            return String.Format("{0:00\\.000-000}", Convert.ToInt64(cepNumeros));

        }

        private static string FormatarData(object v1, int v2)
        {
            throw new NotImplementedException();
        }

        private static string FormatarHora(object v1, int v2)
        {
            throw new NotImplementedException();
        }

        private static string FormatarDimensaoCm(Dimensao valor)
        {
            throw new NotImplementedException();
        }

        public static string FormatarBytes(string totalBytes)
        {
            if (Int64.TryParse(totalBytes, out var bytes))
            {
                return FormatarByteUtil.Formatar(bytes);
            }
            return totalBytes;
        }

        public static string FormatarBytes(long totalBytes)
        {
            return FormatarByteUtil.Formatar(totalBytes);
        }

        public static string FormatarBytes(long totalBytes, EnumFormatacaoBytes formato)
        {
            return FormatarByteUtil.Formatar(totalBytes, formato);
        }

        public static string FormatarTipoQualificado(string nomeTipoQualificado)
        {
            var posicao = nomeTipoQualificado.IndexOf(", Version=");
            return nomeTipoQualificado.Substring(0, posicao);
        }

        public static string FormatarNomeArquivoParaWeb(string nomeArquivo, int maximoCaracteres = 35)
        {
            if (maximoCaracteres < 20)
            {
                throw new ErroNaoSuportado(String.Format("O máximo de caracteres deve ser superior a 20"));
            }
            var nomeArquivoFormatado = nomeArquivo;
            //var nomeArquivoAtual = nomeArquivoFormatado.ToLower();

            while (nomeArquivoFormatado.Contains("  "))
            {
                nomeArquivoFormatado = nomeArquivoFormatado.Replace("  ", " ");
            }
            nomeArquivoFormatado = nomeArquivoFormatado.Replace(" ", "_");
            nomeArquivoFormatado = TextoUtil.RemoverAcentos(nomeArquivoFormatado);
            nomeArquivoFormatado = TextoUtil.RemoverCaracteresEspecial(nomeArquivoFormatado, '_');

            nomeArquivoFormatado = TextoUtil.RemoverCaracteres(nomeArquivoFormatado, Path.GetInvalidFileNameChars());

            if (nomeArquivoFormatado.Length > maximoCaracteres)
            {
                nomeArquivoFormatado = String.Format("{0}...{1}", nomeArquivoFormatado.Substring(0, maximoCaracteres - 15),
                                                                  nomeArquivoFormatado.Substring(nomeArquivoFormatado.Length - 10, 10));
            }
            nomeArquivoFormatado = nomeArquivoFormatado.Trim();
            return nomeArquivoFormatado;
        }

        public static string FormatarNome(string nome)
        {
            return FormatacaoNomeUtil.FormatarNome(nome);
        }
        public static (string, string) FormatarNomeSobreNome(string nomeCompleto)
        {
            return FormatacaoNomeUtil.FormatarNomeSobrenome(nomeCompleto);
        }

        public static string FormatarMoeda(decimal valorPedidoInterface)
        {
            return String.Format(ConfiguracaoUtil.CulturaPortuguesBrasil, "{0:C}", valorPedidoInterface);
        }

        public static string FormatarDimensao(Dimensao dimensao)
        {
            return $"{dimensao.Largura.ToString("0.0")} x {dimensao.Altura.ToString("0.0")}";
        }

        public static string FormatarDimensaoPixels(Dimensao dimensao)
        {
            return $"{dimensao.Largura.ToString("0")} x {dimensao.Altura.ToString("0")}";
        }

        public static string FormatarMilimetros(double espessura)
        {
            return $"{espessura.ToString("0.0")} mm";
        }

        public static string FormatarCpfCnpj(string cpfCnpj, bool isIgnoarErro = false)
        {
            if (String.IsNullOrWhiteSpace(cpfCnpj))
            {
                return String.Empty;
            }

            var cpfCnpjNumeros = TextoUtil.RetornarSomenteNumeros(cpfCnpj);
            if (cpfCnpjNumeros.Length == 11)
            {
                return String.Format(@"{0:000\.000\.000-00}", Convert.ToInt64(cpfCnpjNumeros));
            }

            if (cpfCnpjNumeros.Length == 14)
            {
                return String.Format(@"{0:00\.000\.000/0000-00}", Convert.ToInt64(cpfCnpjNumeros));
            }

            if (isIgnoarErro)
            {
                return cpfCnpjNumeros;
            }
            throw new Exception(String.Format("O valor {0} não está em um formato de CPF ou CNPJ válido.", cpfCnpj));
        }

        public static string FormatarTelefone(string telefone)
        {
            var somenteNumeros = TextoUtil.RetornarSomenteNumeros(telefone);
            if (somenteNumeros.Length == 10)
            {
                return String.Format("{0:(##) ####-####}", somenteNumeros);
            }
            if (somenteNumeros.Length == 11)
            {
                return String.Format("{0:(##) #####-####}", somenteNumeros);
            }
            return telefone;
        }

        public static string FormatarRota(string nome)
        {
            var rota = TextoUtil.RetornarSomentesLetrasNumeros(nome.Trim(), true);
            rota = rota.Replace(" ", "-").ToLower();

            while (rota.Contains("--"))
            {
                rota = rota.Replace("--", "-");
            }

            if (String.IsNullOrWhiteSpace(rota))
            {
                return "sem-nome";
            }
            return UriUtil.AjustarBarraInicialFinal(rota);
        }

        public static string FormatarCentimetro(double espessura)
        {
            return $"{espessura.ToString("0.0", new CultureInfo("pt-BR"))}cm";
        }

        public static string Proteger(string valor)
        {
            if (valor?.Length > 3)
            {
                return valor.Trim().Substring(0, 3) + "*****"; ;
            }
            return "*****";
        }

        public static string ProtegerEmail(string valor)
        {
            if (valor?.Length > 3 && ValidacaoUtil.IsEmail(valor))
            {
                return valor.Trim().Substring(0, 3) + "*****" + valor.Substring(valor.IndexOf('@'));
            }
            return "*****";
        }

        internal static string FormatarPrimeiraLetraMaiuscula(string x)
        {
            throw new NotImplementedException();
        }
    }

    public enum EnumDivisorDecimal
    {
        CulturaAtual,
        Ponto,
        Virgula
    }
}