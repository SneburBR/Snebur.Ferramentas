using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Reflection;

using Snebur.VisualStudio.Utilidade;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio.Projeto.Dominio.Estrutura
{
    public class EstruturaPropriedadePublica : EstruturaPropriedade
    {
        public string NomePriprieadePrivada { get; set; }
        public bool IsPropriedadeInterace { get; set; }

        public string NomePropriedadeRetornar { get; set; }
        //public string NomePropriedadeAtribuir { get; set; }


        public EstruturaPropriedadePublica(PropertyInfo propriedade) : base(propriedade)
        {
            this.NomePriprieadePrivada = String.Format("_{0}", TextoUtil.RetornarMinusculo(this.Propriedade.Name, 0, 2));

            var atributoInterfacePropriedade = propriedade.GetCustomAttributes().Where(k => k.GetType().Name == AjudanteAssembly.NomeTipoAtributoProprieadeInterface).SingleOrDefault();
            if (atributoInterfacePropriedade != null)
            {
                this.IsPropriedadeInterace = true;
                this.NomePropriedadeRetornar = Convert.ToString(Snebur.Utilidade.ReflexaoUtil.RetornarValorPropriedade(atributoInterfacePropriedade, "NomePropriedade"));
                this.NomePropriedade = this.Propriedade.Name.Split('.').Last();
            }
            else
            {
                this.NomePropriedadeRetornar = this.NomePriprieadePrivada;
            }
        }

        public string RetornarLinhaValorVariavelTypeScript(string tabInicial)
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

            linhas.Add(String.Format("{0}public get {1}() : {2} ", tabInicial, this.NomePropriedade, this.CaminhoTipo));
            linhas.Add(String.Format("{0}{{", tabInicial));

            if (this.IsPropriedadeInterace)
            {
                linhas.Add(String.Format("{0}{1}return this.{2} as any;", tabInicial, TAB, this.NomePropriedadeRetornar));
            }
            else
            {
                linhas.Add(String.Format("{0}{1}return this.{2};", tabInicial, TAB, this.NomePropriedadeRetornar));
            }


            linhas.Add(String.Format("{0}}}", tabInicial));

            linhas.Add(String.Format("{0}public set {1}(value: {2})  ", tabInicial, this.NomePropriedade, this.CaminhoTipo));
            linhas.Add(String.Format("{0}{{", tabInicial));
            linhas.Add(String.Format("{0}{1}var antigoValor = this.{2};", tabInicial, TAB, this.NomePropriedadeRetornar));
            linhas.Add(String.Format("{0}{1}this.{2} = value;", tabInicial, TAB, this.NomePropriedadeRetornar));
            linhas.Add(String.Format("{0}{1}this.NotificarPropriedadeAlterada(\"{2}\", antigoValor, value);", tabInicial, TAB, this.NomePropriedade));
            linhas.Add(String.Format("{0}}};", tabInicial));

            return linhas;
        }
    }
}
