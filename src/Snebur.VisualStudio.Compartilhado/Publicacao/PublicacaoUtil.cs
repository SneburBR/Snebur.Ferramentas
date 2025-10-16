using Snebur.Cli;
using Snebur.Utilidade;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    public static class PublicacaoUtil
    {
        public static Task PublicarVersaoAsync(string nomeProjeto,
                                               EnumTipoProjeto tipoProjeto,
                                               EnumTipoCompilacao tipoCompilacao,
                                               string caminhoiProjeto,
                                               string caminhoSolution,
                                               Stopwatch tempo)
        {
            return Task.Run(() =>
            {
                PublicarVersao(nomeProjeto,
                               tipoProjeto,
                               tipoCompilacao,
                               caminhoiProjeto,
                               caminhoSolution,
                               tempo);
            });

        }
        private static void PublicarVersao(string nomeProjeto,
                                           EnumTipoProjeto tipoProjeto,
                                           EnumTipoCompilacao tipoCompilacao,
                                           string caminhoiProjeto,
                                           string caminhoSolution,
                                           Stopwatch tempo)
        {
            var caminhoDestino = PublicarVersaoInterno(nomeProjeto,
                                                      tipoProjeto,
                                                       tipoCompilacao,
                                                       caminhoiProjeto,
                                                       caminhoSolution);
            if (Directory.Exists(caminhoDestino))
            {
                LogVSUtil.Sucesso($"Arquivos publicados  {caminhoDestino}", tempo);
                Process.Start(caminhoDestino);
            }
        }

        private static string PublicarVersaoInterno(string nomeProjeto,
                                                    EnumTipoProjeto tipoProjeto,
                                                    EnumTipoCompilacao tipoCompilacao,
                                                    string caminhoiProjeto,
                                                    string caminhoSolution)
        {
            var diretorioProjeto = Path.GetDirectoryName(caminhoiProjeto);
            var infoPublicacao = RetornarInfoPulicacao(diretorioProjeto);
            if (infoPublicacao != null)
            {
                var diretorioPublicacaoFinal = infoPublicacao.CaminhoPulicacao;
                LogVSUtil.Log($"Publicando projeto {new DirectoryInfo(diretorioProjeto).Name}");

                if (!Directory.Exists(diretorioPublicacaoFinal))
                {
                    LogVSUtil.LogErro($"O caminho da publicação não foi encontrado {diretorioPublicacaoFinal}");

                    return null;
                }

                var versao = AssemblyInfoUtil.RetornarVersaoProjeto(diretorioProjeto);
                if (versao == null)
                {
                    LogVSUtil.LogErro($"Não foi possível encontrada versão do projeto{diretorioProjeto}");
                    return null;
                }
                if (infoPublicacao.IsCriarPastaVersao)
                {
                    diretorioPublicacaoFinal = Path.Combine(diretorioPublicacaoFinal, versao.ToString());
                }

                DiretorioUtil.CriarDiretorio(diretorioProjeto);

                var diretorioTemporario = Path.Combine(Path.GetPathRoot(diretorioProjeto), "temp", versao.ToString());
                DiretorioUtil.CriarDiretorio(diretorioTemporario);

                var compilacoes = infoPublicacao.RetornarCompilacoes();

                foreach (var compilacao in compilacoes)
                {
                    var diretorioTemporarioCompilacao = Path.Combine(diretorioTemporario, compilacao);

                    PublicarCompilacao(
                         nomeProjeto,
                         tipoProjeto,
                         tipoCompilacao,
                         diretorioProjeto,
                         infoPublicacao,
                         diretorioPublicacaoFinal,
                         versao,
                         diretorioTemporarioCompilacao,
                         compilacao);
                }

                //if (tipoProjeto == EnumTipoProjeto.Typescript)
                //{
                var prefixoLastVersion = String.IsNullOrWhiteSpace(infoPublicacao.NomePastaBuild) ? String.Empty :
                                                                                                   $"{infoPublicacao.NomePastaBuild.ToLower()}-";

                File.WriteAllText(Path.Combine(diretorioPublicacaoFinal, $"{prefixoLastVersion}last-version.txt"), versao.ToString());
                File.WriteAllText(Path.Combine(infoPublicacao.CaminhoPulicacao, $"{prefixoLastVersion}last-version.txt"), versao.ToString());
                //}

                if (tipoProjeto == EnumTipoProjeto.Typescript)
                {
                    EscrevaESVersao(diretorioProjeto, diretorioPublicacaoFinal);
                }

                if (File.Exists(infoPublicacao.ExecutarProcessoDepois))
                {
                    try
                    {
                        Process.Start(infoPublicacao.ExecutarProcessoDepois);
                    }
                    catch (Exception ex)
                    {
                        LogVSUtil.LogErro(ex);
                    }
                }

                if (infoPublicacao.IsCopiarSource)
                {
                    var nomeArquivo = $"{versao}_source.zip";
                    var diretorioSource = Path.Combine(diretorioTemporario, "source");

                    CopiarProjectSourceUtil.Copiar(caminhoiProjeto,
                                                   caminhoSolution,
                                                   diretorioSource);

                    var caminhoSourceZipTemp = Path.Combine(diretorioTemporario, nomeArquivo);
                    var caminhoPublicacaoSource = Path.Combine(diretorioPublicacaoFinal, nomeArquivo);

                    ZipUtil.CompactarPasta(diretorioSource, caminhoSourceZipTemp, true);

                    CopiarArquivo(infoPublicacao,
                                  caminhoSourceZipTemp,
                                  caminhoPublicacaoSource);

                }

                DiretorioUtil.ExcluirDiretorio(diretorioTemporario, true, true, true);
                return diretorioPublicacaoFinal;
            }
            return null;
        }

        private static void PublicarCompilacao(
            string nomeProjeto,
            EnumTipoProjeto tipoProjeto,
            EnumTipoCompilacao tipoCompilacao,
            string diretorioProjeto,
            PublicacaoConfig infoPublicacao,
            string diretorioPublicacaoFinal,
            Version versao,
            string diretorioTemporario,
            string compilacao)
        {
            var infosPastas = RetornarCaminhoPastas(
                tipoProjeto,
                tipoCompilacao,
                diretorioProjeto,
                compilacao);

            foreach (var infoPasta in infosPastas)
            {
                if (Directory.Exists(infoPasta.Caminho))
                {
                    CopiarDiretorio(tipoProjeto,
                        tipoCompilacao,
                        infoPublicacao,
                        infoPasta,
                        versao,
                        diretorioTemporario);
                }
            }

            var direotrioTemporarioBuild = diretorioTemporario;
            if (infoPublicacao.BuildJsOptions != null)
            {
                direotrioTemporarioBuild = infoPublicacao.RetornarCaminhoPublicacaoBuild(versao, diretorioTemporario);
                PublicacaoUtil.AplicarJsOptions(infoPublicacao,
                                                diretorioProjeto,
                                                diretorioTemporario,
                                                direotrioTemporarioBuild,
                                                versao);
            }

            if (infoPublicacao.IsZiparBin)
            {
                var nomeArquivo = infoPublicacao.RetornarNomeArquivo(
                    nomeProjeto,
                    versao,
                    tipoCompilacao,
                    compilacao);

                var caminhoZipTemp = Path.Combine(direotrioTemporarioBuild, nomeArquivo);

                ZipUtil.CompactarPasta(diretorioTemporario, caminhoZipTemp, true);

                var caminhoPublicacaoZip = Path.Combine(diretorioPublicacaoFinal, nomeArquivo);

                CopiarArquivo(infoPublicacao,
                              caminhoZipTemp,
                              caminhoPublicacaoZip);

            }
            else
            {
                CopiarTodosArquivos(infoPublicacao,
                    tipoCompilacao,
                    diretorioTemporario,
                    diretorioPublicacaoFinal);

            }
        }

        private static void EscrevaESVersao(string caminhoProjeto, string caminhoPublicacao)
        {
            var caminhoTS = Path.Combine(caminhoProjeto, ConstantesProjeto.CONFIGURACAO_TYPESCRIPT);
            if (File.Exists(caminhoTS))
            {
                var tsconfig = JsonUtil.TryDeserializar<ConfiguracaoProjetoTypeScript>(ArquivoUtil.TryLerTexto(caminhoTS), EnumTipoSerializacao.Javascript);
                if (tsconfig != null)
                {
                    var esversion = tsconfig.CompilerOptions.target;
                    var caminhoESVersion = Path.Combine(caminhoPublicacao, $"{esversion}.txt");
                    try
                    {
                        File.WriteAllText(caminhoESVersion, esversion);
                    }
                    catch (Exception ex)
                    {
                        LogVSUtil.LogErro(ex);
                    }
                }

            }
        }

        private static void AplicarJsOptions(PublicacaoConfig infoPublicacao,
                                            string caminhoProjeto,
                                            string caminhoPublicacao,
                                            string caminhoPublicacaoBuild,
                                            Version versao)
        {
            var caminhoBuild = Path.Combine(caminhoProjeto, ConstantesProjeto.PASTA_WWWROOT_BUILD);
            var arquivosJs = infoPublicacao.Builds.Where(x => Path.GetExtension(x) == ".js");
            var sb = new StringBuilder();
            if (infoPublicacao.BuildJsOptions.IsEncapsular)
            {
                sb.AppendLine("(function() {");
            }
            foreach (var arquivo in arquivosJs)
            {
                sb.AppendLine($"\t\t//{arquivo}");
                var caminhoJs = Path.Combine(caminhoProjeto, ConstantesProjeto.PASTA_WWWROOT_BUILD, arquivo);
                var lines = File.ReadAllLines(caminhoJs, Encoding.UTF8);
                foreach (var line in lines)
                {
                    if (line.TrimStart().StartsWith("//"))
                    {
                        continue;
                    }
                    sb.AppendLine(line);
                }
            }
            if (infoPublicacao.BuildJsOptions.IsEncapsular)
            {
                sb.AppendLine("})()");
            }

            var conteudo = sb.ToString();
            string caminhoDestino = Path.Combine(caminhoPublicacaoBuild, infoPublicacao.BuildJsOptions.NomeArquivo);

            if (infoPublicacao.BuildJsOptions.IsZipar)
            {
                //var nomeArquivoZip = infoPublicacao.BuildJsOptions.NomeArquivoZip ?? $"{Path.GetFileNameWithoutExtension(infoPublicacao.BuildJsOptions.NomeArquivo)}.zip";
                caminhoDestino = Path.ChangeExtension(caminhoDestino, ".zip");
                ZipUtil.CompactarTexto(conteudo, caminhoDestino);
            }
            else
            {
                //var caminhoDestino = Path.Combine(caminhoPublicacaoBuild, infoPublicacao.BuildJsOptions.NomeArquivo);
                File.WriteAllText(caminhoDestino, conteudo, Encoding.UTF8);
            }

            var infoVersion = new InfoVersao
            {
                Data = DateTime.Now,
                Checksum = ChecksumUtil.RetornarChecksum(caminhoDestino),
                Versao = versao.ToString(),
                NomePastaBuild = infoPublicacao.NomePastaBuild,
                IsCompactado = infoPublicacao.BuildJsOptions.IsZipar,
                IsEncapsulado = infoPublicacao.BuildJsOptions.IsEncapsular,
                IsTeste = infoPublicacao.BuildJsOptions.IsTeste,
                IsLibZipAsync = infoPublicacao.BuildJsOptions.IsLibZipAsync
            };
            var json = JsonUtil.SerializarJsonCamelCase(infoVersion);
            var prefixoLastVersion = String.IsNullOrWhiteSpace(infoPublicacao.NomePastaBuild) ? String.Empty :
                                                                                                 $"{infoPublicacao.NomePastaBuild.ToLower()}-";

            var caminhoVersion = Path.Combine(caminhoPublicacaoBuild, "version.json");
            var caminhoLastVerstion = Path.Combine(caminhoPublicacao, $"{prefixoLastVersion}last-version.json");

            File.WriteAllText(caminhoVersion, json, Encoding.UTF8);
            File.WriteAllText(caminhoLastVerstion, json, Encoding.UTF8);
        }

        private static void CopiarDiretorio(EnumTipoProjeto tipoProjeto,
                                            EnumTipoCompilacao tipoCompilacao,
                                            PublicacaoConfig infoPublicacao,
                                            InfoPasta infoPasta,
                                            Version versao,
                                            string caminhoPublicacao)
        {
            var caminhoDestino = infoPublicacao.RetornarCaminhoPublicacaoBuild(versao, caminhoPublicacao);

            switch (infoPasta.TipoPasta)
            {
                case EnumTipoPasta.Bin:

                    if (tipoProjeto == EnumTipoProjeto.Web)
                    {
                        caminhoDestino = Path.Combine(caminhoDestino, "bin");
                    }

                    if (infoPublicacao.ArquivosBin?.Count() > 0)
                    {
                        CopriarArquivos(infoPublicacao,
                                        tipoCompilacao,
                                        infoPasta.Caminho,
                                        caminhoDestino,
                                        infoPublicacao.ArquivosBin);
                    }
                    else
                    {
                        CopiarTodosArquivos(infoPublicacao,
                            tipoCompilacao,
                            infoPasta.Caminho,
                            caminhoDestino);
                    }

                    break;

                case EnumTipoPasta.Build:

                    CopriarArquivos(infoPublicacao,
                        tipoCompilacao,
                        infoPasta.Caminho,
                        caminhoDestino,
                        infoPublicacao.Builds);

                    break;

                case EnumTipoPasta.Web:

                    CopriarArquivos(infoPublicacao,
                        tipoCompilacao,
                        infoPasta.Caminho,
                        caminhoDestino,
                        infoPublicacao.ArquivosWeb);

                    break;

                default:

                    throw new Erro("Tipo de pasta não suportado");
            }
        }

        //internal static void AtribuirVersaoExtensaoVisualStudio(Version versao, string caminhoProjeto)
        //{
        //    var caminhoXML = Path.Combine(caminhoProjeto, "source.extension.vsixmanifest");
        //    try
        //    {
        //        var xml = new System.Xml.XmlDocument();
        //        xml.Load(caminhoXML);

        //        var IdentityTag = xml.GetElementsByTagName("Identity")[0];
        //        IdentityTag.Attributes["Version"].Value = versao.ToString();
        //        xml.Save(caminhoXML);
        //    }
        //    catch (Exception erro)
        //    {
        //        LogVSUtil.LogErro(erro);
        //    }
        //}

        public static void CopiarArquivo(PublicacaoConfig infoPublicacao,
                                         string caminhoZipTemp,
                                         string caminhoPublicacaoZip)
        {
            double lastProgresso = 0;
            StreamUtil.CopiarArquivo(caminhoZipTemp,
                                     caminhoPublicacaoZip,
                                     512 * 1024,
                                     (progressArgs) =>
                                     {
                                         var progresso = progressArgs.Progresso * 100;
                                         if (progresso > (lastProgresso + 10))
                                         {
                                             LogVSUtil.Log($"Publicando {Path.GetFileName(caminhoPublicacaoZip)} {progresso:0.0}%");
                                             lastProgresso = progresso;
                                         }
                                     });

            LogVSUtil.Log($"Publicando {Path.GetFileName(caminhoPublicacaoZip)} {100:0.0}%");

        }

        private static void CopiarTodosArquivos(PublicacaoConfig publicacaoConfig,
                                            EnumTipoCompilacao tipoCompilacao,
                                            string diretorioOrigem,
                                            string diretorioDestino)
        {
            var arquivos = Directory.GetFiles(diretorioOrigem, "*", SearchOption.AllDirectories);
            CopriarArquivos(publicacaoConfig,
                            tipoCompilacao,
                            diretorioOrigem,
                            diretorioDestino,
                            arquivos);
        }

        private static void CopriarArquivos(PublicacaoConfig publicacaoConfig,
                                            EnumTipoCompilacao tipoCompilacao,
                                            string diretorioOrigem,
                                            string diretorioDestino,
                                            string[] arquivos)
        {
            if (arquivos?.Count() > 0)
            {
                var count = 0;
                var progresso = 0;
                var lastProgresso = 0;

                foreach (var arquivo in arquivos.Where(x => !String.IsNullOrWhiteSpace(x)))
                {

                    var caminhoOrigem = Path.GetFullPath(Path.Combine(diretorioOrigem, arquivo));
                    if (!File.Exists(caminhoOrigem))
                    {
                        LogVSUtil.LogErro($"Falha na publicação. Arquivo não encontrado: {caminhoOrigem} ");
                        continue;
                    }

                    if (publicacaoConfig.IsIgnorarArquivo(arquivo))
                    {
                        continue;
                    }

                    if (tipoCompilacao == EnumTipoCompilacao.Release &&
                       publicacaoConfig.IsIgnorarArquivoRelease(arquivo))
                    {
                        continue;
                    }
                     
                    LogVSUtil.Log($"Copiando arquivo {Path.GetFileName(caminhoOrigem)}");

                    var caminhoRelatativo = CaminhoUtil.RetornarCaminhoRelativo(Path.GetDirectoryName(arquivo), diretorioOrigem);
                    var nomeArquivoDestino = NormalizarNomeArquivoDestino(publicacaoConfig, Path.GetFileName(arquivo));
                     
                    var caminhoDestino = Path.Combine(diretorioDestino, caminhoRelatativo, nomeArquivoDestino);
                    ArquivoUtil.CopiarArquivo(caminhoOrigem, caminhoDestino, true);

                    count++;
                    progresso = (count * 100) / arquivos.Count();

                    if (progresso > (lastProgresso + 10))
                    {
                        LogVSUtil.Log($"Publicando {Path.GetFileName(diretorioDestino)} {progresso}%");
                        lastProgresso = progresso;
                    }
                }
            }
        }

        private static string NormalizarNomeArquivoDestino(PublicacaoConfig publicacaoConfig,
                                                           string nomeArquivo)
        {
            if (!String.IsNullOrWhiteSpace(publicacaoConfig.NomeLib))
            {
                if (nomeArquivo.StartsWith("Snebur"))
                {
                    return $"{publicacaoConfig.NomeLib}{nomeArquivo.Substring(6)}";
                }
            }
            return nomeArquivo;
        }

        private static PublicacaoConfig RetornarInfoPulicacao(string caminhoProjeto)
        {
            var caminhoInfoPulicacao = Path.Combine(caminhoProjeto, "publicacao.json");

            if (!File.Exists(caminhoInfoPulicacao))
            {
                LogVSUtil.LogErro($"O arquivo publicacao.json não foi encontrado no projeto {Path.GetDirectoryName(caminhoProjeto)}");
                return null;
            }
            try
            {
                return JsonUtil.DeserializaArquivor<PublicacaoConfig>(caminhoInfoPulicacao,
                                                                      Encoding.UTF8,
                                                                      EnumTipoSerializacao.Javascript);
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro("falha ao desserializar publicacao.json ", ex);
                return null;
            }
        }

        private static InfoPasta[] RetornarCaminhoPastas(EnumTipoProjeto tipoProjeto,
                                                         EnumTipoCompilacao tipoCompiolacao,
                                                         string caminhoProjeto,
                                                         string caminhoCompilacao)
        {

            var caminhoBin = Path.Combine(caminhoProjeto, "bin");

            switch (tipoProjeto)
            {
                case EnumTipoProjeto.ExtensaoVisualStudio:
                case EnumTipoProjeto.Desktop:

                    var compilacao = (tipoCompiolacao == EnumTipoCompilacao.Debug) ? "debug" : "release";
                    var caminhoFinal = Path.Combine(caminhoBin, compilacao, caminhoCompilacao ?? "");
                    return new InfoPasta[] { new InfoPasta(caminhoFinal, EnumTipoPasta.Bin) };

                case EnumTipoProjeto.Typescript:

                    var caminhoBuild = Path.Combine(caminhoProjeto, ConstantesProjeto.PASTA_WWWROOT_BUILD);

                    return new InfoPasta[] { new InfoPasta(caminhoBin, EnumTipoPasta.Bin) ,
                                             new InfoPasta(caminhoBuild, EnumTipoPasta.Build)};

                case EnumTipoProjeto.Web:

                    return new InfoPasta[] { new InfoPasta(caminhoBin, EnumTipoPasta.Bin),
                                             new InfoPasta(caminhoProjeto, EnumTipoPasta.Web) };

                default:

                    throw new Exception("Tipo de projeto não suportado");

            }
        }

        public static EnumTipoProjeto RetornarTipoProjeto(string caminhoProjeto)
        {
            var caminhoVSIX = Path.Combine(caminhoProjeto, "source.extension.vsixmanifest");
            if (File.Exists(caminhoVSIX))
            {
                return EnumTipoProjeto.ExtensaoVisualStudio;
            }

            var caminhoTS = Path.Combine(caminhoProjeto, "tsconfig.json");
            if (File.Exists(caminhoTS))
            {
                return EnumTipoProjeto.Typescript;
            }

            var caminhoWebConfig = Path.Combine(caminhoProjeto, "web.config");
            if (File.Exists(caminhoWebConfig))
            {
                return EnumTipoProjeto.Web;
            }
            return EnumTipoProjeto.Desktop;
        }

        public static EnumTipoCompilacao RetornarTipoCompilacao(string buildType)
        {
            if (buildType == "Debug")
            {
                return EnumTipoCompilacao.Debug;
            }

            if (buildType == "Release")
            {
                return EnumTipoCompilacao.Release;
            }
            return EnumTipoCompilacao.Custom;
        }
    }

    public class InfoPasta
    {
        public string Caminho { get; }
        public EnumTipoPasta TipoPasta { get; }
        public InfoPasta(string caminhoFinal, EnumTipoPasta bin)
        {
            this.Caminho = caminhoFinal;
            this.TipoPasta = bin;
        }
    }

    public enum EnumTipoProjeto
    {
        Desktop,
        Typescript,
        Web,
        ExtensaoVisualStudio
    }

    public enum EnumTipoCompilacao
    {
        Debug,
        Release,
        Custom
    }
}
