using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.VisualStudio.Reflexao;
using System.Reflection;

namespace Snebur.VisualStudio.Projeto.Dominio.Estrutura
{
    public class EstruturaInterface : BaseEstrutura
    {
        public string Extensao { get; set; }

        public EstruturaInterface(Type tipo) : base(tipo)
        {
            this.Extensao = this.RetornarExtensao();
        }

        public List<string> RetornarLinhasTypeScriptInterface()
        {
            var linhas = new List<string>();
            linhas.Add(String.Format("export interface {0}{1}",  this.NomeTipo, this.Extensao));
            linhas.Add("{");

            var propriedades = ReflexaoUtil.RetornarPropriedades(this.Tipo, true);
            propriedades = propriedades.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarPropriedadeTS)).ToList();

            foreach (var propriedade in propriedades)
            {
                var caminhoTipo = TipoUtil.RetornarCaminhoTipo(propriedade.PropertyType);
                linhas.Add(String.Format("{0}{1} : {2};", TAB, propriedade.Name, caminhoTipo));
            }

            var metodos = ReflexaoUtil.RetornarMetodos(this.Tipo, true);
            metodos = metodos.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarMetodoTS)).ToList();
            foreach (var metodo in metodos)
            {
                var caminhoTipo = TipoUtil.RetornarCaminhoTipo(metodo.ReturnType);
                var parametros = this.RetornarParametros(metodo);
                linhas.Add(String.Format("{0}{1}({2}) : {3};", TAB, metodo.Name, parametros, caminhoTipo));
            }

            linhas.Add("}");
            return linhas;
        }

        #region Métodos privados

        private string RetornarExtensao()
        {
            var interfaces = this.Tipo.GetInterfaces().Where(x => !x.Namespace.StartsWith("System")).ToList();
            interfaces = interfaces.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarInterfaceTS)).ToList();
            if (interfaces.Count()> 0)
            {
                var aaa = String.Join(",", interfaces.Select(x => TipoUtil.RetornarCaminhoTipo(x)));
                return string.Format(" extends {0} ", aaa);
            }
            return String.Empty;
        }

        private string RetornarParametros(MethodInfo metodo)
        {
            var parametros = metodo.GetParameters();
            return String.Join(",", parametros.Select(x => String.Format("{0} : {1}", x.Name, TipoUtil.RetornarCaminhoTipo(x.ParameterType))));
        }

        #endregion
    }
}
