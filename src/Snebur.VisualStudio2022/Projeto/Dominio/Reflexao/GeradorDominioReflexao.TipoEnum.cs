using System;
using System.Linq;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public partial class GeradorDominioReflexao : BaseGeradorDominio
    {
        private string RetornarConteudoTiposEnum()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("//Enum");
            sb.AppendLine(this.RetornarDeclaracoesTipoEnum());
            sb.AppendLine("//Lista Enum");
            sb.AppendLine(this.RetornarDeclaracoesListaTipoEnum());
            sb.AppendLine("//Adicionar Enum");
            sb.AppendLine(this.RetornarConteudoAdicionarTiposEnum());
            sb.AppendLine(this.RetornarConteudoAdicionarListaTipoEnum());
            sb.AppendLine("//Associar caminhos Emil");
            sb.AppendLine(this.RetornarAssociarCaminhosTiposEnum());
            return sb.ToString();
        }

        private string RetornarDeclaracoesTipoEnum()
        {
            var sb = new System.Text.StringBuilder();
            
            

            var gruposNamespace = this.TiposEnum.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();
            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                foreach (var tipoEnum in grupoNamespace)
                {
                    sb.AppendLine(this.RetornarDeclaracaoTipoEnum(tipoEnum, _namespace));
                }
            }
            return sb.ToString();
        }

        private string RetornarDeclaracoesListaTipoEnum()
        {
            var sb = new System.Text.StringBuilder();
            
            var gruposNamespace = this.TiposEnum.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();
            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                foreach (var tipoEnum in grupoNamespace)
                {
                    sb.AppendLine(this.RetornarDeclaracaoListaTipoEnum(tipoEnum, _namespace));
                }
            }


            return sb.ToString();
        }

        private string RetornarDeclaracaoTipoEnum(Type tipo, string _namespace)
        {
            var declaracaoEnum = AjudanteReflexao.RetornarDeclaracaoTipoEnum(tipo);
            //var caminhoTipoEnum = AjudanteReflexao.RetornarCaminhoTipoEnum(tipo);

            var assemblyQualifiedName = AjudanteReflexao.RetornarAssemblyQualifiedNameTipoEnum(tipo);
            var retorno = String.Format("const {0} = new Snebur.Reflexao.TipoEnum(\"{1}\", \"{2}\", \"{3}\");",
                                                       declaracaoEnum,
                                                       tipo.Name,
                                                       _namespace,
                                                       assemblyQualifiedName);

            return retorno;
        }

        private string RetornarDeclaracaoListaTipoEnum(Type tipo, string _namespace)
        {
            var declaracaoListaEnum = AjudanteReflexao.RetornarDeclaracaoListaTipoEnum(tipo);
            //var caminhoListaTipoEnum = AjudanteReflexao.RetornarCaminhoListaTipoEnum(tipo);
            var nomeTipoListaEnum = AjudanteReflexao.RetornarNomeListaTipoEnum(tipo);
            var assemblyQualifiedName = AjudanteReflexao.RetornarAssemblyQualifiedNameListaTipoEnum(tipo);

            var declaracaoEnum = AjudanteReflexao.RetornarDeclaracaoTipoEnum(tipo);

            var retorno = String.Format("const {0} = new Snebur.Reflexao.TipoListaEnum(\"{1}\", \"{2}\", \"{3}\", {4});",
                                                       declaracaoListaEnum,
                                                       nomeTipoListaEnum,
                                                       _namespace,
                                                       assemblyQualifiedName,
                                                       declaracaoEnum);

            return retorno;
        }

        private string RetornarConteudoAdicionarTiposEnum()
        {
            var sb = new System.Text.StringBuilder();


            var gruposNamespace = this.TiposEnum.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;

                foreach (var tipoEnum in grupoNamespace)
                {
                    var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(tipoEnum);
                    var declaracao = AjudanteReflexao.RetornarDeclaracaoTipoEnum(tipoEnum);
                    sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
                }
            }
            return sb.ToString();
        }

        private string RetornarConteudoAdicionarListaTipoEnum()
        {
            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposEnum.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;

                foreach (var tipoEnum in grupoNamespace)
                {
                    var caminhoTipo = AjudanteReflexao.RetornarCaminhoListaTipoEnum(tipoEnum);
                    var declaracao = AjudanteReflexao.RetornarDeclaracaoListaTipoEnum(tipoEnum);
                    sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
                }
            }
            return sb.ToString();
        }

        private string RetornarAssociarCaminhosTiposEnum()
        {
            var sb = new System.Text.StringBuilder();
            var nomePropriedadeCaminhoTipo = ReflexaoUtil.RetornarNomePropriedade<Snebur.Dominio.ICaminhoTipo>(x => x.__CaminhoTipo);
            var gruposNamespace = this.TiposEnum.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                foreach (var tipoEnum in grupoNamespace)
                {
                    var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(tipoEnum);
                    sb.AppendLine(String.Format("{0}.{1} = \"{0}\";", caminhoTipo, nomePropriedadeCaminhoTipo));
                }
            }
            return sb.ToString();
        }
    }
}
