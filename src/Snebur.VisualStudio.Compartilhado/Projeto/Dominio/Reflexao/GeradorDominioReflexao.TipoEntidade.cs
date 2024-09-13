using System;
using System.Linq;
using Snebur.Dominio;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public partial class GeradorDominioReflexao
    {
        public string RetornarConteudoEntidade()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("//BaseEntidades");
            sb.AppendLine(this.RetornarDeclaracoesTipoEntidade());
            sb.AppendLine("//ListaBaseDominio TipoEntidade");
            sb.AppendLine(this.RetornarDeclaracoesListaBaseDominioTipoEntidade());
            sb.AppendLine("//ListaBaseEntidades");
            sb.AppendLine(this.RetornarDeclaracoesListaEntidadesTipoEntidade());
            sb.AppendLine("//Adicionar BaseEntidades");
            sb.AppendLine(this.RetornarAdicionarTiposBaseEntidade());
            sb.AppendLine("//Adicionar ListaBaseDominio TipoEntidade");
            sb.AppendLine(this.RetornarAdicionarTiposListaBaseDominioTipoEntidade());
            sb.AppendLine("//Adicionar ListaBaseEntidades");
            sb.AppendLine(this.RetornarAdicionarTiposListaBaseEntidade());
            sb.AppendLine("//Associar caminhos BaseEntidades");
            sb.AppendLine(this.RetornarAssociarCaminhosTipoEntidade());
            sb.AppendLine("//Atributos TipoEntidade");
            sb.AppendLine(this.RetornarAtributosTiposEntidade());
            return sb.ToString();
        }

        private string RetornarDeclaracoesTipoEntidade()
        {
            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposEntidade.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                sb.AppendLine(this.RetornarDeclaracaoTipoEntidade(AjudanteAssembly.TipoEntidade, "Snebur.Dominio"));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoEntidade);

                foreach (var tipo in tiposOrdenados)
                {
                    sb.AppendLine(this.RetornarDeclaracaoTipoEntidade(tipo, _namespace));
                }
            }
            return sb.ToString();
        }

        private string RetornarDeclaracaoTipoEntidade(Type tipo, string _namespace)
        {

            var declaracaoTipoEntidade = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo);
            var construtorBaseDominio = String.Format("{0}.{1}", tipo.Namespace, tipo.Name);
            //var caminhoTipoBaseDominio = AjudanteReflexao.RetornarCaminhoTipoBaseDominio(tipo);
            var declaracaoTipoBaseDominioBase = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo.BaseType);
            var assemblyQualifiedName = AjudanteReflexao.RetornarAssemblyQualifiedNameTipoBaseDominio(tipo);
            var isAbstrato = tipo.IsAbstract;
            var isImplementaIDeletado = TipoUtil.TipoImplementaInterface(tipo, typeof(IDeletado));
            var isImplementaIAtivo = TipoUtil.TipoImplementaInterface(tipo, typeof(IAtivo));
            var isIdentity = TipoUtil.IsTipoEntitidadeIdentity(tipo);
            var retorno = $"const {declaracaoTipoEntidade} = new Snebur.Reflexao.TipoEntidade({construtorBaseDominio}, \"{tipo.Name}\", \"{_namespace}\",\"{assemblyQualifiedName}\",{declaracaoTipoBaseDominioBase}, {isAbstrato.ToString().ToLower()}, {isImplementaIDeletado.ToString().ToLower()}, {isImplementaIAtivo.ToString().ToLower()},{isIdentity.ToString().ToLower()});";

            return retorno;
        }

        private string RetornarDeclaracoesListaBaseDominioTipoEntidade()
        {
            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposEntidade.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                sb.AppendLine(this.RetornarDeclaracaoListaTipoBaseDominio(AjudanteAssembly.TipoEntidade, "Snebur.Dominio"));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), 
                                                                 AjudanteAssembly.TipoEntidade);

                foreach (var tipo in tiposOrdenados)
                {
                    if (TipoUtil.TipoIgualOuSubTipo(tipo, AjudanteAssembly.TipoEntidade))
                    {
                        sb.AppendLine(this.RetornarDeclaracaoListaTipoBaseDominio(tipo, _namespace));
                    }
                }
            }
            return sb.ToString();

        }

        private string RetornarDeclaracoesListaEntidadesTipoEntidade()
        {
            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposEntidade.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                sb.AppendLine(this.RetornarDeclaracaoListaTipoEntidade(AjudanteAssembly.TipoEntidade, "Snebur.Dominio"));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoEntidade);

                foreach (var tipo in tiposOrdenados)
                {
                    if (TipoUtil.TipoIgualOuSubTipo(tipo, AjudanteAssembly.TipoEntidade))
                    {
                        sb.AppendLine(this.RetornarDeclaracaoListaTipoEntidade(tipo, _namespace));
                    }
                }
            }
            return sb.ToString();
        }

        private string RetornarDeclaracaoListaTipoEntidade(Type tipo, string _namespace)
        {
            var declaracaoListaBaseEntidade = AjudanteReflexao.RetornarDeclaracaoListaTipoEntidade(tipo);
            //var caminhoTipoListaEntidade = AjudanteReflexao.RetornarCaminhoListaTipoEntidade(tipo);
            var nomeTipoListaEntidade = AjudanteReflexao.RetornarNomeTipoEntidade(tipo);
            var assemblyQualifiedName = AjudanteReflexao.RetornarAssemblyQualifiedNameListaTipoEntidade(tipo);
            //var declaracaoListaBaseDominio = AjudanteReflexao.RetornarDeclaracaoListaTipoBaseDominio(tipo);
            var declaracaoTipoEntidade = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo);

            var retorno = String.Format("const {0} = new Snebur.Reflexao.TipoListaEntidade(\"{1}\", \"{2}\", \"{3}\",{4});",
                                                          declaracaoListaBaseEntidade,
                                                          nomeTipoListaEntidade,
                                                          _namespace,
                                                          assemblyQualifiedName,
                                                          declaracaoTipoEntidade);
            return retorno;
        }

        private string RetornarAdicionarTiposBaseEntidade()
        {

            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposEntidade.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(AjudanteAssembly.TipoEntidade);
                var declaracao = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(AjudanteAssembly.TipoEntidade);

                sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoEntidade);

                foreach (var tipo in tiposOrdenados)
                {
                    var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(tipo);
                    var declaracao = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo);

                    sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
                }
            }
            return sb.ToString();
        }

        private string RetornarAdicionarTiposListaBaseDominioTipoEntidade()
        {
            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposEntidade.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                var caminhoTipo = AjudanteReflexao.RetornarCaminhoListaTipoBaseDominio(AjudanteAssembly.TipoEntidade);
                var declaracao = AjudanteReflexao.RetornarDeclaracaoListaTipoBaseDominio(AjudanteAssembly.TipoEntidade);

                sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoEntidade);

                foreach (var tipo in tiposOrdenados)
                {
                    if (TipoUtil.TipoIgualOuSubTipo(tipo, AjudanteAssembly.TipoEntidade))
                    {
                        var caminhoTipo = AjudanteReflexao.RetornarCaminhoListaTipoBaseDominio(tipo);
                        var declaracao = AjudanteReflexao.RetornarDeclaracaoListaTipoBaseDominio(tipo);

                        sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
                    }
                }
            }
            return sb.ToString();

        }

        private string RetornarAdicionarTiposListaBaseEntidade()
        {
            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposEntidade.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                var caminhoTipo = AjudanteReflexao.RetornarCaminhoListaTipoEntidade(AjudanteAssembly.TipoEntidade);
                var declaracao = AjudanteReflexao.RetornarDeclaracaoListaTipoEntidade(AjudanteAssembly.TipoEntidade);

                sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoEntidade);

                foreach (var tipo in tiposOrdenados)
                {
                    if (TipoUtil.TipoIgualOuSubTipo(tipo, AjudanteAssembly.TipoEntidade))
                    {
                        var caminhoTipo = AjudanteReflexao.RetornarCaminhoListaTipoEntidade(tipo);
                        var declaracao = AjudanteReflexao.RetornarDeclaracaoListaTipoEntidade(tipo);

                        sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
                    }
                }
            }
            return sb.ToString();
        }

        private string RetornarAssociarCaminhosTipoEntidade()
        {

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");


            var gruposNamespace = this.TiposEntidade.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();
            var nomePropriedadeCaminhoTipo = ReflexaoUtil.RetornarNomePropriedade<Snebur.Dominio.ICaminhoTipo>(x => x.__CaminhoTipo);

            if (this.ProjetoSneburDominio)
            {
                var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(AjudanteAssembly.TipoEntidade);
                sb.AppendLine(String.Format("{0}.{1} = \"{0}\";", caminhoTipo, nomePropriedadeCaminhoTipo));
            }
            foreach (var grupoNamespace in gruposNamespace)
            {
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoEntidade);
                foreach (var tipo in tiposOrdenados)
                {
                    var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(tipo);
                    sb.AppendLine(String.Format("{0}.{1} = \"{0}\";", caminhoTipo, nomePropriedadeCaminhoTipo));
                }
            }
            return sb.ToString();
        }

        private string RetornarAtributosTiposEntidade()
        {
            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposEntidade.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                sb.AppendLine(this.RetornarConteudoAtributosTipo(AjudanteAssembly.TipoEntidade));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoEntidade);

                foreach (var tipo in tiposOrdenados)
                {
                    sb.AppendLine(this.RetornarConteudoAtributosTipo(tipo));
                }
            }
            return sb.ToString();

        }
    }
}
