using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public partial class GeradorDominioReflexao : BaseGeradorDominio
    {
        private string RetornarConteudoPropriedades()
        {
            var sb = new StringBuilder();
            sb.AppendLine(this.RetornarConteudoPropriedadesBaseDominio());
            return sb.ToString();

        }
        private string RetornarConteudoPropriedadesBaseDominio()
        {
            var sb = new StringBuilder();
            var tiposBaseDominio = this.TiposDominio.Where(x => TipoUtil.TipoIgualOuSubTipo(x, AjudanteAssembly.TipoBaseDominio));
            var gruposNamespace = tiposBaseDominio.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                sb.AppendLine(this.RetornarPropriedades(AjudanteAssembly.TipoBaseDominio));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TiposBaseDominio);

                foreach (var tipo in tiposOrdenados)
                {
                    sb.AppendLine(this.RetornarPropriedades(tipo));
                }
            }
            return sb.ToString();
        }

        private string RetornarPropriedades(Type tipo)
        {
            var sb = new StringBuilder();

            //ReflexaoUtil.RetornarPropriedades


            var propriedades = AjudantePropriedades.RetornarPropriedadesReflexao(tipo);

            var declaracaoTipoDeclarado = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo);
            foreach (var propriedade in propriedades)
            {

                var aceitaNulo = ReflexaoUtil.IsTipoNullable(propriedade.PropertyType) || !propriedade.PropertyType.IsValueType;
                var tipoPropriedade = ReflexaoUtil.RetornarTipoSemNullable(propriedade.PropertyType);
                var declaracaoPropriedade = AjudanteReflexao.RetornarDeclaracaoPropriedade(propriedade);
                var declaracaoTipoPropriedade = AjudanteReflexao.RetornarDeclaracaoTipo(tipoPropriedade);

                var declaracao = String.Format("const {0} = new Snebur.Reflexao.Propriedade(\"{1}\", {2}, {3}, {4});",
                                                          declaracaoPropriedade,
                                                          propriedade.Name,
                                                          declaracaoTipoPropriedade,
                                                          declaracaoTipoDeclarado,
                                                          aceitaNulo.ToString().ToLower());

                sb.AppendLine(declaracao);
                sb.AppendLine(this.RetornarConteudoAtributosPropriedade(propriedade));
            }

            if (TipoUtil.TipoIgualOuSubTipo(tipo, AjudanteAssembly.TipoEntidade))
            {
                sb.AppendLine(this.RetornarAssociacaoPropriedadeChavePrimara(tipo));
            }

            sb.AppendLine("");

            //Adicionaros Atributos

            foreach (var propriedade in propriedades)
            {
                var declaracaoPropriedade = AjudanteReflexao.RetornarDeclaracaoPropriedade(propriedade);
                sb.AppendLine(String.Format("{0}.Propriedades.Add({1});", declaracaoTipoDeclarado, declaracaoPropriedade));
            }

            return sb.ToString();

        }

        private string RetornarAssociacaoPropriedadeChavePrimara(Type tipoEntidade)
        {
            var sb = new StringBuilder();
            var propriedadeChavePrimaria = this.RetornarPropriedadeChavePrimaria(tipoEntidade);
            var declaracaoTipoDeclarado = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipoEntidade);
            var declaracaoPropriedadeChavePrimaria = AjudanteReflexao.RetornarDeclaracaoPropriedade(propriedadeChavePrimaria);
            sb.AppendLine(String.Format("{0}.PropriedadeChavePrimaria = {1};", declaracaoTipoDeclarado, declaracaoPropriedadeChavePrimaria));
            return sb.ToString();
        }

        private PropertyInfo RetornarPropriedadeChavePrimaria(Type tipoEntidade)
        {
            var tipoKeyAttribute = typeof(KeyAttribute);

            if (TipoUtil.TipoIgual(tipoEntidade, AjudanteAssembly.TipoEntidade))
            {
                return AjudanteAssembly.PropriedadeChavePrimariaEntidade;
            }

            var propriedadesChavePrimaria = ReflexaoUtil.RetornarPropriedades(tipoEntidade, false).Where(x => ReflexaoUtil.PropriedadePossuiAtributo(x, tipoKeyAttribute)).ToList();
            if (propriedadesChavePrimaria.Count == 0)
            {
                throw new Erro(String.Format("Não foi encontrada a propriedade chave primária em {0}", tipoEntidade.Name));
            }

            if (propriedadesChavePrimaria.Count > 1)
            {
                throw new Erro(String.Format("Existe mais de propriedade chave primária em {0}", tipoEntidade.Name));
            }

            var propriedadeChavePrimaria = propriedadesChavePrimaria.Single();
            if (propriedadeChavePrimaria.Name == AjudanteAssembly.PropriedadeChavePrimariaEntidade.Name)
            {
                return AjudanteAssembly.PropriedadeChavePrimariaEntidade;
            }
            return propriedadeChavePrimaria;
        }

    }
}
