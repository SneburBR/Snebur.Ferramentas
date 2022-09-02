using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public abstract class BaseEstruturaClasse : BaseEstrutura
    {

        public string CaminhoTipoBase { get; set; }

        public string ImplentacaoInterfaces { get; set; }

        public string Abastrada { get; set; }

        public EstruturaConstrutor EstruturaConstrutor { get; }
        public List<EstruturaPropriedade> Propriedades { get; }
        public Dictionary<string, PropertyInfo> PropriedadesRelacaoChaveEstrangeira { get; }
        public List<string> LinhasExtra { get; set; }

        public BaseEstruturaClasse(Type tipo) : base(tipo)
        {
            this.LinhasExtra = new List<string>();
            this.CaminhoTipoBase = TipoUtil.RetornarCaminhoTipoTS(tipo.BaseType);
            this.PropriedadesRelacaoChaveEstrangeira = this.RetornarPropriedadesRelacaoChaveEstrangeiras();
            this.Propriedades = this.RetornarEstruturasPropriedades(true);
            this.EstruturaConstrutor = this.RetornarEstruturaConstrutor();
            this.Abastrada = (tipo.IsAbstract) ? " abstract" : String.Empty;
            this.ImplentacaoInterfaces = this.RetornarImplentacaoInterfaces();

        }

        private Dictionary<string, PropertyInfo> RetornarPropriedadesRelacaoChaveEstrangeiras()
        {
            var dicionario = new Dictionary<string, PropertyInfo>();

            var propriedades = AjudantePropriedades.RetornarPropriedadesClassePublicas(this.Tipo);

            var propriedadesRelacaoChavaEstrangeira = propriedades.Where(x => PropriedadeUtil.PossuiAtributo(x, typeof(ChaveEstrangeiraAttribute).Name)).ToList();

            foreach (var propriedadeRelacao in propriedadesRelacaoChavaEstrangeira)
            {
                var atributo = propriedadeRelacao.GetCustomAttributes().Where(x => x.GetType().Name == typeof(ChaveEstrangeiraAttribute).Name).Single();
                var nomePropriedade = ReflexaoUtil.RetornarValorPropriedade(atributo, "NomePropriedade").ToString();
                var proprieadeChaveEstrangeira = propriedades.Where(x => x.Name == nomePropriedade).SingleOrDefault();
                if(proprieadeChaveEstrangeira == null)
                {
                    throw new Exception($"Não foi encontrada a propriedade da chave estrangeira '{nomePropriedade}' no tipo '{this.Tipo.Name}'");
                }
                dicionario.Add(nomePropriedade, propriedadeRelacao);
            }
            return dicionario;
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
                    linhas.Add(estruturaPropriedade.RetornarLinhaValorVariavelCampoPrivadoTypeScript(TAB));
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

                foreach (var estruturaPropriedade in this.Propriedades.OfType<EstruturaPropriedadeEspecializada>())
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
            var propriedadesEspecializada = AjudantePropriedades.RetornarPropriedadesClasseEspecializada(this.Tipo);

            foreach (var propriedade in propriedadesPublica)
            {
                PropertyInfo propriedadeRelacaoChaveEstrangeira;
                this.PropriedadesRelacaoChaveEstrangeira.TryGetValue(propriedade.Name, out propriedadeRelacaoChaveEstrangeira);
                estruturasPropriedade.Add(new EstruturaPropriedadePublica(propriedade, propriedadeRelacaoChaveEstrangeira));
            }

            var propriedadesInterface = AjudantePropriedades.RetornarPropriedadesInterface(this.Tipo);
            foreach (var propriedadeInterface in propriedadesInterface)
            {
                estruturasPropriedade.Add(new EstruturaPropriedadePublica(propriedadeInterface, null));
            }

            foreach (var propriedade in propriedadesEspecializada)
            {
                estruturasPropriedade.Add(new EstruturaPropriedadeEspecializada(propriedade));
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
            var interfacesTipoBase = this.Tipo.BaseType.GetInterfaces().Where(x => !x.Namespace.StartsWith("System")).ToList();
            interfaces = interfaces.Except(interfacesTipoBase, new TipoIgual()).ToList();
            interfaces = interfaces.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarInterfaceTS)).ToList();


            if (interfaces.Count() > 0)
            {
                if (TipoUtil.TipoSubTipo(this.Tipo, AjudanteAssembly.TipoEntidade) && interfaces.Any(x => x.Name == AjudanteAssembly.NomeTipoInterfaceIImagem))
                {
                    this.AdicionarLinhasExtrasInterfaceImagm();
                }
                else if (TipoUtil.TipoSubTipo(this.Tipo, AjudanteAssembly.TipoEntidade) && interfaces.Any(x => x.Name == AjudanteAssembly.NomeTipoInterfaceIArquivo))
                {
                    this.LinhasExtra.Add(String.Format("{0}public OrigemArquivo : Snebur.ServicoArquivo.OrigemArquivo;", TAB));
                }
                return String.Format(" implements {0} ", String.Join(", ", interfaces.Select(x => TipoUtil.RetornarCaminhoTipoTS(x))));
            }
            return String.Empty;
        }

        public class TipoIgual : IEqualityComparer<Type>
        {
            public bool Equals(Type tipo1, Type tipo2)
            {
                return tipo1.Name == tipo2.Name && tipo1.Namespace == tipo2.Namespace;
            }

            public int GetHashCode(Type obj)
            {
                var nome = (obj.Namespace + "." + obj.Name).ToString();
                return nome.GetHashCode();
            }
        }



        private void AdicionarLinhasExtrasInterfaceImagm()
        {
            this.LinhasExtra.Add(String.Empty);
            this.LinhasExtra.Add(String.Format("{0}public readonly EventoImagemLocalCarregada = new Evento<ImagemLocalCarregadaEventArgs>(this);", TAB));
            this.LinhasExtra.Add(String.Format("{0}public readonly EventoImagemLocalCarregando = new Evento<EventArgs>(this);", TAB));
            this.LinhasExtra.Add(String.Format("{0}public readonly EventoImagemServidorCarregada = new Evento<ImagemServidorCarregadaEventArgs>(this);", TAB));
            this.LinhasExtra.Add(String.Empty);
            this.LinhasExtra.Add(String.Format("{0}public get OrigemImagem(): Snebur.ServicoArquivo.OrigemImagem", TAB));
            this.LinhasExtra.Add(String.Format("{0}{{", TAB));
            this.LinhasExtra.Add(String.Format("{0}{0}return this.OrigemArquivo;", TAB));
            this.LinhasExtra.Add(String.Format("{0}}}", TAB));
            this.LinhasExtra.Add(String.Format("{0}public set OrigemImagem(value: Snebur.ServicoArquivo.OrigemImagem)", TAB));
            this.LinhasExtra.Add(String.Format("{0}{{", TAB));
            this.LinhasExtra.Add(String.Format("{0}{0}this.OrigemArquivo = value;", TAB));
            this.LinhasExtra.Add(String.Format("{0}}}", TAB));
            this.LinhasExtra.Add(String.Empty);

            this.LinhasExtra.Add(String.Format("{0}public get IsExisteApresentacao(): boolean", TAB));
            this.LinhasExtra.Add(String.Format("{0}{{", TAB));
            this.LinhasExtra.Add(String.Format("{0}{0}return this.IsExisteMiniatura && this.IsExistePequena && this.IsExisteMedia && this.IsExisteGrande;", TAB));
            this.LinhasExtra.Add(String.Format("{0}}}", TAB));

            this.LinhasExtra.Add(String.Empty);

            this.LinhasExtra.Add(String.Format("{0}public ExisteImagem(tamanhoImagem: d.EnumTamanhoImagem): boolean", TAB));
            this.LinhasExtra.Add(String.Format("{0}{{", TAB));
            this.LinhasExtra.Add(String.Format("{0}{0}return u.ImagemUtil.ExisteImagem(this, tamanhoImagem);", TAB));
            this.LinhasExtra.Add(String.Format("{0}}}", TAB));

            this.LinhasExtra.Add(String.Empty);

            this.LinhasExtra.Add(String.Format("{0}public RetornarUrlImagem(dimensaoRecipiente: d.Dimensao): string;", TAB));
            this.LinhasExtra.Add(String.Format("{0}public RetornarUrlImagem(tamanhoImagem: d.EnumTamanhoImagem): string;", TAB));
            this.LinhasExtra.Add(String.Format("{0}public RetornarUrlImagem(tamanhoImagemOuDimensao: d.EnumTamanhoImagem|d.Dimensao): string", TAB));
            this.LinhasExtra.Add(String.Format("{0}{{", TAB));
            this.LinhasExtra.Add(String.Format("{0}{0}return u.ImagemUtil.RetornarUrlImagem(this, tamanhoImagemOuDimensao);", TAB));
            this.LinhasExtra.Add(String.Format("{0}}}", TAB));
            this.LinhasExtra.Add(String.Empty);
            this.LinhasExtra.Add(String.Format("{0}public InicializarImagem(arquivo: SnBlob, informacaoImagem: IInformacaoImagem): void", TAB));
            this.LinhasExtra.Add(String.Format("{0}{{", TAB));
            this.LinhasExtra.Add(String.Format("{0}{0}return u.ImagemUtil.InicializarImagem(this, arquivo, informacaoImagem);", TAB));
            this.LinhasExtra.Add(String.Format("{0}}}", TAB));
        }
    }
}
