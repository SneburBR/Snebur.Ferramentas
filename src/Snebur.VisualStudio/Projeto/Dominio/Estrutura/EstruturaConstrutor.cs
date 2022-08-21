using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Reflexao;
using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio.Projeto.Dominio.Estrutura
{
    public class EstruturaConstrutor
    {
        public readonly string TAB = "    ";
        public Type Tipo { get; set; }
        public List<EstruturaPropriedadePublica> Propriedades { get; set; }

        public EstruturaConstrutor(Type tipo, List<EstruturaPropriedadePublica> propriedades)
        {
            this.Tipo = tipo;
            this.Propriedades = propriedades;
        }

        public List<string> RetornarLinhasTypeScript(string tabInicial)
        {

            var parametrosConstrutor = AjudanteReflexao.RetornarParametrosConstrutor(this.Tipo);

            var parametrosTypeScript = String.Join(", ", parametrosConstrutor.Select(x => x.RetornarTypeScript()));


            var linhas = new List<string>();
            linhas.Add(String.Format("{0}public constructor({1}) ", tabInicial, parametrosTypeScript));
            linhas.Add(string.Format("{0}{{", tabInicial));
            //linhas.Add(string.Format("{0}{1}super({1});", tabInicial, TAB, this.RetornarParametrosSuperTypeScript()));
            linhas.Add(string.Format("{0}{1}super();", tabInicial, TAB));

            foreach (var parametroConstrutor in parametrosConstrutor)
            {
                var propriedade = this.Propriedades.Where(x => x.NomePropriedade.Equals(parametroConstrutor.NomeParametro, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                if (propriedade == null)
                {
                    throw new Exception(String.Format("Não foi encontrada um propriedade para o parametro {0} na classe {1} ", parametroConstrutor.NomeParametro, this.Tipo.Name));
                }
                linhas.Add(string.Format("{0}{1}this.{2} = {3};", tabInicial, TAB, propriedade.NomePriprieadePrivada, parametroConstrutor.NomeParametro));

            }

           
            linhas.Add(string.Format("{0}}}", tabInicial));
            linhas.Add("");
            return linhas;

        }

        //public string RetornarParametrosTypeScript()
        //{
        //    return this.RetornarParametrosTypeScript(this.Tipo);
        //}



        

    }
}
