using System;
using System.Collections.Generic;
using System.Linq;
using Snebur.Dominio;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public class EstruturaConstrutor
    {
        public readonly string TAB = "    ";
        public Type Tipo { get; }
        public List<EstruturaPropriedadePublica> Propriedades { get; }
        public bool IsIImagem { get; }
        //public bool IsPropriedadeInicializadora { get; } = true;


        public EstruturaConstrutor(Type tipo, List<EstruturaPropriedadePublica> propriedades)
        {
            this.Tipo = tipo;
            this.Propriedades = propriedades;
            this.IsIImagem = TipoUtil.TipoImplementaInterface(this.Tipo, AjudanteAssembly.TipoInterfaceIImagem);
        }

        public List<string> RetornarLinhasTypeScript(string tabInicial)
        {
            var parametrosConstrutor = AjudanteReflexao.RetornarParametrosConstrutor(this.Tipo);
            var (parametrosConstrutorTS, parametrosSuperTS) = this.RetornarParametrosConstrutor(parametrosConstrutor);

            var linhas = new List<string>();


            linhas.Add($"{tabInicial}public constructor({parametrosConstrutorTS}) ");
            linhas.Add($"{tabInicial}{{");
            linhas.Add($"{tabInicial}{TAB}super({parametrosSuperTS});");

            if (!this.Tipo.IsAbstract && this.Tipo.BaseType != null && this.Tipo.BaseType.IsAbstract)
            {
                linhas.Add(String.Format("{0}{1}this.Inicializar();", tabInicial, TAB));
            }
            if (!this.Tipo.IsAbstract && TipoUtil.TipoImplementaInterface(this.Tipo, AjudanteAssembly.TipoInterfaceIImagem))
            {
                linhas.Add(String.Format("{0}{1}this.InicializarImagem(arquivo, informacaoImagem);", tabInicial, TAB));
            }

            foreach (var parametroConstrutor in parametrosConstrutor)
            {
                var propriedade = this.Propriedades.Where(x => x.NomePropriedade.Equals(parametroConstrutor.NomeParametro, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                if (parametroConstrutor.IsInicializarPropriedade)
                {
                    if (propriedade == null)
                    {
                        throw new Exception(String.Format("Não foi encontrada um propriedade para o parametro {0} na classe {1} ", parametroConstrutor.NomeParametro, this.Tipo.Name));
                    }
                    linhas.Add(String.Format("{0}{1}this.{2} = {3};", tabInicial, TAB, propriedade.NomePriprieadePrivada, parametroConstrutor.NomeParametro));
                }
            }

            linhas.Add(String.Format("{0}}}", tabInicial));
            linhas.Add("");
            return linhas;

        }

        private (string parametrosConstrutorTS, string parametrosSuperTS) RetornarParametrosConstrutor(List<EstruturaParametroConstrutor> parametrosConstrutor)
        {

            if (parametrosConstrutor.Count > 0)
            {
                var parametrosConstrutorTS = String.Join(", ", parametrosConstrutor.Select(x => x.RetornarTypeScript()));
                return (parametrosConstrutorTS, String.Empty);
            }

            if (!this.Tipo.IsAbstract && this.Tipo.BaseType != null && !this.Tipo.BaseType.IsAbstract)
            {
                return (String.Empty, String.Empty);
            }
            //if(TipoUtil.TipoSubTipo(this.Tipo, typeof(Entidade)))
            //{
            return ($"inicializador?: Partial<{this.Tipo.Name}>", "inicializador");
            //}
            //return (String.Empty, String.Empty);

        }

        //public string RetornarParametrosTypeScript()
        //{
        //    return this.RetornarParametrosTypeScript(this.Tipo);
        //}





    }
}
