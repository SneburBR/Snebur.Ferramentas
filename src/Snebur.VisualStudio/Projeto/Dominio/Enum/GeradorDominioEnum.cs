using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Reflection;
using Snebur.VisualStudio.Utilidade;
using Snebur.VisualStudio.Reflexao;
using Snebur.VisualStudio.Projeto.Dominio.Estrutura;
using Snebur.Utilidade;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public class GeradorDominioEnum : BaseGeradorDominio
    {
        private List<Type> TiposEnum { get; set; }

        public GeradorDominioEnum(ConfiguracaoProjetoDominio configuracaoDominio,
                                  List<Type> todosTipos,
                                  string nomeArquivo) : base(configuracaoDominio, todosTipos, nomeArquivo)
        {
            this.TiposEnum = this.TiposDominio;
        }

        protected override List<Type> RetornarRetornarTiposDominios()
        {
            return this.RetornarTiposEnum();
        }

        protected override string RetornarConteudoTypeScript()
        {
            var sb = new System.Text.StringBuilder();
            var gruposNamespace = this.TiposEnum.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;

                sb.AppendLine("");
                sb.AppendLine(String.Format("namespace {0}", _namespace));
                sb.AppendLine("{");

                foreach (var tipoEnum in grupoNamespace)
                {
                    sb.AppendLine("");
                    sb.AppendLine(String.Format("{0}export enum {1}", TAB, tipoEnum.Name));
                    sb.AppendLine(String.Format("{0}{{", TAB));
                    sb.AppendLine("");

                    var valoresEnum = EnumUtil.RetornarValoresEnum(tipoEnum);

                    if (valoresEnum.Count() != valoresEnum.Distinct().Count())
                    {
                        throw new Exception(String.Format("Existe valores duplicado no Enum {0}", tipoEnum.Name));
                    }

                    foreach (var valorEnum in valoresEnum)
                    {
                        var descricaoEnum = valorEnum.ToString();
                        var intValorEnum = Convert.ToInt32(valorEnum);
                        sb.AppendLine(String.Format("{0}{0}{1} = {2},", TAB, descricaoEnum, intValorEnum));
                    }
                    sb.AppendLine(String.Format("{0}}}", TAB));

                    //var rotulos = String.Join(",", valoresEnum.Select(x => String.Format("\"{0}\"", EnumUtil.RetornarDescricao(x)));
                    sb.AppendLine();
                    sb.AppendLine(String.Format("{0}({1} as any).Rotulos = {{}};",TAB, tipoEnum.Name));

                    foreach (var valorEnum in valoresEnum)
                    {
                        sb.AppendLine(String.Format("{0}({1} as any).Rotulos[\"{2}\"] = \"{3}\";",
                                      TAB,
                                      tipoEnum.Name,
                                      valorEnum.ToString(),
                                      EnumUtil.RetornarDescricao(valorEnum)));
                    }
                    sb.AppendLine();

                }

                sb.AppendLine("");
                sb.AppendLine("}");
                sb.AppendLine("");


            }
            return sb.ToString();

        }

        protected override string RetornarNomeArquivoDestino()
        {
            return "Enums";
        }

        private List<Type> RetornarTiposEnum()
        {
            var tiposEnum = this.TodosTipo.Where(x => x.IsEnum).ToList();
            tiposEnum = TipoUtil.IgnorarAtributo(tiposEnum, AjudanteAssembly.NomeTipoIgnorarEnumTS);
            return tiposEnum;
        }


    }
}
