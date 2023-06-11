using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Snebur.Publicacao;

namespace Snebur.VisualStudio
{
    internal class TipoArquivoTypeScriptUtil
    {
        internal const string PROCURAR_NAMESPACE = "namespace ";
        internal const string PROCURAR_CLASSE_BASE = "class ";
        internal const string PROCURAR_CLASSE_BASE_ABSTRATA = "abstract class ";
        internal const string PROCURAR_CLASSE_EXPORT = "export class ";
        internal const string PROCURAR_CLASSE_EXPORT_ABSTRATA = "export abstract class";
        internal const string PROCURAR_ENUM_BASE = "enum ";
        internal const string PROCURAR_ENUM_EXPORT = "export enum ";
        internal const string PROCURAR_INTERFACE = "interface ";
        internal const string PROCURAR_INTERFACE_EXPORT = "export interface ";
        internal const string PROCURAR_EXTENDS = " extends ";


        internal static IEnumerable<string> RetornarCaminhosClassesBase(ConfiguracaoProjetoTypeScript configuracaoProjetoTypeScript, string caminhoProjeto)
        {
            var caminhosClassesBase = new List<string>();
            if (configuracaoProjetoTypeScript.Depedencias?.Count > 0)
            {
                foreach (var chaveValor in configuracaoProjetoTypeScript.Depedencias)
                {
                    var nomeProjeto = chaveValor.Key;
                    var caminhoParcial = chaveValor.Value;
                    var caminhoDefinicacao = Path.GetFullPath(Path.Combine(caminhoProjeto, caminhoParcial));
                    var fi = new FileInfo(caminhoDefinicacao);
                    if (!fi.Exists)
                    {
                        if (!configuracaoProjetoTypeScript.Depedencias.Values.Any(x => Path.GetFileName(x).Equals(fi.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            throw new Exception($"Não foi encontrado o arquivo de definição da dependência {fi.FullName}");
                        }
                    }

                    using (var projetoDepedencia = new ProjetoTSDepdencia(nomeProjeto, caminhoDefinicacao))
                    {
                        caminhosClassesBase.AddRange(projetoDepedencia.RetornarCaminhosClassesBase());
                    }
                }
            }
            return caminhosClassesBase;
        }

        internal static Dictionary<EnumTipoArquivoTypeScript, string> TiposProcurar
        {
            get
            {
                return new Dictionary<EnumTipoArquivoTypeScript, string>
                {
                    { EnumTipoArquivoTypeScript.ClasseBase,PROCURAR_CLASSE_BASE  },
                    { EnumTipoArquivoTypeScript.ClasseBaseAbstrata,PROCURAR_CLASSE_BASE_ABSTRATA  },
                    { EnumTipoArquivoTypeScript.ClasseExportAbstrata,PROCURAR_CLASSE_EXPORT_ABSTRATA  },
                    { EnumTipoArquivoTypeScript.ClasseExport,PROCURAR_CLASSE_EXPORT  },
                    { EnumTipoArquivoTypeScript.EnumBase,PROCURAR_ENUM_BASE  },
                    { EnumTipoArquivoTypeScript.EnumExport,PROCURAR_ENUM_EXPORT  },
                    { EnumTipoArquivoTypeScript.Inteface,PROCURAR_INTERFACE  },
                    { EnumTipoArquivoTypeScript.IntefaceExport,PROCURAR_INTERFACE_EXPORT  }
                };
            }
        }


        public static EnumTipoArquivoTypeScript RetornarTipoArquivoTypeScript(string linha)
        {
            var tiposEncontrados = new List<EnumTipoArquivoTypeScript>();
            foreach (var tipoProcurar in TipoArquivoTypeScriptUtil.TiposProcurar)
            {
                var tipoEnum = tipoProcurar.Key;
                var procurar = tipoProcurar.Value;

                if (linha != null)
                {
                    tiposEncontrados.Add(tipoEnum);
                }
            }

            if (tiposEncontrados.Count == 0)
            {
                throw new Exception("Nenhum tipo foi encontrado no arquivo, verificar export " + linha);
            }

            return tiposEncontrados.OrderByDescending(x => x).First();
        }


        internal static string RetornarProcurarTipo(EnumTipoArquivoTypeScript tipoArquivoTypescript)
        {
            switch (tipoArquivoTypescript)
            {
                case EnumTipoArquivoTypeScript.ClasseBaseAbstrata:

                    return PROCURAR_CLASSE_BASE_ABSTRATA;

                case EnumTipoArquivoTypeScript.ClasseBase:

                    return PROCURAR_CLASSE_BASE;

                case EnumTipoArquivoTypeScript.BaseClasseViewModelAbstrata:

                    return PROCURAR_CLASSE_BASE_ABSTRATA;

                case EnumTipoArquivoTypeScript.ClasseViewModelAbstrataExport:

                    return PROCURAR_CLASSE_EXPORT_ABSTRATA;

                case EnumTipoArquivoTypeScript.ClasseViewModel:
                case EnumTipoArquivoTypeScript.ClasseExport:

                    return PROCURAR_CLASSE_EXPORT;

                case EnumTipoArquivoTypeScript.ClasseExportAbstrata:

                    return PROCURAR_CLASSE_EXPORT_ABSTRATA;

                case EnumTipoArquivoTypeScript.EnumBase:

                    return PROCURAR_ENUM_BASE;

                case EnumTipoArquivoTypeScript.EnumExport:

                    return PROCURAR_ENUM_EXPORT;

                case EnumTipoArquivoTypeScript.Inteface:

                    return PROCURAR_INTERFACE;

                case EnumTipoArquivoTypeScript.IntefaceExport:

                    return PROCURAR_INTERFACE_EXPORT;

                default:
                    throw new NotSupportedException("TipoArquivoTypeScript não suportado " + tipoArquivoTypescript.ToString());
            }

        }

        internal static List<BaseArquivoTypeScript> RetornarArquivosTypeScript(ConfiguracaoProjetoTypeScript configuracao,
            string caminhoProjeto,
            HashSet<string> arquivos,
            Func<FileInfo, int> funcaoRetornarPrioridade)
        {
            //var caminhoScriptps = Path.GetDirectoryName(Path.Combine(caminhoProjeto, configuracao.compilerOptions.outFile));
            var caminhoScripts = Path.Combine(caminhoProjeto, ConstantesPublicacao.NOME_PASTA_BUILD);
            var arquivosTS = new List<BaseArquivoTypeScript>();
            foreach (var caminhoArquivo in arquivos)
            {
                if (!File.Exists(caminhoArquivo))
                {
                    LogVSUtil.Alerta($"O arquivo '{Path.GetFileName(caminhoArquivo)}' não existe {caminhoArquivo}");
                    continue;
                }
                var arquivo = new FileInfo(caminhoArquivo);
                if (arquivo.Name.EndsWith(".bkp.ts") ||
                    arquivo.Name.EndsWith(".backup.ts") ||
                    arquivo.Name.EndsWith(".copia.ts"))
                {
                    //ignorar
                    continue;
                }

                if (VSUtil.IsArquivoVisualStudio(arquivo))
                {
                    continue;
                }

                if (arquivo.Name.StartsWith("Snebur") && arquivo.Name.EndsWith(".d.ts"))
                {
                    LogVSUtil.Log($"Arquivo auto gerado encontrado e removido {arquivo.Name} ");
                    continue;
                }
                 
                var prioridadeProjeto = funcaoRetornarPrioridade?.Invoke(arquivo) ?? 0;
                if (arquivo.Name.Contains(".Dominio."))
                {
                    arquivosTS.Add(new ArquivoTSDominio(arquivo, prioridadeProjeto));
                }
                else if (ArquivoTSSistema.IsArquivoSistema(arquivo))
                {
                    arquivosTS.Add(new ArquivoTSSistema(arquivo, prioridadeProjeto));
                }
                else
                {

                    arquivosTS.Add(new ArquivoTypeScript(configuracao,  caminhoProjeto, arquivo, prioridadeProjeto));
                }
            }

            return arquivosTS.OrderBy(x => x.PrioridadeProjeto).
                              ThenBy(x => Convert.ToInt32(x.TipoArquivoTypeScript)).
                              ThenBy(x =>
                              {
                                  if (x is ArquivoTSDominio)
                                  {
                                      return ((ArquivoTSDominio)x).PrioridadeDominio;
                                  }
                                  return 0;
                              }).
                              ThenBy(x => x.Namespace).
                              ThenBy(x => Convert.ToInt32(x.TipoArquivoTypeScript)).
                              ToList();
        }
     }
}
