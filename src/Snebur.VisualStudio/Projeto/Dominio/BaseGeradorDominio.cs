using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Reflection;
using System.IO;
using Snebur.VisualStudio.Utilidade;
using Snebur.VisualStudio.Reflexao;
using Snebur.Utilidade;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public abstract class BaseGeradorDominio : IDisposable
    {

        protected const string TAB = "    ";

        protected string CaminhoArquivoDestino { get; set; }

        protected List<Type> TodosTipo { get; set; }

        protected List<Type> TiposDominio { get; set; }

        protected ConfiguracaoProjetoDominio ConfiguracaoDominio { get; set; }

        protected bool ProjetoSneburDominio { get; set; }

        protected string NomeProjeto { get; set; }

        
        public BaseGeradorDominio(ConfiguracaoProjetoDominio configuracaoDominio,
                                  List<Type> todosTipos, string nomeProjeto)
        {

            this.ConfiguracaoDominio = configuracaoDominio;
            this.NomeProjeto = nomeProjeto;
            this.TodosTipo = todosTipos;
            this.CaminhoArquivoDestino = this.RetornarCaminhoArquivoDestino();
            this.ProjetoSneburDominio = this.ConfiguracaoDominio.Namespace == "Snebur.Dominio";
            this.TiposDominio = this.RetornarRetornarTiposDominios();
        }

        public void Gerar()
        {
            if (this.TiposDominio.Count > 0)
            {
                var sb = new StringBuilder();

                sb.AppendLine(String.Format("//Data : {0}", DateTime.Now.ToLongDateString()));
                sb.AppendLine(String.Format("//Hora : {0}", DateTime.Now.ToLongTimeString()));
                sb.AppendLine(String.Format("//@Namespace: {0}", this.ConfiguracaoDominio.Namespace));
                sb.AppendLine(String.Format("//@PrioridadeDominio: {0}", this.ConfiguracaoDominio.PrioridadeDominio));
                sb.AppendLine(String.Format("//@Globalizar: {0}", this.ConfiguracaoDominio.Globalizar));
                sb.AppendLine(String.Format("//@Dominios depedentes: [{0}]", String.Join(",", this.ConfiguracaoDominio.DominiosDepentendes)));
                sb.AppendLine("");

                sb.AppendLine(this.RetornarConteudoTypeScript());

                var conteudoTypeScript = sb.ToString();

                ArquivoUtil.SalvarArquivoTexto(this.CaminhoArquivoDestino, conteudoTypeScript, true);
            }

        }
 
        protected List<Type> RetornarSubTipos(Type tipo)
        {
            return this.TodosTipo.Where(x => TipoUtil.TipoSubTipo(x, tipo)).ToList();
        }

        protected List<Type> RetornarSubTiposTipoBase(Type tipoBase)
        {   
            return TipoUtil.RetornarBaseIgualTipoBase(this.TodosTipo, tipoBase);
        }

        protected List<Type> RetornarTiposOrdenados(List<Type> tipos, params Type[] tiposBase)
        {
            var ordenar = new OrdenarTiposHeranca(tipos, tiposBase);
            return ordenar.RetornarTiposOrdenados();
        }

        #region Retornar Tipos

        protected List<Type> RetornarTiposAtributos()
        {

            var tiposAtributoValidacao = this.RetornarSubTipos(typeof(ValidationAttribute)).ToList();
            var tiposAtributoValidacaoAsync = this.RetornarSubTiposTipoBase(AjudanteAssembly.TipoBaseAtributoValidacaoAsync).ToList();
            var tiposAtributoDominio = this.RetornarSubTiposTipoBase(AjudanteAssembly.TipoBaseAtributoDominio).ToList();

            var tipoAtributoChaveEstrangeira = this.RetornarSubTiposTipoBase(typeof(ForeignKeyAttribute)).SingleOrDefault();
            var tiposAtributoOcultarColuna = this.RetornarSubTiposTipoBase(typeof(ScaffoldColumnAttribute)).SingleOrDefault();


            //var tiposAtributoDominioAsync = this.RetornarSubTiposTipoBase(AjudanteAssembly.TipoBaseAtributoDominio).ToList();
            foreach (var tipo in tiposAtributoDominio.ToList())
            {
                tiposAtributoDominio.AddRange(this.RetornarSubTipos(tipo));
            }

            var tiposAtributos = new List<Type>();
            tiposAtributos.AddRange(tiposAtributoValidacaoAsync);
            tiposAtributos.AddRange(tiposAtributoValidacao);
            tiposAtributos.AddRange(tiposAtributoDominio);

            if (tipoAtributoChaveEstrangeira != null)
            {
                tiposAtributos.Add(tipoAtributoChaveEstrangeira);
            }

            if (tiposAtributoOcultarColuna != null)
            {
                tiposAtributos.Add(tiposAtributoOcultarColuna);
            }

            tiposAtributos = tiposAtributos.Distinct().ToList();
            //tiposAtributos.AddRange(tiposAtributoDominioAsync);

            TipoUtil.RemoverTipo(tiposAtributos, AjudanteAssembly.TipoBaseAtributoValidacaoAsync);
            TipoUtil.RemoverTipo(tiposAtributos, AjudanteAssembly.TipoBaseAtributoDominio);

            tiposAtributos = TipoUtil.IgnorarAtributo(tiposAtributos, AjudanteAssembly.NomeTipoIgnorarAtributoTS);
            return tiposAtributos;

        }

        #endregion

        #region Métodos privados

        private string RetornarCaminhoArquivoDestino()
        {
            var formatacao = this.NomeProjeto.EndsWith(".Dominio") ? this.NomeProjeto.Replace(".Dominio", String.Empty) : this.NomeProjeto;
            var nomeArquivo = String.Format("{0}.Dominio.{1}.ts", formatacao, this.RetornarNomeArquivoDestino());
            return Path.Combine(this.ConfiguracaoDominio.CaminhoDominioTypeScript, nomeArquivo);
        }
        #endregion

        #region Métodos Abstratos

        protected abstract string RetornarConteudoTypeScript();

        protected abstract string RetornarNomeArquivoDestino();

        protected abstract List<Type> RetornarRetornarTiposDominios();

        #endregion

        #region IDisposable

        public void Dispose()
        {
            //System.Reflection.Assembly.LoadFile("")
        }

        #endregion

    }
}
