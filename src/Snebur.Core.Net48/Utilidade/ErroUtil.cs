using Snebur.AcessoDados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Snebur.Utilidade
{
    public static class ErroUtil
    {
        private static List<string> ReferenciasIgnorar = new List<string> { "System", "Microsoft", "mscorlib", "DotNetZip", "App_Web" };

        public static string RetornarDescricaoCompletaErro(Exception erro,
                                                           string nomeMetodo,
                                                           string caminhoArquivo,
                                                           int linhaDoErro,
                                                           bool incluirReferencias = false)
        {
            var sb = new StringBuilder();
            sb.AppendLine(String.Format("Data hora: {0}", DateTime.Now.ToString()));
            sb.AppendLine(RetornarDescricaoDetalhadaErro(erro));

            if (incluirReferencias)
            {
                sb.AppendLine(" --- REFERENCIAS --");
                sb.AppendLine("");

                var referencias = ReflexaoUtil.RetornarNomesVersaoAssemblies();

                foreach (var referencia in referencias)
                {
                    var nomeReferencia = referencia.Item1;
                    var versao = referencia.Item2;
                    if (!ReferenciasIgnorar.Any(x => nomeReferencia.StartsWith(x)))
                    {
                        sb.AppendLine(String.Format("{0} - {1}", nomeReferencia, versao.ToString()));
                    }
                }
                sb.AppendLine("");
            }
            return TextoUtil.RetornarStringUTF8(sb.ToString());
        }

        public static string RetornarMensagem(Exception erro)
        {
            var sb = new StringBuilder();
            sb.AppendLine(String.Format("Erro: '{0}' : {1}", erro.GetType().Name, erro.Message));
            var erroInterno = erro.InnerException;
            while (erroInterno != null)
            {
                sb.AppendLine(String.Format("Erro interno: '{0}' : {1}", erroInterno.GetType().Name, erroInterno.Message));
                erroInterno = erroInterno.InnerException;
            }
            return sb.ToString();
        }

        public static string RetornarDescricaoDetalhadaErro(Exception _erro)
        {
            var sb = new StringBuilder();
            var erroAtual = _erro;
            while (erroAtual != null)
            {
                sb.AppendLine("");
                var isExisteInterno = erroAtual != _erro;
                if (isExisteInterno)
                {
                    sb.AppendLine("----------------------ERRO INTERNO ---------------------");
                }
                sb.AppendLine($"Erro : '{erroAtual.GetType().Name}' : {erroAtual.Message}");

                if (erroAtual is Erro erroTipado)
                {
                    if (erroTipado.IsExisteNomeMetodo)
                    {
                        sb.AppendLine($"Nome do método: '{erroTipado.NomeMetodo}'");
                    }
                    if (erroTipado.IsExisteCaminhoArquivo)
                    {
                        sb.AppendLine($"Caminho do arquivo: '{erroTipado.CaminhoArquivo}'");
                    }
                    if (erroTipado.IsExisteLinhaDoErro)
                    {
                        sb.AppendLine($"Caminho do arquivo: '{erroTipado.LinhaDoErro}'");
                    }
                }
                if (!String.IsNullOrEmpty(erroAtual.StackTrace))
                {
                    sb.AppendLine("---------------------- StackTrace --------------------------");
                    sb.AppendLine($"{erroAtual.StackTrace}");
                    sb.AppendLine("------------------------------------------------------------");
                }
                sb.AppendLine("");
                erroAtual = erroAtual.InnerException;
            }
            return sb.ToString();
        }

        public static bool IsTipo<T>(Exception ex)
        {
            while (ex != null)
            {
                if (ex is T)
                {
                    return true;
                }
                if (!ReferenceEquals(ex, ex.InnerException))
                {
                    ex = ex.InnerException;
                }
            }
            return false;
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

        public static void ValidarStringVazia(string texto, string nomeReferencia, [CallerMemberName] string nomeMetodo = "", [CallerFilePath] string caminhoArquivo = "", [CallerLineNumber] int linhaDoErro = 0)
        {
            if (String.IsNullOrEmpty(texto))
            {
                var mensagem = String.Format("A referencia '{0}' não foi definida", nomeReferencia);
                throw new ErroNaoDefinido(mensagem, null, nomeMetodo, caminhoArquivo, linhaDoErro);
            }
        }

        public static bool IsErroSessaoInvalida(Exception ex)
        {
            return IsTipo<ErroSessaoUsuarioExpirada>(ex) ||
                   IsTipo<ErroSessaoUsuarioInvalida>(ex);
        }
    }
}