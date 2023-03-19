using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Snebur.Dominio;
using Snebur.Utilidade;
using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio
{
    public class ArquivoTypeScript : BaseArquivoTypeScript
    {
        public const string REPOSITORIO_NATIVO = "Nativo";

        public static readonly Enum TIPOS_ABSTRATOS = EnumTipoArquivoTypeScript.BaseClasseViewModelAbstrata |
                                             EnumTipoArquivoTypeScript.ClasseBaseAbstrata |
                                             EnumTipoArquivoTypeScript.ClasseExportAbstrata |
                                             EnumTipoArquivoTypeScript.ClasseViewModelAbstrataExport;

        private readonly HashSet<string> ArquivosBaseViewModelProtegidos = new HashSet<string>() { $"{nameof(BaseViewModel)}.ts" };

        //private readonly HashSet<string> ArquivosBaseViewModelProtegidos = new HashSet<string>() { $"{nameof(BaseViewModel)}.ts",
        //                                                                                           $"{nameof(EntidadeViewModel)}.ts"};

        //public const string CAMINHO_DIRETORIO_EXTENSOES_NATIVA = @"Snebur.Typescript\TypeScripts\Snebur\Core\Nativo";

        public bool IsTipoHtmlReferencia { get; }

        public bool IsLink { get; set; }
        public string CaminhoProjeto { get; }
        public ConfiguracaoProjetoTypeScript ConfiguracaoProjetoTypeScript { get; }
        public FileInfo ArquivoHtmlReferencia { get; }

        public bool IsTipoAbstrato => TIPOS_ABSTRATOS.HasFlag(this.TipoArquivoTypeScript);


        public ArquivoTypeScript(ConfiguracaoProjetoTypeScript configuracaoProjetoTypeScript,
                                 string caminhoProjeto,
                                 FileInfo arquivo,
                                 int prioridadeProjeto ) : 
                                    base(arquivo, prioridadeProjeto)
        {
            this.CaminhoProjeto = caminhoProjeto;
            this.ConfiguracaoProjetoTypeScript = configuracaoProjetoTypeScript;

            //this.Projeto2 = projeto;
            this.IsLink = this.RetornarIsLink();
            this.ArquivoHtmlReferencia = this.RetornarArquivoHtmlReferencia();
            this.IsTipoHtmlReferencia = this.RetornarPossuiHtmlReferencia();

        }



        protected override EnumTipoArquivoTypeScript RetornarTipoArquivoTypeScript()
        {
            if (this.Linhas.All(x => String.IsNullOrWhiteSpace(x) || x.Trim().StartsWith("//")))
            {
                return EnumTipoArquivoTypeScript.Vazio;
            }

            if (this.Arquivo.Directory.Name == ArquivoTypeScript.REPOSITORIO_NATIVO)
            {
                return EnumTipoArquivoTypeScript.Nativo;
            }

            var nomeArquivo = this.SubExtensao;

            var subExtensao = ArquivoUtil.RetornarSubExtenmsao(nomeArquivo).ToLower();
            switch (subExtensao.ToLower())
            {
                case ".partial.ts":

                case ".statica.ts":
                case ".static.ts":

                    return EnumTipoArquivoTypeScript.ClassePartial;
                case ".test.ts":
                case ".teste.ts":
                case ".spec.ts":
                    return EnumTipoArquivoTypeScript.Teste;
                default:
                    break;
            }


            var tiposEncontrados = new Dictionary<EnumTipoArquivoTypeScript, int>();

            foreach (var tipoProcurar in TipoArquivoTypeScriptUtil.TiposProcurar)
            {
                var tipoEnum = tipoProcurar.Key;
                var procurar = tipoProcurar.Value;

                var linha = (this.Linhas.Where(x => x.TrimStart().StartsWith(procurar))).FirstOrDefault();
                if (linha != null)
                {
                    var posicao = this.Linhas.IndexOf(linha);
                    if (!(posicao >= 0))
                    {
                        throw new Exception("Posicao da linha invalida");
                    }
                    tiposEncontrados.Add(tipoEnum, posicao);
                }
            }

            if (tiposEncontrados.Count == 0)
            {
                var erro = new Exception("Nenhum tipo foi encontrado no arquivo, verificar export " + this.Arquivo.FullName);

                LogVSUtil.LogErro(erro);
                return EnumTipoArquivoTypeScript.Desconhecido;

            }

            var tipoArquivo = tiposEncontrados.OrderBy(x => x.Value).First().Key;

            if (!this.ArquivosBaseViewModelProtegidos.Contains(this.Arquivo.Name) &&
                 this.Arquivo.Name != "Entidade" &&
                 (this.Arquivo.Name.EndsWith("ViewModel.ts", StringComparison.InvariantCultureIgnoreCase) ||
                 this.Arquivo.Name.EndsWith("VM.ts", StringComparison.InvariantCultureIgnoreCase)))
            {
                switch (tipoArquivo)
                {
                    case EnumTipoArquivoTypeScript.ClasseBase:
                        return EnumTipoArquivoTypeScript.BaseClasseViewModel;
                    case EnumTipoArquivoTypeScript.ClasseExport:
                        return EnumTipoArquivoTypeScript.ClasseViewModel;
                    case EnumTipoArquivoTypeScript.ClasseBaseAbstrata:
                        return EnumTipoArquivoTypeScript.BaseClasseViewModelAbstrata;
                    case EnumTipoArquivoTypeScript.ClasseExportAbstrata:
                        return EnumTipoArquivoTypeScript.ClasseViewModelAbstrataExport;
                    default:
                        break;
                }

                throw new Exception($"o tipo de declaração da classe não é suportado para arquivos do tipo ViewModel.ts ou VM.ts");
            }

            return tipoArquivo;
        }

        protected override string RetornarNamespace()
        {
            if (!(this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Nativo ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Inteface ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.IntefaceExport ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseBaseAbstrata ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseBase ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.EnumBase ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Vazio ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Teste ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.SistemaVariaveis ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Desconhecido))
            {

                var linhasNamespace = this.Linhas.Where(x => x.TrimStart().StartsWith(TipoArquivoTypeScriptUtil.PROCURAR_NAMESPACE)).ToList();
                if (linhasNamespace.Count == 0)
                {
                    throw new Exception(String.Format("Não foi encontrado o namespace em {0} ", this.Arquivo.FullName));
                }

                if (linhasNamespace.Count > 1)
                {
                    //throw new Exception(String.Format("Foi encontrado mais de um namespace em {0} ", this.Arquivo.FullName));
                }
                var linhaNamespace = linhasNamespace.First().Trim();
                var _namespace = linhaNamespace.Substring(TipoArquivoTypeScriptUtil.PROCURAR_NAMESPACE.Length);
                _namespace = FormatacaoVSUtil.RetornarNomeFormatado(_namespace, this.Arquivo);
                return _namespace;


            }
            return null;
        }

        protected override string RetornarCaminhoClasseBase()
        {


            if ((this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseBase) ||
                (this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseBaseAbstrata) ||
                (this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseExport) ||
                (this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.BaseClasseViewModel) ||
                (this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.BaseClasseViewModelAbstrata) ||
                (this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseViewModel) ||
                (this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseViewModelAbstrataExport) ||
                (this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseExportAbstrata))
            {

                var procurar = TipoArquivoTypeScriptUtil.RetornarProcurarTipo(this.TipoArquivoTypeScript);
                var linhaClasse = this.Linhas.Where(x => x.Trim().StartsWith(procurar)).First();

                linhaClasse = linhaClasse.Trim().Substring(procurar.Length).Trim();

                var nomeClasse = linhaClasse.Split(' ').First();
                if (!nomeClasse.Contains("<"))
                {
                    linhaClasse = linhaClasse.Substring(linhaClasse.IndexOf(nomeClasse) + nomeClasse.Length);
                }
                else
                {
                    nomeClasse = linhaClasse.Split('<').First();
                    var linhaExpressao = linhaClasse.Substring(linhaClasse.IndexOf(nomeClasse) + nomeClasse.Length);
                    var parteGenerica = ExpressaoUtil.RetornarExpressaoAbreFecha(linhaExpressao, false, '<', '>');
                    linhaClasse = linhaClasse.Substring(linhaClasse.IndexOf(parteGenerica) + parteGenerica.Length);
                }

                //if (linhaClasse.Contains(">"))
                //{
                //    var posicaoFimTipoGenerico = linhaClasse.IndexOf(">");
                //    linhaClasse = linhaClasse.Substring(posicaoFimTipoGenerico);
                //}

                if (linhaClasse.Contains(TipoArquivoTypeScriptUtil.PROCURAR_EXTENDS))
                {

                    var posicao = linhaClasse.LastIndexOf(TipoArquivoTypeScriptUtil.PROCURAR_EXTENDS) + TipoArquivoTypeScriptUtil.PROCURAR_EXTENDS.Length;

                    var caminhoClasseSuperior = linhaClasse.Substring(posicao).Trim();
                    caminhoClasseSuperior = caminhoClasseSuperior.Split(" ".ToCharArray()).First();
                    caminhoClasseSuperior = Utilidade.FormatacaoVSUtil.RetornarNomeFormatado(caminhoClasseSuperior, this.Arquivo);
                    //caminhoClasseSuperior = FormatacaoUtils.RetornarNomeTipoSemGenerico(caminhoClasseSuperior);
                    if (caminhoClasseSuperior == "Object" ||
                        caminhoClasseSuperior == "Array<T>" ||
                        caminhoClasseSuperior == "Array" ||
                        caminhoClasseSuperior == "Error" ||
                        caminhoClasseSuperior == "Erro")
                    {
                        return null;
                    }

                    var segundaLetra = caminhoClasseSuperior.Substring(1, 1);
                    if (segundaLetra == ".")
                    {
                        throw new Exception(String.Format("Não utilizar atalho no caminho da classe superior \"extendida\"  {0} - ", caminhoClasseSuperior, this.Arquivo.FullName));
                    }

                    //if (!caminhoClasseSuperior.Contains(".") && !String.IsNullOrEmpty(this.Namespace))
                    //{
                    //    caminhoClasseSuperior = String.Format("{0}.{1}", this.Namespace, caminhoClasseSuperior);
                    //}

                    return caminhoClasseSuperior;
                }

            }

            return null;
        }

        protected override string RetornarNomeTipo()
        {
            if (!(this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Nativo ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClassePartial ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseStatica ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Inteface ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.IntefaceExport ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Vazio ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Teste ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Desconhecido))
            {
                var procurar = TipoArquivoTypeScriptUtil.RetornarProcurarTipo(this.TipoArquivoTypeScript);
                var linhaTipo = this.Linhas.Where(x => x.TrimStart().StartsWith(procurar)).First().Trim();
                linhaTipo = linhaTipo.Substring(procurar.Length).Trim();

                var nomeTipo = linhaTipo.Split(" ".ToCharArray()).First();
                nomeTipo = FormatacaoVSUtil.RetornarNomeFormatado(nomeTipo, this.Arquivo);


                var nomeArquivoSemExtensao = Path.GetFileNameWithoutExtension(this.Arquivo.Name);
                if (!nomeArquivoSemExtensao.Equals(nomeArquivoSemExtensao))
                {
                    throw new Exception(String.Format(" O nome {0} {1} é diferente do Arquivo: Ignorar .ts " + this.TipoArquivoTypeScript.ToString(), this.NomeTipo, this.Arquivo.Name));
                }

                if (nomeTipo.Contains(" "))
                {
                    throw new Exception(String.Format("O nome do tipo é invalido {0} em {1} ", nomeTipo, this.Arquivo.FullName));
                }

                return nomeTipo;
            }
            return null;
        }

        private bool RetornarPossuiHtmlReferencia()
        {
            //return !this.IsLink && this.IsExisteTipo && this.RetornarArquivosHtmlReferencia().Count > 0;
            return !this.IsLink && this.IsExisteTipo && this.ArquivoHtmlReferencia != null && this.ArquivoHtmlReferencia.Exists;
        }

        private FileInfo RetornarArquivoHtmlReferencia()
        {
            var nomeSemExtensao = CaminhosUtil.RetornarNomeSemExtensao(this.Arquivo.Name, true);
            var nomeArquivoShtml = $"{nomeSemExtensao}.shtml";
            var caminhoArquivo = Path.Combine(this.Arquivo.Directory.FullName, nomeArquivoShtml);
            if (File.Exists(caminhoArquivo))
            {
                return new FileInfo(caminhoArquivo);
            }
            return null;
        }

        private bool RetornarIsLink()
        {
            return !DiretorioUtil.IsDiretorioFilho(this.Arquivo.Directory, new DirectoryInfo(this.CaminhoProjeto));

        }

    }

}
