using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.IO;
using Snebur.VisualStudio.Utilidade;
using Snebur.Utilidade;

namespace Snebur.VisualStudio.Projeto.TypeScript
{
    public class ArquivoTypeScript : BaseArquivoTypeScript
    {
        public const string CAMINHO_DIRETORIO_EXTENSOES_NATIVA = @"Snebur.Framework\Snebur.Typescript\TypeScripts\Snebur\Core\Nativo";

        public const string PROCURAR_NAMESPACE = "namespace ";
        public const string PROCURAR_CLASSE_BASE = "class ";
        public const string PROCURAR_CLASSE_BASE_ABSTRATA = "abstract class ";
        public const string PROCURAR_CLASSE_EXPORT = "export class ";
        public const string PROCURAR_CLASSE_EXPORT_ABSTRATA = "export abstract class";
        public const string PROCURAR_ENUM_BASE = "enum ";
        public const string PROCURAR_ENUM_EXPORT = "export enum ";
        public const string PROCURAR_INTERFACE = "interface ";
        public const string PROCURAR_INTERFACE_EXPORT = "export interface ";
        public const string PROCURAR_EXTENDS = " extends ";

        public string NomeTipo { get; set; }

        public string CaminhoTipo { get; set; }

        public bool PossuiReferenciaHtml { get; set; }


        public bool IsLink { get; set; }

        public ArquivoTypeScript(ProjetoTypeScript projeto, FileInfo arquivo, int prioridadeProjeto) : base(projeto, arquivo, prioridadeProjeto)
        {
            this.NomeTipo = this.RetornarNomeTipo();
            this.CaminhoTipo = (String.IsNullOrEmpty(this.Namespace)) ? this.NomeTipo : String.Format("{0}.{1}", this.Namespace, this.NomeTipo);
            this.Projeto = projeto;
            this.IsLink = this.RetornarIsLink();
            this.PossuiReferenciaHtml = this.RetornarPoussiHtmlReferencia();
        }

