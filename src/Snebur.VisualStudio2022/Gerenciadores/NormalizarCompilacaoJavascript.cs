using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    internal class NormalizarCompilacaoJavascript : IDisposable
    {
        public const string ABRE_DATA_NORMALIZACAO = "/*DT";
        public const string FECHA_DATA_NORMALIZACAO = "DT*/";
        public const string NORMALIZADO = "/*normalizado*/";
        public const string TICKS = "ticks=";
        private const string EXPRESSAO_WHERE = "Where";
        private const string WHERE_BUFFER = "here";

        private ProjetoTypeScript ProjetoTS;
        private ConfiguracaoProjetoTypeScriptFramework ConfiguracaoTypescript;
        internal static DateTime DATA_NORMALIZACAO_IMPLEMENTADA = new DateTime(2020, 3, 1);
        private readonly Encoding CODIFICACAO = Encoding.UTF8;

        private readonly bool IsNormalizarWhere;

        public NormalizarCompilacaoJavascript(ProjetoTypeScript projetoTS)
        {
            this.ProjetoTS = projetoTS;
            this.ConfiguracaoTypescript = JsonUtil.Deserializar<ConfiguracaoProjetoTypeScriptFramework>(ArquivoUtil.LerTexto(this.ProjetoTS.CaminhoConfiguracao), true);
            this.IsNormalizarWhere = !this.ConfiguracaoTypescript.IsIgnorarNormnalizacaoCompilacao;
        }

        internal void Normalizar()
        {
            try
            {
                var caminhoSaidaAtual = this.ProjetoTS.CaminhoSaidaAtual;
                if (GerenciadorProjetos.DiretorioProjetoTypescriptInicializacao != null)
                {
                    if (!File.Exists(caminhoSaidaAtual) && File.Exists(this.ProjetoTS.CaminhoSaidaPadrao))
                    {
                        this.CopiarArquivosSaia(this.ProjetoTS.CaminhoSaidaPadrao, caminhoSaidaAtual);
                    }
                }

                lock (ProjetoTypeScriptUtil.BloqueioManipuladorArquivos)
                {
                    var t = Stopwatch.StartNew();
                    if (this.RetornarIsNormalizarCompilacao(caminhoSaidaAtual))
                    {
                        this.NornalizarScript(caminhoSaidaAtual);

                        if (!File.Exists(caminhoSaidaAtual))
                        {
                            throw new FileNotFoundException(caminhoSaidaAtual);
                        }
                        else
                        {
                            LogVSUtil.Sucesso("Normalização javascript sucesso " + caminhoSaidaAtual, t);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro($"Falha '{ex.Message}' normalizar compilação javascript  {this.ProjetoTS.NomeProjeto} .");
            }
        }

        private void CopiarArquivosSaia()
        {
            throw new NotImplementedException();
        }
        private void CopiarArquivosSaia(string caminhoScriptOrigem, string caminhoScriptDestino)
        {
            var caminhoDeclaracaoOrigem = Path.ChangeExtension(caminhoScriptOrigem, ".d.ts");
            var caminhoDeclaracaoDestino = Path.ChangeExtension(caminhoScriptDestino, ".d.ts");

            var caminhoDeclaracaoMapOrigem = Path.ChangeExtension(caminhoScriptOrigem, ".d.ts.map");
            var caminhoDeclaracaoMapDestino = Path.ChangeExtension(caminhoScriptDestino, ".d.ts.map");

            if (File.Exists(caminhoScriptOrigem))
            {
                ArquivoUtil.CopiarArquivo(caminhoScriptOrigem, caminhoScriptDestino, true, true);
            }

            if (File.Exists(caminhoDeclaracaoOrigem))
            {
                ArquivoUtil.CopiarArquivo(caminhoDeclaracaoOrigem, caminhoDeclaracaoDestino, true, true);
            }

            if (File.Exists(caminhoDeclaracaoMapOrigem))
            {
                ArquivoUtil.CopiarArquivo(caminhoDeclaracaoMapOrigem, caminhoDeclaracaoMapDestino, true, true);
            }
        }

        private void NornalizarScript(string caminhoSaida)
        {
            var tempo = Stopwatch.StartNew();

            var caminhoJavascriptOriginal = ArquivoUtil.AlterarExtensaoNomeArquivo(caminhoSaida, ".original.js");
            var caminhoJavascriptNormalizado = caminhoSaida;

            LogVSUtil.Log("Copiando arquivos antes de normalizar " + caminhoJavascriptOriginal);
            ArquivoUtil.DeletarArquivo(caminhoJavascriptOriginal, false, true);
            ArquivoUtil.CopiarArquivo(caminhoSaida, caminhoJavascriptOriginal, true);
            ArquivoUtil.DeletarArquivo(caminhoJavascriptNormalizado, false, true);

            if (!File.Exists(caminhoJavascriptOriginal))
            {
                throw new Exception("Falha ao copiar arquivo original" + caminhoJavascriptOriginal);
            }
            LogVSUtil.Sucesso($"Arquivo original: {File.Exists(caminhoJavascriptOriginal)}: {caminhoJavascriptOriginal}", tempo, false);
          
            this.NornalizarScriptInterno(caminhoJavascriptOriginal,
                                         caminhoJavascriptNormalizado);

            if (!CaminhoUtil.CaminhoIgual(caminhoSaida, this.ProjetoTS.CaminhoSaidaPadrao))
            {
                ProjetoTypeScriptUtil.CopiarScripts(caminhoSaida, this.ProjetoTS.CaminhoSaidaPadrao);
            }

            var caminhoRelativo = CaminhoUtil.RetornarCaminhoRelativo(caminhoSaida, this.ProjetoTS.DiretorioProjeto.FullName);
            LogVSUtil.Sucesso($"Compilação javascript {this.ProjetoTS.NomeProjeto} '{this.ConfiguracaoTypescript.compilerOptions.target}' normalizada ", tempo);
            LogVSUtil.Log($"{caminhoRelativo}");


        }
        private void NornalizarScriptInterno(string caminhoJavascriptOriginal,
                                             string caminhoJavascriptNormalizado,
                                             int tentativa = 1)
        {
            if (!File.Exists(caminhoJavascriptOriginal))
            {
                throw new FileNotFoundException("Original: " + caminhoJavascriptOriginal);
            }

            LogVSUtil.Log($"Normalizando compilação javascript {this.ProjetoTS.NomeProjeto} .");
            try
            {
                this.NormalizarInterno(caminhoJavascriptOriginal,
                                       caminhoJavascriptNormalizado);
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }

            if (!File.Exists(caminhoJavascriptNormalizado))
            {
                LogVSUtil.LogErro($"Falha na normalização, TENTATIVA ${tentativa + 1} " + caminhoJavascriptNormalizado);
                Thread.Sleep(500);
                this.NornalizarScriptInterno(caminhoJavascriptOriginal,
                                            caminhoJavascriptNormalizado);
            }
        }

        private bool NormalizarInterno(string caminhoJavascriptOriginal, string caminhoJavascriptNormalizado)
        {
            using (var msNormalizar = new MemoryStream())
            {
                using (var msOriginal = new MemoryStream(File.ReadAllBytes(caminhoJavascriptOriginal)))
                {
                    using (var leitor = new StreamReader(msOriginal, this.CODIFICACAO))
                    {
                        return this.NormalizarInterno(leitor, caminhoJavascriptNormalizado);
                    }
                }
            }
        }

        private bool NormalizarInterno(StreamReader leitor, string caminhoJavascriptNormalizado)
        {
            int intCaracter;
            var sb = new StringBuilder();
            var dicionarios = new List<string>();
            var isNormalizadoPrimeiraLinhaInserido = false;
            while (true)
            {
                intCaracter = leitor.Read();
                if (intCaracter == -1)
                {
                    break;
                }
                var caracter = (char)intCaracter;
                if (caracter == '\r' && !isNormalizadoPrimeiraLinhaInserido)
                {
                    isNormalizadoPrimeiraLinhaInserido = true;
                    sb.Append(NormalizarCompilacaoJavascript.NORMALIZADO);
                    sb.Append($" {ABRE_DATA_NORMALIZACAO} {DateTime.Now} {NormalizarCompilacaoJavascript.TICKS}{DateTime.Now.Ticks} {FECHA_DATA_NORMALIZACAO} ");
                }

                ////NAMEOF
                //if (caracter == 'n' && (char)leitor.Peek() == 'a')
                //{
                //    var (isNameofEncontrado, caracteres) = this.RetornarCaracteresNameof(leitor);
                //    if (isNameofEncontrado && leitor.Peek() == '(')
                //    {
                //        var expressao = ExpressaoUtil.RetornarExpressaoAbreFecha(leitor, true);
                //        sb.Append($"nameof(\"{expressao}\")");
                //    }
                //    else
                //    {
                //        sb.Append(caracteres);
                //    }
                //}
                else
                {
                    sb.Append(caracter);

                    if (this.IsNormalizarWhere)
                    {
                        //WHERE
                        if (caracter == 'W' && leitor.Peek() == 'h')
                        {
                            var buffer = new char[WHERE_BUFFER.Length];
                            leitor.Read(buffer, 0, WHERE_BUFFER.Length);
                            sb.Append(buffer);
                            var stringBuffer = new string(buffer);
                            if (stringBuffer.Equals(WHERE_BUFFER))
                            {
                                while (Char.IsWhiteSpace((char)leitor.Peek()))
                                {
                                    caracter = (char)leitor.Read();
                                    sb.Append(caracter);
                                }
                                if ((char)leitor.Peek() == '(')
                                {
                                    var expressao = ExpressaoUtil.RetornarExpressaoAbreFecha(leitor);
                                    using (var normalizarExpressao = this.RetornarNormalizarEspessaoWhere(expressao))
                                    {
                                        if (normalizarExpressao.IsNormalizarExpressaoWhere)
                                        {
                                            expressao = normalizarExpressao.Normalizar();
                                            dicionarios.Add(expressao);
                                        }
                                    }
                                    sb.Append(expressao);
                                }
                            }
                        }
                    }

                }

            }

            sb.AppendLine();
            sb.AppendLine($"{this.ProjetoTS.Namespace}.__IsScriptNormalizado = true;");
            sb.AppendLine($"{this.ProjetoTS.Namespace}.__DataHoraNormalizado = new Date({DateTime.UtcNow.RetornarMilisegundosJavascript()});");
            //sb.AppendLine($"/* Normalização {DateTime.Now.ToString()} */");
            var resultado = sb.ToString();

            this.SalvarNormalizacao(caminhoJavascriptNormalizado, resultado);
            this.SalvarNormalizacaoDicionarioWhere(caminhoJavascriptNormalizado, dicionarios);

            sb.Clear();
            sb = null;
            return true;

        }

        private BaseNormalizadorExpreessaoWhere RetornarNormalizarEspessaoWhere(string expressao)
        {
            var es = this.ConfiguracaoTypescript.compilerOptions.target.ToUpper();
            if (es == "ES5")
            {
                return new NormalizadorExpreessaoWhere(expressao);
            }
            return new NormalizadorExpreessaWhereES6(expressao);
            //throw new Exception($"A normalização para {this.ConfiguracaoTypescript.compilerOptions.target}  não é suportada");
        }

        //private readonly char[] PROCURAR_FUNCTION = "nameof".ToCharArray();
        //private (bool, char[]) RetornarCaracteresNameof(StreamReader leitor)
        //{
        //    var encotrados = new List<char>();
        //    encotrados.Add('n');

        //    for (var i = 1; i < PROCURAR_FUNCTION.Length; i++)
        //    {
        //        var c = (char)leitor.Read();
        //        var c2 = PROCURAR_FUNCTION[i];
        //        encotrados.Add(c);
        //        if (c != c2)
        //        {
        //            return (false, encotrados.ToArray());
        //        }
        //    }
        //    return (true, encotrados.ToArray());
        //}

        private void SalvarNormalizacao(string caminhoJavascriptNormalizado, string resultado)
        {
            ArquivoUtil.DeletarArquivo(caminhoJavascriptNormalizado);
            File.WriteAllText(caminhoJavascriptNormalizado, resultado, this.CODIFICACAO);
        }
        private void SalvarNormalizacaoDicionarioWhere(string caminhoJavascriptNormalizado, List<string> dicionarios)
        {
            var caminhoDicionario = ArquivoUtil.AlterarExtensaoNomeArquivo(caminhoJavascriptNormalizado, "dicionario.js");
            if (dicionarios.Count > 0)
            {
                File.WriteAllText(caminhoDicionario, String.Join(System.Environment.NewLine, dicionarios), this.CODIFICACAO);
            }
            else
            {
                ArquivoUtil.DeletarArquivo(caminhoDicionario);
            }
        }

        private bool RetornarIsNormalizarCompilacao(string caminhoSaida)
        {
            if (File.Exists(caminhoSaida))
            {

                using (var fsOriginal = StreamUtil.OpenRead(caminhoSaida))
                {
                    using (var leitor = new StreamReader(fsOriginal, this.CODIFICACAO))
                    {
                        var primeiraLinha = leitor.ReadLine();
                        return !primeiraLinha.Contains(NORMALIZADO);
                    }
                }
            }
            return false;
        }



        #region IDisposable 

        public void Dispose()
        {
            this.ConfiguracaoTypescript = null;
            this.ProjetoTS = null;
        }

        #endregion

    }
}