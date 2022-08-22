using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    internal static class PublicacaoUtil
    {
        internal static Task<string> PublicarVersaoAsync(EnumTipoProjeto tipoProjeto,
                                                         string caminhoProjeto,
                                                         bool isDebug)
        {
            return Task.Factory.StartNew(() => PublicacaoUtil.PublicarVersao(tipoProjeto,
                                                                            caminhoProjeto,
                                                                            isDebug),
                                                                            CancellationToken.None,
                                                                            TaskCreationOptions.None,
                                                                            TaskScheduler.Default);
        }

        public static string PublicarVersao(EnumTipoProjeto tipoProjeto,
                                             string caminhoProjeto,
                                             bool isDebug)
        {
            var infoPublicacao = RetornarInfoPulicacao(caminhoProjeto);
            if (infoPublicacao != null)
            {
                var caminhoPublicacao = infoPublicacao.CaminhoPulicacao;
                LogVSUtil.Log($"Publicando projeto {new DirectoryInfo(caminhoProjeto).Name}");

                if (!Directory.Exists(caminhoPublicacao))
                {
                    LogVSUtil.LogErro($"O caminho da publicação não foi encontrado {caminhoPublicacao}");

                    return null;
                }

                var versao = AssemblyInfoUtil.RetornarVersaoProjeto(caminhoProjeto);
                if (infoPublicacao.IsCriarPastaVersao)
                {
                    caminhoPublicacao = Path.Combine(caminhoPublicacao, versao.ToString());
                }
                DiretorioUtil.CriarDiretorio(caminhoProjeto);


                var infosPastas = RetornarCaminhoPastas(tipoProjeto,
                                                        caminhoProjeto,
                                                        isDebug);
                foreach (var infoPasta in infosPastas)
                {
                    if (Directory.Exists(infoPasta.Caminho))
                    {
                        CopiarDiretorio(tipoProjeto,
                                        infoPublicacao,
                                        infoPasta,
                                        versao,
                                        caminhoPublicacao);
                    }
                }

                if (infoPublicacao.BuildJsOptions != null)
                {
                    //var caminhoPublicacaoBuild = Path.Combine(caminhoPublicacao, ExtensaoContantes.PASTA_BUILD, versao.ToString());
                    var caminhoPublicacaoBuild = String.IsNullOrWhiteSpace(infoPublicacao.NomePastaBuild) ?
                                                                           Path.Combine(caminhoPublicacao, ConstantesProjeto.PASTA_BUILD, versao.ToString()) :
                                                                           Path.Combine(caminhoPublicacao, ConstantesProjeto.PASTA_BUILD, infoPublicacao.NomePastaBuild, versao.ToString());

                    PublicacaoUtil.AplicarJsOptions(infoPublicacao,
                                                    caminhoProjeto,
                                                    caminhoPublicacao,
                                                    caminhoPublicacaoBuild,
                                                    versao);
                }


                if (tipoProjeto == EnumTipoProjeto.Typescript)
                {
                    var prefixoLastVersion = String.IsNullOrWhiteSpace(infoPublicacao.NomePastaBuild) ? String.Empty :
                                                                                                       $"{infoPublicacao.NomePastaBuild.ToLower()}-";
                    File.WriteAllText(Path.Combine(caminhoPublicacao, $"{prefixoLastVersion}last-version.txt"), versao.ToString());
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
                return caminhoPublicacao;
            }
            return null;


        }

        private static void AplicarJsOptions(PublicacaoConfig infoPublicacao,
                                            string caminhoProjeto,
                                            string caminhoPublicacao,
                                            string caminhoPublicacaoBuild,
                                            Version versao)
        {
            var caminhoBuild = Path.Combine(caminhoProjeto, ConstantesProjeto.PASTA_BUILD);
            var arquivosJs = infoPublicacao.Builds.Where(x => Path.GetExtension(x) == ".js");
            var sb = new StringBuilder();
            if (infoPublicacao.BuildJsOptions.IsEncapsular)
            {
                sb.AppendLine("(function() {");
            }
            foreach (var arquivo in arquivosJs)
            {
                sb.AppendLine($"\t\t//{arquivo}");
                var caminhoJs = Path.Combine(caminhoProjeto, ConstantesProjeto.PASTA_BUILD, arquivo);
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
                IsTeste = infoPublicacao.BuildJsOptions.IsTeste
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
                                            PublicacaoConfig infoPublicacao,
                                            InfoPasta infoPasta,
                                            Version versao,
                                            string caminhoPublicacao)
        {
            switch (infoPasta.TipoPasta)
            {
                case EnumTipoPasta.Bin:

                    var caminhoBin = tipoProjeto == EnumTipoProjeto.Desktop ||
                                     tipoProjeto == EnumTipoProjeto.ExtensaoVisualStudio ? caminhoPublicacao :
                                                                                           Path.Combine(caminhoPublicacao, "bin");

                    if (infoPublicacao.ArquivosBin?.Count() > 0)
                    {
                        CopriarArquivos(infoPublicacao,
                                        infoPasta.Caminho,
                                        caminhoBin,
                                        infoPublicacao.ArquivosBin);
                    }
                    else
                    {
                        CopiarDiretorio(infoPublicacao,
                                        infoPasta.Caminho,
                                        caminhoBin);
                    }

                    break;
                case EnumTipoPasta.Build:

                    var caminhoBuild = String.IsNullOrWhiteSpace(infoPublicacao.NomePastaBuild) ?
                                            Path.Combine(caminhoPublicacao, ConstantesProjeto.PASTA_BUILD, versao.ToString()) :
                                            Path.Combine(caminhoPublicacao, ConstantesProjeto.PASTA_BUILD, infoPublicacao.NomePastaBuild, versao.ToString());

                    CopriarArquivos(infoPublicacao,
                                    infoPasta.Caminho,
                                    caminhoBuild,
                                    infoPublicacao.Builds);

                    break;

                case EnumTipoPasta.Web:

                    CopriarArquivos(infoPublicacao,
                                    infoPasta.Caminho,
                                    caminhoPublicacao,
                                    infoPublicacao.ArquivosWeb);
                    break;
                default:

                    throw new Erro("Tipo de pasta não suportado");
            }
        }

        internal static void AtribuirVersaoExtensaoVisualStudio(Version versao, string caminhoProjeto)
        {
            var caminhoXML = Path.Combine(caminhoProjeto, "source.extension.vsixmanifest");
            try
            {
                var xml = new System.Xml.XmlDocument();
                xml.Load(caminhoXML);

                var IdentityTag = xml.GetElementsByTagName("Identity")[0];
                IdentityTag.Attributes["Version"].Value = versao.ToString();
                xml.Save(caminhoXML);
            }
            catch (Exception erro)
            {
                LogVSUtil.LogErro(erro);
            }
        }

        private static void CopiarDiretorio(PublicacaoConfig publicacaoConfig,
                                            string diretorioOrigem,
                                            string diretorioDestino)
        {
            var arquivos = Directory.GetFiles(diretorioOrigem);
            CopriarArquivos(publicacaoConfig,
                            diretorioOrigem,
                            diretorioDestino,
                            arquivos);
        }

        private static void CopriarArquivos(PublicacaoConfig publicacaoConfig,
                                            string diretorioOrigem,
                                            string diretorioDestino,
                                            string[] arquivos)
        {
            if (arquivos?.Count() > 0)
            {
                foreach (var arquivo in arquivos.Where(x => !String.IsNullOrWhiteSpace(x)))
                {
                    var caminhoOrigem = Path.GetFullPath(Path.Combine(diretorioOrigem, arquivo));
                    if(!File.Exists(caminhoOrigem))
                    {
                        LogVSUtil.LogErro($"Falha na publicação. Arquivo não encontrado: {caminhoOrigem} ");
                        continue;
                    }

                    var nomeArquivoDestino = NormalizarNomeArquivoDestino(publicacaoConfig, Path.GetFileName(arquivo));
                    var caminhoDestino = Path.Combine(diretorioDestino, nomeArquivoDestino);
                    ArquivoUtil.CopiarArquivo(caminhoOrigem, caminhoDestino, true);

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
                return JsonUtil.DeserializaArquivor<PublicacaoConfig>(caminhoInfoPulicacao, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro("falha ao deserializar publicacao.json ", ex);
                return null;
            }
        }

        private static InfoPasta[] RetornarCaminhoPastas(EnumTipoProjeto tipoProjeto,
                                                         string caminhoProjeto,
                                                         bool isDebug)
        {
            var caminhoBin = Path.Combine(caminhoProjeto, "bin");

            switch (tipoProjeto)
            {
                case EnumTipoProjeto.ExtensaoVisualStudio:
                case EnumTipoProjeto.Desktop:

                    var pasta = (isDebug) ? "debug" : "release";
                    var caminhoFinal = Path.Combine(caminhoBin, pasta);
                    return new InfoPasta[] { new InfoPasta(caminhoFinal, EnumTipoPasta.Bin) };

                case EnumTipoProjeto.Typescript:

                    var caminhoBuild = Path.Combine(caminhoProjeto, ConstantesProjeto.PASTA_BUILD);

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
}