        protected override EnumTipoArquivoTypeScript RetornarTipoArquivoTypeScript()
        {

            if (this.Linhas.All(x => String.IsNullOrWhiteSpace(x)))
            {
                return EnumTipoArquivoTypeScript.Vazio;
            }

            if (this.Arquivo.Directory.FullName.EndsWith(CAMINHO_DIRETORIO_EXTENSOES_NATIVA))
            {
                return EnumTipoArquivoTypeScript.Nativo;
            }

            if (this.Arquivo.Name.EndsWith(".Partial.ts"))
            {
                return EnumTipoArquivoTypeScript.ClassePartial;
            }

            if (this.Arquivo.Name.EndsWith(".Statica.ts"))
            {
                return EnumTipoArquivoTypeScript.ClasseStatica;
            }

            if (this.Arquivo.Name.EndsWith("ViewModel.ts") || this.Arquivo.Name.EndsWith("VM.ts"))
            {
                return EnumTipoArquivoTypeScript.ClasseViewModel;
            }

            var linhasClassesBase = this.Linhas.Where(x => x.TrimStart().StartsWith(PROCURAR_CLASSE_BASE)).ToList();
            var linhasClassesBaseAbstrata = this.Linhas.Where(x => x.TrimStart().StartsWith(PROCURAR_CLASSE_BASE_ABSTRATA)).ToList();

            var linhasClassesExport = this.Linhas.Where(x => x.TrimStart().StartsWith(PROCURAR_CLASSE_EXPORT)).ToList();
            var linhasClassesAbstrata = this.Linhas.Where(x => x.TrimStart().StartsWith(PROCURAR_CLASSE_EXPORT_ABSTRATA)).ToList();

            var linhasEnumBase = this.Linhas.Where(x => x.TrimStart().StartsWith(PROCURAR_ENUM_BASE)).ToList();
            var linhasEnumExport = this.Linhas.Where(x => x.TrimStart().StartsWith(PROCURAR_ENUM_EXPORT)).ToList();
            var linhasInterface = this.Linhas.Where(x => x.TrimStart().StartsWith(PROCURAR_INTERFACE) || x.TrimStart().StartsWith(PROCURAR_INTERFACE_EXPORT)).ToList();

            if (linhasClassesBase.Count > 1)
            {
                throw new Exception("Existe mais de um classe no arquivo " + this.Arquivo.FullName);
            }

            if (linhasClassesBaseAbstrata.Count > 1)
            {
                throw new Exception("Existe mais de um classe no arquivo " + this.Arquivo.FullName);
            }

            if (linhasClassesExport.Count > 1)
            {
                throw new Exception("Existe mais de um classe no arquivo " + this.Arquivo.FullName);
            }

            if (linhasClassesAbstrata.Count > 1)
            {
                throw new Exception("Existe mais de um classe no arquivo " + this.Arquivo.FullName);
            }

            if (linhasEnumBase.Count > 1)
            {
                throw new Exception("Existe mais de um enum no arquivo " + this.Arquivo.FullName);
            }

            if (linhasEnumExport.Count > 1)
            {
                throw new Exception("Existe mais de um enum no arquivo " + this.Arquivo.FullName);
            }

            if (linhasInterface.Count > 1)
            {
                throw new Exception("Existe mais de uma interface no arquivo " + this.Arquivo.FullName);
            }

            var totalTipos = linhasClassesBase.Count +
                             linhasClassesBaseAbstrata.Count +
                             linhasClassesExport.Count +
                             linhasClassesAbstrata.Count +
                             linhasEnumBase.Count +
                             linhasEnumExport.Count +
                             linhasInterface.Count;

            if (totalTipos == 0)
            {
                throw new Exception("Nenhum tipo foi encontrado no arquivo, verificar export " + this.Arquivo.FullName);
            }

            if (totalTipos > 1)
            {
                throw new Exception("Existe mais de um tipo no arquivo " + this.Arquivo.FullName);
            }

            if (linhasClassesBase.Count == 1)
            {
                return EnumTipoArquivoTypeScript.ClasseBase;
            }

            if (linhasClassesBaseAbstrata.Count == 1)
            {
                return EnumTipoArquivoTypeScript.ClasseBaseAbstrata;
            }

            if (linhasClassesExport.Count == 1)
            {
                return EnumTipoArquivoTypeScript.ClasseExport;
            }

            if (linhasClassesAbstrata.Count == 1)
            {
                return EnumTipoArquivoTypeScript.ClasseExportAbstrata;
            }

            if (linhasEnumBase.Count == 1)
            {
                return EnumTipoArquivoTypeScript.EnumBase;
            }

            if (linhasEnumExport.Count == 1)
            {
                return EnumTipoArquivoTypeScript.EnumExport;
            }

            if (linhasInterface.Count == 1)
            {
                return EnumTipoArquivoTypeScript.Inteface;
            }

            throw new InvalidOperationException("Operação invalida no ArquivoTypeScript: Erro de programação.");

        }

        protected override string RetornarNamespace()
        {
            if (!(this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Nativo ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Inteface ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseBaseAbstrata ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseBase ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.EnumBase ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Vazio))
            {

                var linhasNamespace = this.Linhas.Where(x => x.TrimStart().StartsWith(PROCURAR_NAMESPACE)).ToList();
                if (linhasNamespace.Count == 0)
                {
                    throw new Exception(String.Format("Não foi encontrado o namespace em {0} ", this.Arquivo.FullName));
                }

                if (linhasNamespace.Count > 1)
                {
                    throw new Exception(String.Format("Foi encontrado mais de um namespace em {0} ", this.Arquivo.FullName));
                }
                var linhaNamespace = linhasNamespace.Single().Trim();
                var _namespace = linhaNamespace.Substring(PROCURAR_NAMESPACE.Length);
                _namespace = FormatacaoUtil.RetornarNomeFormatado(_namespace, this.Arquivo);
                return _namespace;


            }
            return null;
        }

