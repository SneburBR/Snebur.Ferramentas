using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public class EstruturaPropriedadeEspecializada : EstruturaPropriedade
    {

        public string NomePropriedaEspecializada { get; set; }
        public PropriedadeTSEspecializadaAttribute Atributo { get; set; }

        //public string NomePropriedadeAtribuir { get; set; }

        public EstruturaPropriedadeEspecializada(PropertyInfo propriedade) :
            base(propriedade)
        {

            var atributoPropriedadeEspecializada = propriedade.GetCustomAttributes().Where(k => k.GetType().Name == nameof(PropriedadeTSEspecializadaAttribute)).SingleOrDefault();
            this.NomePropriedaEspecializada = Convert.ToString(Snebur.Utilidade.ReflexaoUtil.RetornarValorPropriedade(atributoPropriedadeEspecializada, "NomePropriedade"));
        }


        public string RetornarLinhaValorVariavelTypeScript(string tabInicial)
        {
            return String.Empty;
        }

        public override List<string> RetornarLinhasTypeScript(string tabInicial)
        {
            var linhas = new List<string>();
            //var modificadorLeitura = this.Propriedade.GetMethod.IsPublic?  "public" : "protected";
            //var modificadorGravacao = this.Propriedade.SetMethod.IsPublic ? "public" : "protected";

            var modificadorLeitura = "public";
            var modificadorGravacao = "public";

            if(this.Propriedade.GetMethod!= null)
            {
                linhas.Add($"{tabInicial}{modificadorLeitura} get {this.NomePropriedade}() : {this.CaminhoTipo} ");
                linhas.Add(String.Format("{0}{{", tabInicial));
                linhas.Add(String.Format("{0}{1}return this.{2} as any;", tabInicial, TAB, this.NomePropriedaEspecializada));
                linhas.Add(String.Format("{0}}}", tabInicial));

            }
            if (this.Propriedade.SetMethod != null)
            {
                linhas.Add($"{tabInicial}{modificadorGravacao} set { this.NomePropriedade}(value: {this.CaminhoTipo})  ");
                linhas.Add(String.Format("{0}{{", tabInicial));
                var metodoNotificarPropriedadeAlterada = "NotificarPropriedadeAlterada";
                linhas.Add($"{tabInicial}{TAB}this.{metodoNotificarPropriedadeAlterada}(\"{this.NomePropriedade}\", this.{this.NomePropriedaEspecializada}, this.{this.NomePropriedaEspecializada} = value as any);");
                linhas.Add(String.Format("{0}}};", tabInicial));
            }
               

            return linhas;
        }
    }
}
