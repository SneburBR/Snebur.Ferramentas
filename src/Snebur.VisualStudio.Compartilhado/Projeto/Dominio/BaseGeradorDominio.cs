using Snebur.Dominio.Atributos;
using Snebur.Linq;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;
using Snebur.VisualStudio.Utilidade;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;

namespace Snebur.VisualStudio
{
    public abstract class BaseGeradorDominio : IDisposable
    {

        protected const string TAB = "    ";

        protected string CaminhoArquivoDestino { get; set; }
        protected HashSet<Type> TodosTipo { get; set; }
        protected HashSet<Type> TiposDominio { get; set; }
        protected ConfiguracaoProjetoDominio ConfiguracaoDominio { get; set; }
        protected bool ProjetoSneburDominio { get; set; }
        protected string NomeProjeto { get; set; }
       protected string CaminhoProjeto { get; set; }

        public BaseGeradorDominio(ConfiguracaoProjetoDominio configuracaoDominio, 
                                  string caminhoProjeto,
                                  List<Type> todosTipos, 
                                  string nomeProjeto)
        {

            this.ConfiguracaoDominio = configuracaoDominio;
            this.CaminhoProjeto = caminhoProjeto;
            this.NomeProjeto = nomeProjeto;
            this.TodosTipo = todosTipos.ToHashSet();
            this.CaminhoArquivoDestino = this.RetornarCaminhoArquivoDestino();
            this.ProjetoSneburDominio = this.ConfiguracaoDominio.Namespace == "Snebur.Dominio";
            this.TiposDominio = this.RetornarRetornarTiposDominios();

        }

        public void Gerar()
        {
            if (this.TiposDominio.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("/*eslint-disable*/");
                sb.AppendLine(String.Format("//Data : {0}", DateTime.Now.ToLongDateString()));
                sb.AppendLine(String.Format("//Hora : {0}", DateTime.Now.ToLongTimeString()));
                sb.AppendLine(String.Format("//@Namespace: {0}", this.ConfiguracaoDominio.Namespace));
                sb.AppendLine(String.Format("//@PrioridadeDominio: {0}", this.ConfiguracaoDominio.PrioridadeDominio));
                sb.AppendLine(String.Format("//@Globalizar: {0}", this.ConfiguracaoDominio.Globalizar));
                sb.AppendLine(String.Format("//@Dominios dependentes: [{0}]", String.Join(",", this.ConfiguracaoDominio.DominiosDepentendes.Select(x => x.Nome))));
                sb.AppendLine(this.RetornarConteudoTypeScript());

                var conteudoTypeScript = FormatarDocumentoUtil.RetornarConteudoFormatado(sb.ToString(), false);
                ArquivoUtil.SalvarArquivoTexto(this.CaminhoArquivoDestino, conteudoTypeScript);
            }
        }

        protected List<Type> RetornarSubTipos(Type tipo)
        {
            return this.TodosTipo.Where(x => TipoUtil.TipoSubTipo(x, tipo)).ToList();
        }

        protected List<Type> RetornarSubTiposTipoBase(Type tipoBase)
        {
            return TipoUtil.RetornarBaseIgualTipoBase(this.TodosTipo.ToList(), tipoBase);
        }

        protected List<Type> RetornarTiposOrdenados(List<Type> tipos, params Type[] tiposBase)
        {
            var ordenar = new OrdenarTiposHeranca(tipos, tiposBase);
            return ordenar.RetornarTiposOrdenados();
        }

        #region Retornar Tipos

        protected HashSet<Type> RetornarTiposAtributos()
        {

            var tiposAtributoValidacao = this.RetornarSubTipos(typeof(ValidationAttribute)).ToList();

            var tiposAtributoValidacaoAsync = this.RetornarSubTiposTipoBase(AjudanteAssembly.TipoBaseAtributoValidacaoAsync).ToList();
            var tiposAtributoValidacaoEntidades = this.RetornarSubTipos(typeof(BaseValidacaoEntidadeAttribute)).ToList();
            var tiposAtributoDominio = this.RetornarSubTiposTipoBase(AjudanteAssembly.TipoBaseAtributoDominio).ToList();
            var tiposPropriedadeComputadaServico = this.RetornarSubTiposTipoBase(AjudanteAssembly.TipoBasePropriedadeComputada).ToList();

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
            tiposAtributos.AddRange(tiposAtributoValidacaoEntidades);
            tiposAtributos.AddRange(tiposAtributoDominio);
            tiposAtributos.AddRange(tiposPropriedadeComputadaServico);

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
            return tiposAtributos.ToHashSet();

        }

        #endregion

        #region Métodos privados

        private string RetornarCaminhoArquivoDestino()
        {
            var formatacao = this.NomeProjeto.EndsWith(".Dominio") ? this.NomeProjeto.Replace(".Dominio", String.Empty) : this.NomeProjeto;
            var nomeArquivo = String.Format("{0}.Dominio.{1}.ts", formatacao, this.RetornarNomeArquivoDestino());

            return Path.Combine(this.ConfiguracaoDominio.RetornarCaminhoAbsolutoDominioTypeScript(this.CaminhoProjeto), nomeArquivo);
        }


        #endregion

        #region Métodos Abstratos

        protected abstract string RetornarConteudoTypeScript();
        protected abstract string RetornarNomeArquivoDestino();
        protected abstract HashSet<Type> RetornarRetornarTiposDominios();

        #endregion

        #region IDisposable

        public void Dispose()
        {
            //System.Reflection.Assembly.LoadFile("")
        }

        #endregion

    }
}
