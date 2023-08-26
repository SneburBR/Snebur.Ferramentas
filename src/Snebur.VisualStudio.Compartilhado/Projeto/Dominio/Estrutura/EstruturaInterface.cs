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
using Snebur.Dominio.Atributos;

namespace Snebur.VisualStudio
{
    public class EstruturaInterface : BaseEstrutura
    {
        public string Extensao { get; set; }
        public bool IsAsync { get; }

        public EstruturaInterface(Type tipo) : base(tipo)
        {
            this.Extensao = this.RetornarExtensao();
            this.IsAsync = TipoUtil.TipoImplementaInterface(tipo, AjudanteAssembly.TipoInterfaceIBaseServico);
        }

        public List<string> RetornarLinhasTypeScriptInterface()
        {
            var linhas = new List<string>();
            linhas.Add(String.Format("export interface {0}{1}", this.NomeTipo, this.Extensao));
            linhas.Add("{");

            var propriedades = ReflexaoUtil.RetornarPropriedades(this.Tipo, true);
            propriedades = propriedades.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarPropriedadeTS)).ToList();
             
            foreach (var propriedade in propriedades)
            {
                var modificador = propriedade.SetMethod == null &&  !PropriedadeUtil.PossuiAtributo(propriedade, typeof(PropriedadeGravacaoTSAttribute)) ? "readonly " : String.Empty;
                var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(propriedade.PropertyType);
                var opcional = this.IsOpcional(propriedade) ? "?": String.Empty;
                var declaracao = $"{TAB}{modificador}{propriedade.Name}{opcional} : {caminhoTipo};";
                linhas.Add(declaracao);
            }

            var async = this.IsAsync ? "Async" : "";
            var metodos = ReflexaoUtil.RetornarMetodos(this.Tipo, true);
            metodos = metodos.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarMetodoTS)).ToList();
            foreach (var metodo in metodos)
            {
                var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(metodo.ReturnType, this.IsAsync);
                var parametros = this.RetornarParametros(metodo);
                var opcional = PropriedadeUtil.PossuiAtributo(metodo, typeof(MetodoOpcionalTSAttribute)) ? "?" : String.Empty;
                linhas.Add($"{TAB}{metodo.Name}{async}{opcional}({parametros}) : {caminhoTipo};");
            }

            linhas.Add("}");
            return linhas;
        }

        private bool IsOpcional(PropertyInfo propriedade)
        {
            return PropriedadeUtil.PossuiAtributo(propriedade, typeof(PropriedadeOpcionalTSAttribute)) ||
                 ReflexaoUtil.IsTipoNullable(propriedade.PropertyType);
            
        }

        #region Métodos privados

        private string RetornarExtensao()
        {
            var interfaces = this.Tipo.GetInterfaces().Where(x => !x.Namespace.StartsWith("System")).ToList();
            interfaces = interfaces.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarInterfaceTS)).ToList();
            if (interfaces.Count() > 0)
            {
                var aaa = String.Join(",", interfaces.Select(x => TipoUtil.RetornarCaminhoTipoTS(x)));
                return string.Format(" extends {0} ", aaa);
            }
            return String.Empty;
        }

        private string RetornarParametros(MethodInfo metodo)
        {
            var parametros = metodo.GetParameters();
            return String.Join(",", parametros.Select(x => String.Format("{0} : {1}", x.Name, this.RetornarCaminhoTipoParametro(x))));
        }

       

        #endregion
    }
}
