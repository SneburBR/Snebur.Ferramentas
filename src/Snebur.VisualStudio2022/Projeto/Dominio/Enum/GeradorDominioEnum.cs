using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Snebur.VisualStudio.Reflexao;
using Snebur.Utilidade;
using Snebur.Dominio.Atributos;

namespace Snebur.VisualStudio
{
    public class GeradorDominioEnum : BaseGeradorDominio
    {
        private HashSet<Type> TiposEnum { get; set; }

        public GeradorDominioEnum(ConfiguracaoProjetoDominio configuracaoDominio,
                                  string caminhoProjeto,
                                  List<Type> todosTipos,
                                  string nomeArquivo) :
                                  base(configuracaoDominio, caminhoProjeto, todosTipos, nomeArquivo)
        {
            this.TiposEnum = this.TiposDominio;
        }

        protected override HashSet<Type> RetornarRetornarTiposDominios()
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
                    var ignorarEnum = TipoUtil.TipoPossuiAtributo(tipoEnum, nameof(IgnorarValidacaoEnumValorUnicoAttribute));
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
                    sb.AppendLine();
                    sb.AppendLine(String.Format("{0}({1} as any).Rotulos = {{}};", TAB, tipoEnum.Name));
                   
                    foreach (var valorEnum in valoresEnum)
                    {
                        var membroInfo = tipoEnum.GetField(valorEnum.ToString());

                        var rotulo = this.RetornarRotulo(membroInfo);
                        sb.AppendLine(String.Format("{0}({1} as any).Rotulos[\"{2}\"] = \"{3}\";",
                                      TAB,
                                      tipoEnum.Name,
                                      valorEnum.ToString(),
                                      rotulo));
 
                    }

                    var isDeclaradoObjetoVSIntelliSense = false;
                    foreach (var valorEnum in valoresEnum)
                    {
                        var membroInfo = tipoEnum.GetField(valorEnum.ToString());

                        var atriutosRotuloVS = PropriedadeUtil.RetornarAtributos(membroInfo, AjudanteAssembly.TipoAtributoRotuloVSIntelliSense, true);
                        if (atriutosRotuloVS.Count > 0)
                        {
                            if (!isDeclaradoObjetoVSIntelliSense)
                            {
                                isDeclaradoObjetoVSIntelliSense = true;
                                sb.AppendLine();
                                sb.AppendLine($"{TAB}({tipoEnum.Name} as any).RotulosVSIntelliSense = {{}};");
                                sb.AppendLine();
                            }


                            foreach (var atributoRotuloVS in atriutosRotuloVS)
                            {
                                var intValor = System.Convert.ToInt32(valorEnum);
                                var descricao = ReflexaoUtil.RetornarValorPropriedade<RotuloVSIntelliSenseAttribute>(x => x.Rotulo, atributoRotuloVS).ToString();
                                sb.AppendLine($"{TAB}({tipoEnum.Name} as any).RotulosVSIntelliSense[\"{descricao}\"] = \"{intValor}\";");
                            }
                        }
                    }
                    sb.AppendLine();

                }

                sb.AppendLine("");
                sb.AppendLine("}");
                sb.AppendLine("");


            }
            return sb.ToString();

        }

        private string RetornarRotulo(MemberInfo membroInfo)
        {
            var nomeAtributo = "RotuloAttribute";
            var rotuloAtributo = membroInfo.GetCustomAttributes(true).Where(x => x.GetType().Name == nomeAtributo).SingleOrDefault();
            if (rotuloAtributo != null)
            {
                return ReflexaoUtil.RetornarValorPropriedade(rotuloAtributo, "Rotulo").ToString();
            }
            return membroInfo.Name;
        }

        protected override string RetornarNomeArquivoDestino()
        {
            return "Enums";
        }

        private HashSet<Type> RetornarTiposEnum()
        {
            var tiposEnum = this.TodosTipo.Where(x => x.IsEnum).ToList();
            tiposEnum = TipoUtil.IgnorarAtributo(tiposEnum, AjudanteAssembly.NomeTipoIgnorarEnumTS);
            return tiposEnum.ToHashSet();
        }


    }
}
