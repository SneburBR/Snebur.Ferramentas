using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Snebur.Dominio;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public class EstruturaPropriedadePublica : EstruturaPropriedade
    {
        public string NomePriprieadePrivada { get; set; }
        public bool IsPropriedadeInterace { get; set; }

        public string NomeVariavelPrivada { get; set; }
        public PropertyInfo PropriedadeRelacaoChaveEstrangeira { get; }
        public bool IsPropriedadeIsAtivo { get; private set; }

        //public string NomePropriedadeAtribuir { get; set; }

        public EstruturaPropriedadePublica(PropertyInfo propriedade, PropertyInfo propriedadeRelacaoChaveEstrangeira) :
            base(propriedade)
        {
            this.NomePriprieadePrivada = String.Format("_{0}", TextoUtil.RetornarInicioMinusculo(this.Propriedade.Name, 2));
            this.PropriedadeRelacaoChaveEstrangeira = propriedadeRelacaoChaveEstrangeira;

            var atributoInterfacePropriedade = propriedade.GetCustomAttributes().Where(k => k.GetType().Name == AjudanteAssembly.NomeTipoAtributoProprieadeInterface).SingleOrDefault();
            if (atributoInterfacePropriedade != null)
            {
                this.IsPropriedadeInterace = true;
                this.NomeVariavelPrivada = Convert.ToString(Snebur.Utilidade.ReflexaoUtil.RetornarValorPropriedade(atributoInterfacePropriedade, "NomePropriedade"));
                this.NomePropriedade = this.Propriedade.Name.Split('.').Last();
            }
            else
            {
                this.NomeVariavelPrivada = this.NomePriprieadePrivada;
            }

            this.IsPropriedadeIsAtivo = TipoUtil.TipoSubTipo(propriedade.DeclaringType, typeof(Entidade)) &&
                                        TipoUtil.TipoImplementaInterface(propriedade.DeclaringType, typeof(IAtivo)) &&
                                        this.NomePropriedade == nameof(IAtivo.IsAtivo);

        }

        public string RetornarLinhaValorVariavelCampoPrivadoTypeScript(string tabInicial)
        {
            if (!this.IsPropriedadeInterace)
            {
                return String.Format("{0}private {1} : {2} = {3};", tabInicial, this.NomePriprieadePrivada, this.CaminhoTipo, this.ValorPropriedade);
            }
            return String.Empty;
        }

        public override List<string> RetornarLinhasTypeScript(string tabInicial)
        {
            var linhas = new List<string>();

            linhas.Add(String.Format("{0}public get {1}(): {2} ", tabInicial, this.NomePropriedade, this.CaminhoTipo));
            linhas.Add(String.Format("{0}{{", tabInicial));

            if (this.IsPropriedadeInterace)
            {
                linhas.Add(String.Format("{0}{1}return this.{2} as any;", tabInicial, TAB, this.NomeVariavelPrivada));
            }
            else
            {

                if (this.PropriedadeRelacaoChaveEstrangeira != null)
                {
                    linhas.Add(String.Format("{0}{1}return this.RetornarValorChaveEstrangeira(\"{2}\",\"{3}\", this.{4});", tabInicial, TAB, this.NomePropriedade, this.PropriedadeRelacaoChaveEstrangeira.Name, this.NomeVariavelPrivada));
                }
                else if(this.IsPropriedadeIsAtivo)
                {
                    linhas.Add($"{tabInicial}{TAB}return this.RetornarValorPropriedadeIsAtivo(this.{this.NomeVariavelPrivada});");
                }
                else
                {
                    linhas.Add(String.Format("{0}{1}return this.{2};", tabInicial, TAB, this.NomeVariavelPrivada));
                }


            }


            linhas.Add(String.Format("{0}}}", tabInicial));

            linhas.Add(String.Format("{0}public set {1}(value: {2})  ", tabInicial, this.NomePropriedade, this.CaminhoTipo));
            linhas.Add(String.Format("{0}{{", tabInicial));

            var metodoNotificarPropriedadeAlterada = this.RetornarMetodoNotificarPropriedadeAlterada();
            var valor = this.RetornarValor();

            linhas.Add($"{tabInicial}{TAB}this.{metodoNotificarPropriedadeAlterada}(\"{this.NomePropriedade}\", this.{this.NomeVariavelPrivada}, this.{this.NomeVariavelPrivada} = {valor});");
            //linhas.Add(String.Format("{0}{1}this.NotificarPropriedadeAlterada(\"{2}\", this.{3}, this.{3} = value);", tabInicial, TAB, this.NomePropriedade, this.NomeVariavelPrivada));


            linhas.Add(String.Format("{0}}}", tabInicial));

            return linhas;
        }

        private string RetornarMetodoNotificarPropriedadeAlterada()
        {
            if (TipoUtil.TipoIgualOuSubTipo(this.Propriedade.PropertyType, typeof(Entidade)))
            {
                return ProjetoDominio.NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_RELACAO;
            }
            if (TipoUtil.TipoIgualOuSubTipo(this.Propriedade.PropertyType, typeof(BaseTipoComplexo)))
            {
                return ProjetoDominio.NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA_TIPO_COMPLEXO;
            }
            return ProjetoDominio.NOME_METODO_NOTIFICAR_PROPRIEDADE_ALTERADA;
        }

        private string RetornarValor()
        {
            if (TipoUtil.TipoIgualOuSubTipo(this.Propriedade.PropertyType, typeof(BaseTipoComplexo)))
            {
                return "value.Clone()";
            }
            return "value";

        }
    }
}
