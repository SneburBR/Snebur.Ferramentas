using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Utilidade;
using System.Reflection;
using Snebur.VisualStudio.Reflexao;
using Snebur.Reflexao;
using Snebur.Utilidade;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public partial class GeradorDominioReflexao
    {
        public string RetornarConteudoTipoComplexo()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("//TiposComplexos");
            sb.AppendLine(this.RetornarDeclaracoesTipoComplexo());
            sb.AppendLine("//ListaBaseDominio TipoComplexo");
            sb.AppendLine(this.RetornarDeclaracoesListaBaseDominioTipoComplexo());

            sb.AppendLine("//Adicionar TiposComplexos");
            sb.AppendLine(this.RetornarAdicionarTiposComplexo());
            
            sb.AppendLine("//Adicionar ListaBaseDominio TiposComplexos");
            sb.AppendLine(this.RetornarAdicionarTiposListaBaseDominioTipoComplexo());

            sb.AppendLine("//Associar caminhos TiposComplexos");
            sb.AppendLine(this.RetornarAssociarCaminhosTipoComplexo());
            sb.AppendLine("//Atributos TiposComplexos");
            sb.AppendLine(this.RetornarAtributosTipoComplexo());
            return sb.ToString();
        }

        private string RetornarDeclaracoesTipoComplexo()
        {
            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposComplexo.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                sb.AppendLine(this.RetornarDeclaracaoTipoComplexo(AjudanteAssembly.TipoBaseTipoComplexo, "Snebur.Dominio"));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoBaseTipoComplexo);

                foreach (var tipo in tiposOrdenados)
                {
                    sb.AppendLine(this.RetornarDeclaracaoTipoComplexo(tipo, _namespace));
                }
            }
            return sb.ToString();
        }

        private string RetornarDeclaracaoTipoComplexo(Type tipo, string _namespace)
        {

            var declaracaoTipoComplexo = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo);
            var construtorBaseDominio = String.Format("{0}.{1}", tipo.Namespace, tipo.Name);
            //var caminhoTipoBaseDominio = AjudanteReflexao.RetornarCaminhoTipoBaseDominio(tipo);
            var declaracaoTipoBaseDominioBase = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo.BaseType);
            var assemblyQualifiedName = AjudanteReflexao.RetornarAssemblyQualifiedNameTipoBaseDominio(tipo);
            var abstrato = tipo.IsAbstract;
            var retorno = String.Format("var {0} = new Snebur.Reflexao.TipoComplexo({1}, \"{2}\", \"{3}\",\"{4}\",{5}, {6});",
                                                 declaracaoTipoComplexo,
                                                 construtorBaseDominio,
                                                 tipo.Name,
                                                 _namespace,
                                                 assemblyQualifiedName,
                                                 declaracaoTipoBaseDominioBase,
                                                 abstrato.ToString().ToLower());
            return retorno;
        }

        private string RetornarDeclaracoesListaBaseDominioTipoComplexo()
        {
            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposComplexo.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                sb.AppendLine(this.RetornarDeclaracaoListaTipoBaseDominio(AjudanteAssembly.TipoBaseTipoComplexo, "Snebur.Dominio"));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoBaseTipoComplexo);

                foreach (var tipo in tiposOrdenados)
                {
                    if (TipoUtil.TipoIgualOuSubTipo(tipo, AjudanteAssembly.TipoBaseTipoComplexo))
                    {
                        sb.AppendLine(this.RetornarDeclaracaoListaTipoBaseDominio(tipo, _namespace));
                    }
                }
            }
            return sb.ToString();

        }

        private string RetornarAdicionarTiposComplexo()
        {

            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposComplexo.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                var caminhoTipo = TipoUtil.RetornarCaminhoTipo(AjudanteAssembly.TipoBaseTipoComplexo);
                var declaracao = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(AjudanteAssembly.TipoBaseTipoComplexo);

                sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoBaseTipoComplexo);

                foreach (var tipo in tiposOrdenados)
                {
                    var caminhoTipo = TipoUtil.RetornarCaminhoTipo(tipo);
                    var declaracao = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo);

                    sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
                }
            }
            return sb.ToString();
        }

        private string RetornarAdicionarTiposListaBaseDominioTipoComplexo()
        {
            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposComplexo.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                var caminhoTipo = AjudanteReflexao.RetornarCaminhoListaTipoBaseDominio(AjudanteAssembly.TipoBaseTipoComplexo);
                var declaracao = AjudanteReflexao.RetornarDeclaracaoListaTipoBaseDominio(AjudanteAssembly.TipoBaseTipoComplexo);

                sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoBaseTipoComplexo);

                foreach (var tipo in tiposOrdenados)
                {
                    if (TipoUtil.TipoIgualOuSubTipo(tipo, AjudanteAssembly.TipoBaseTipoComplexo))
                    {
                        var caminhoTipo = AjudanteReflexao.RetornarCaminhoListaTipoBaseDominio(tipo);
                        var declaracao = AjudanteReflexao.RetornarDeclaracaoListaTipoBaseDominio(tipo);

                        sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
                    }
                }
            }
            return sb.ToString();

        }
        
        private string RetornarAssociarCaminhosTipoComplexo()
        {

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");

            
            var gruposNamespace = this.TiposComplexo.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();
            var nomePropriedadeCaminhoTipo = ReflexaoUtil.RetornarNomePropriedade<Snebur.Dominio.ICaminhoTipo>(x => x.__CaminhoTipo);

            if (this.ProjetoSneburDominio)
            {
                var caminhoTipo = TipoUtil.RetornarCaminhoTipo(AjudanteAssembly.TipoBaseTipoComplexo);
                sb.AppendLine(String.Format("{0}.{1} = \"{0}\";", caminhoTipo, nomePropriedadeCaminhoTipo));
            }
            foreach (var grupoNamespace in gruposNamespace)
            {
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoBaseTipoComplexo);
                foreach (var tipo in tiposOrdenados)
                {
                    var caminhoTipo = TipoUtil.RetornarCaminhoTipo(tipo);
                    sb.AppendLine(String.Format("{0}.{1} = \"{0}\";", caminhoTipo, nomePropriedadeCaminhoTipo));
                }
            }
            return sb.ToString();
        }

        private string RetornarAtributosTipoComplexo()
        {
            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposComplexo.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                sb.AppendLine(this.RetornarConteudoAtributosTipo(AjudanteAssembly.TipoBaseTipoComplexo));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoBaseTipoComplexo);

                foreach (var tipo in tiposOrdenados)
                {
                    sb.AppendLine(this.RetornarConteudoAtributosTipo(tipo));
                }
            }
            return sb.ToString();

        }
    }
}
 