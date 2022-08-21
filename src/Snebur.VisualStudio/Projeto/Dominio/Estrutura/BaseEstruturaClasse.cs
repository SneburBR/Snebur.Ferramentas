using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Reflexao;
using Snebur.VisualStudio.Utilidade;
using System.Reflection;
using Snebur.Utilidade;

namespace Snebur.VisualStudio.Projeto.Dominio.Estrutura
{
    public abstract class BaseEstruturaClasse : BaseEstrutura
    {

        public string CaminhoTipoBase { get; set; }

        public string ImplentacaoInterfaces { get; set; }

        public string Abastrada { get; set; }

        public EstruturaConstrutor EstruturaConstrutor { get; set; }

        public List<EstruturaPropriedade> Propriedades { get; set; }
        public List<string> LinhasExtra { get; set; }

        public BaseEstruturaClasse(Type tipo) : base(tipo)
        {
            this.LinhasExtra = new List<string>();
            this.CaminhoTipoBase = TipoUtil.RetornarCaminhoTipo(tipo.BaseType);
            this.Propriedades = this.RetornarEstruturasPropriedades(true);
            this.EstruturaConstrutor = this.RetornarEstruturaConstrutor();
            this.Abastrada = (tipo.IsAbstract) ? " abstract" : string.Empty;
            this.ImplentacaoInterfaces = this.RetornarImplentacaoInterfaces();
        }

        public List<string> RetornarLinhasTypeScriptClasse()
        {
            var linhas = new List<string>();


            linhas.Add(String.Format("export{0} class {1} extends {2}{3}", this.Abastrada, this.NomeTipo, this.CaminhoTipoBase, this.ImplentacaoInterfaces));
            linhas.Add("{");

            if (this.Propriedades.Count() > 0)
            {
                linhas.Add("");
                linhas.Add(String.Format("{0}//#region Propriedades", TAB));
                linhas.Add("");
                foreach (EstruturaPropriedadePublica estruturaPropriedade in this.Propriedades.OfType<EstruturaPropriedadePublica>())
                {
                    linhas.Add(estruturaPropriedade.RetornarLinhaValorVariavelTypeScript(TAB));
                }
                linhas.Add("");
                foreach (var estruturaPropriedade in this.Propriedades.OfType<EstruturaPropriedadePublica>())
                {
                    linhas.Add("");
                    linhas.AddRange(estruturaPropriedade.RetornarLinhasTypeScript(TAB));
                }

                foreach (var estruturaPropriedade in this.Propriedades.OfType<EstruturaPropriedadeEstatica>())
                {
                    linhas.Add("");
                    linhas.AddRange(estruturaPropriedade.RetornarLinhasTypeScript(TAB));
                }
                linhas.Add(String.Format("{0}//#endregion", TAB));
            }

            if (this.LinhasExtra.Count() > 0)
            {
                linhas.Add(String.Empty);
                linhas.Add(String.Format("{0}//#region Extra", TAB));
                linhas.AddRange(this.LinhasExtra);
                linhas.Add(String.Format("{0}//#endregion", TAB));
                linhas.Add(String.Empty);
            }

            linhas.Add("");
            linhas.Add(String.Format("{0}//#region Construtor", TAB));
            linhas.Add("");
            linhas.AddRange(this.EstruturaConstrutor.RetornarLinhasTypeScript(TAB));
            linhas.Add("");
            linhas.Add(String.Format("{0}//#endregion", TAB));
            linhas.Add("}");
            return linhas;
        }

        protected virtual List<EstruturaPropriedade> RetornarEstruturasPropriedades(bool ignoratTipoBase)
        {
            var estruturasPropriedade = new List<EstruturaPropriedade>();
            var propriedadesPublica = AjudantePropriedades.RetornarPropriedadesClassePublicas(this.Tipo);
            foreach (var propriedade in propriedadesPublica)
            {
                estruturasPropriedade.Add(new EstruturaPropriedadePublica(propriedade));
            }

            var propriedadesInterface = AjudantePropriedades.RetornarPropriedadesInterface(this.Tipo);
            foreach(var propriedadeInterface in propriedadesInterface)
            {
                estruturasPropriedade.Add(new EstruturaPropriedadePublica(propriedadeInterface));
            }

            return estruturasPropriedade;
        }

        private EstruturaConstrutor RetornarEstruturaConstrutor()
        {
            return new EstruturaConstrutor(this.Tipo, this.RetornarEstruturasPropriedades(false).OfType<EstruturaPropriedadePublica>().ToList());
        }

        private string RetornarTypeScriptPropriedades(string tabInicial)
        {
            return String.Join(System.Environment.NewLine + System.Environment.NewLine, this.Propriedades.Select(x => x.RetornarLinhasTypeScript(tabInicial)));
        }

        private string RetornarImplentacaoInterfaces()
        {
            var interfaces = this.Tipo.GetInterfaces().Where(x => !x.Namespace.StartsWith("System")).ToList();
            interfaces = interfaces.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarInterfaceTS)).ToList();
            if (interfaces.Count() > 0)
            {
                if (TipoUtil.TipoSubTipo(this.Tipo, AjudanteAssembly.TipoEntidade) && interfaces.Any(x => x.Name == AjudanteAssembly.NomeTipoInterfaceIImagem))
                {
                    this.LinhasExtra.Add(String.Format("{0}public OrigemImagem : Snebur.ServicoArquivo.OrigemImagem;", TAB));
                    this.LinhasExtra.Add(String.Format("{0}public ImagemPequenaBase64 : string;", TAB));
                }
                else if (TipoUtil.TipoSubTipo(this.Tipo, AjudanteAssembly.TipoEntidade) && interfaces.Any(x => x.Name == AjudanteAssembly.NomeTipoInterfaceIArquivo))
                {
                    this.LinhasExtra.Add(String.Format("{0}public OrigemArquivo : Snebur.ServicoArquivo.OrigemArquivo;", TAB));
                }
                return string.Format(" implements {0} ", String.Join(", ", interfaces.Select(x => TipoUtil.RetornarCaminhoTipo(x))));
            }
            return String.Empty;
        }
    }
}