        protected override string RetornarClasseSuperior()
        {


            if ((this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseBase) ||
                (this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseBaseAbstrata) ||
                (this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseExport) ||
                (this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseViewModel) ||
                (this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseExportAbstrata))
            {

                var procurar = this.RetornarProcurarTipo();
                var linhaClasse = this.Linhas.Where(x => x.Trim().StartsWith(procurar)).Single();

                linhaClasse = linhaClasse.Trim().Substring(procurar.Length);

                var nomeClasse = linhaClasse.Split(' ').First();
                if (nomeClasse.Contains("<"))
                {
                    var posicaoFimTipoGenerico = linhaClasse.IndexOf(">");
                    linhaClasse = linhaClasse.Substring(posicaoFimTipoGenerico);

                }

                //if (linhaClasse.Contains(">"))
                //{
                //    var posicaoFimTipoGenerico = linhaClasse.IndexOf(">");
                //    linhaClasse = linhaClasse.Substring(posicaoFimTipoGenerico);
                //}

                if (linhaClasse.Contains(PROCURAR_EXTENDS))
                {

                    var posicao = linhaClasse.LastIndexOf(PROCURAR_EXTENDS) + PROCURAR_EXTENDS.Length;
                    var caminhoClasseSuperior = linhaClasse.Substring(posicao).Trim();
                    caminhoClasseSuperior = caminhoClasseSuperior.Split(" ".ToCharArray()).First();
                    caminhoClasseSuperior = FormatacaoUtil.RetornarNomeFormatado(caminhoClasseSuperior, this.Arquivo);
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

        private string RetornarNomeTipo()
        {
            if (!(this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Nativo ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClassePartial ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.ClasseStatica ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Inteface ||
                  this.TipoArquivoTypeScript == EnumTipoArquivoTypeScript.Vazio))
            {
                var procurar = this.RetornarProcurarTipo();
                var linhaTipo = this.Linhas.Where(x => x.TrimStart().StartsWith(procurar)).Single().Trim();
                linhaTipo = linhaTipo.Substring(procurar.Length).Trim();

                var nomeTipo = linhaTipo.Split(" ".ToCharArray()).First();
                nomeTipo = FormatacaoUtil.RetornarNomeFormatado(nomeTipo, this.Arquivo);


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

        private string RetornarProcurarTipo()
        {
            switch (this.TipoArquivoTypeScript)
            {
                case EnumTipoArquivoTypeScript.ClasseBaseAbstrata:

                    return PROCURAR_CLASSE_BASE_ABSTRATA;

                case EnumTipoArquivoTypeScript.ClasseBase:

                    return PROCURAR_CLASSE_BASE;

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

                default:
                    throw new NotSupportedException("TipoArquivoTypeScript não suportado " + this.TipoArquivoTypeScript.ToString());
            }

        }

        private bool RetornarPoussiHtmlReferencia()
        {
            return !this.IsLink && this.RetornarArquivosHtmlReferencia().Count > 0;
        }

        private List<FileInfo> _arquivosHtmlReferencias;
        internal List<FileInfo> RetornarArquivosHtmlReferencia()
        {
            if (_arquivosHtmlReferencias == null)
            {
                _arquivosHtmlReferencias = new List<FileInfo>();
                var diretorio = this.Arquivo.Directory;
                var nomeArquivoSemExtensao = Path.GetFileNameWithoutExtension(this.Arquivo.FullName);
                if (nomeArquivoSemExtensao.EndsWith(".shtml"))
                {
                    nomeArquivoSemExtensao = nomeArquivoSemExtensao.Replace(".shtml", String.Empty);
                }
                var nomeArquivoHtmlReferencia = String.Format("{0}.shtml", nomeArquivoSemExtensao);

                var caminhoArquivoHtmlReferencia = Path.Combine(diretorio.FullName, nomeArquivoHtmlReferencia);
                if (File.Exists(caminhoArquivoHtmlReferencia))
                {
                    _arquivosHtmlReferencias.Add(new FileInfo(caminhoArquivoHtmlReferencia));
                }

                var arquivos = diretorio.GetFiles(String.Format("{0}_*.shtml", nomeArquivoSemExtensao));
                _arquivosHtmlReferencias.AddRange(arquivos);

            }
            return _arquivosHtmlReferencias;


        }

        private bool RetornarIsLink()
        {
            return DiretorioUtil.IsDiretorioFilho(new DirectoryInfo(this.Projeto.CaminhoProjeto), this.Arquivo.Directory);

        }

    }

}
