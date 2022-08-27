using System;
using System.Linq;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public partial class GeradorDominioReflexao
    {

        public string RetornarConteudoBaseDominio()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("//BaseDominio");
            sb.AppendLine(this.RetornarDeclaracoesTipoBaseDominio());
            sb.AppendLine("//ListaBaseDominio");
            sb.AppendLine(this.RetornarDeclaracoesListaTipoBaseDominio());
            sb.AppendLine("//Adicionar BasesDominio");
            sb.AppendLine(this.RetornarAdicionarTiposBaseDominio());

            sb.AppendLine(this.RetornarAdicionarListaTiposBaseDominio());
            sb.AppendLine("//Associar caminhos BaseDominio");
            sb.AppendLine(this.RetornarAssociarCaminhosTipoBaseDominio());
            sb.AppendLine("//Atributos");
            sb.AppendLine(this.RetornarAtributosTiposBaseDominio());
            return sb.ToString();
        }

        private string RetornarDeclaracoesTipoBaseDominio()
        {
            var sb = new System.Text.StringBuilder();
            var gruposNamespace = this.TiposBaseDominio.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                sb.AppendLine(this.RetornarDeclaracaoTipoBaseDominio(AjudanteAssembly.TipoBaseDominio, "Snebur.Dominio"));
                //sb.AppendLine(this.RetornarDeclaracaoTipoBaseDominio(AjudanteAssembly.TipoEntidade, "Snebur.Dominio"));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TiposBaseDominio);

                foreach (var tipo in tiposOrdenados)
                {
                    sb.AppendLine(this.RetornarDeclaracaoTipoBaseDominio(tipo, _namespace));
                }
            }
            return sb.ToString();
        }

        private string RetornarDeclaracaoTipoBaseDominio(Type tipo, string _namespace)
        {
            var declaracaoTipoReflexao = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo);
            var construtorBaseDominio = String.Format("{0}.{1}", tipo.Namespace, tipo.Name);
            //var caminhoTipoBaseDominio = AjudanteReflexao.RetornarCaminhoTipoBaseDominio(tipo);
            var declaracaoTipoBaseDominioBase = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo.BaseType);
            var assemblyQualifiedName = AjudanteReflexao.RetornarAssemblyQualifiedNameTipoBaseDominio(tipo);
            var abstrato = tipo.IsAbstract;

            var retorno = String.Format("const {0} = new Snebur.Reflexao.TipoBaseDominio({1}, \"{2}\", \"{3}\",\"{4}\",{5}, {6});",
                                                 declaracaoTipoReflexao,
                                                 construtorBaseDominio,
                                                 tipo.Name,
                                                 _namespace,
                                                 assemblyQualifiedName,
                                                 declaracaoTipoBaseDominioBase,
                                                 abstrato.ToString().ToLower());
            return retorno;
        }

        private string RetornarDeclaracoesListaTipoBaseDominio()
        {
            var sb = new System.Text.StringBuilder();


            var gruposNamespace = this.TiposBaseDominio.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                sb.AppendLine(this.RetornarDeclaracaoListaTipoBaseDominio(AjudanteAssembly.TipoBaseDominio, "Snebur.Dominio"));
                //sb.AppendLine(this.RetornarDeclaracaoListaTipoBaseDominio(AjudanteAssembly.TipoEntidade, "Snebur.Dominio"));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), 
                                                                AjudanteAssembly.TiposBaseDominio );

                foreach (var tipo in tiposOrdenados)
                {
                    sb.AppendLine(this.RetornarDeclaracaoListaTipoBaseDominio(tipo, _namespace));
                }
            }
            return sb.ToString();
        }

        private string RetornarDeclaracaoListaTipoBaseDominio(Type tipo, string _namespace)
        {
            var declaracaoReflexaoListaBaseDominio = AjudanteReflexao.RetornarDeclaracaoListaTipoBaseDominio(tipo);
            //var caminhoTipo = AjudanteReflexao.RetornarCaminhoListaTipoBaseDominio(tipo);
            var nomeTipo = AjudanteReflexao.RetornarNomeListaTipoBaseDominio(tipo);
            var assemblyQualifiedName = AjudanteReflexao.RetornarAssemblyQualifiedNameListaTipoBaseDominio(tipo);
            var caminhoTipoReflexaoBaseDominio = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo);

            var retorno = String.Format("const {0} = new Snebur.Reflexao.TipoListaBaseDominio(\"{1}\", \"{2}\", \"{3}\",{4});",
                                                          declaracaoReflexaoListaBaseDominio,
                                                          nomeTipo,
                                                          _namespace,
                                                          assemblyQualifiedName,
                                                          caminhoTipoReflexaoBaseDominio);
            return retorno;
        }

        private string RetornarAdicionarTiposBaseDominio()
        {

            var sb = new System.Text.StringBuilder();
            var gruposNamespace = this.TiposBaseDominio.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(AjudanteAssembly.TipoBaseDominio);
                var declaracao = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(AjudanteAssembly.TipoBaseDominio);
                sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TiposBaseDominio);

                foreach (var tipo in tiposOrdenados)
                {
                    var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(tipo);
                    var declaracao = AjudanteReflexao.RetornarDeclaracaoTipoBaseDominio(tipo);

                    sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
                }
            }
            return sb.ToString();
        }

        private string RetornarAdicionarListaTiposBaseDominio()
        {
            var sb = new System.Text.StringBuilder();

            var gruposNamespace = this.TiposBaseDominio.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                var caminhoTipo = AjudanteReflexao.RetornarCaminhoListaTipoBaseDominio(AjudanteAssembly.TipoBaseDominio);
                var declaracao = AjudanteReflexao.RetornarDeclaracaoListaTipoBaseDominio(AjudanteAssembly.TipoBaseDominio);
                sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));

                //var caminhoTipoEntidade = AjudanteReflexao.RetornarCaminhoListaTipoBaseDominio(AjudanteAssembly.TipoEntidade);
                //var declaracaoEntidade = AjudanteReflexao.RetornarDeclaracaoListaTipoBaseDominio(AjudanteAssembly.TipoEntidade);
                //sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipoEntidade, declaracaoEntidade));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TiposBaseDominio);

                foreach (var tipo in tiposOrdenados)
                {
                    var caminhoTipo = AjudanteReflexao.RetornarCaminhoListaTipoBaseDominio(tipo);
                    var declaracao = AjudanteReflexao.RetornarDeclaracaoListaTipoBaseDominio(tipo);

                    sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracao));
                }
            }
            return sb.ToString();
        }

        private string RetornarAssociarCaminhosTipoBaseDominio()
        {

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");

            var gruposNamespace = this.TiposBaseDominio.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();
            var nomePropriedadeCaminhoTipo = ReflexaoUtil.RetornarNomePropriedade<Snebur.Dominio.ICaminhoTipo>(x => x.__CaminhoTipo);

            if (this.ProjetoSneburDominio)
            {
                var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(AjudanteAssembly.TipoBaseDominio);

                sb.AppendLine(String.Format("{0}.{1} = \"{0}\";", caminhoTipo, nomePropriedadeCaminhoTipo));
            }
            foreach (var grupoNamespace in gruposNamespace)
            {
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TiposBaseDominio);
                foreach (var tipo in tiposOrdenados)
                {
                    var caminhoTipo = TipoUtil.RetornarCaminhoTipoTS(tipo);
                    sb.AppendLine(String.Format("{0}.{1} = \"{0}\";", caminhoTipo, nomePropriedadeCaminhoTipo));
                }
            }
            return sb.ToString();
        }

        public string RetornarAtributosTiposBaseDominio()
        {
            var sb = new System.Text.StringBuilder();
            var gruposNamespace = this.TiposBaseDominio.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            if (this.ProjetoSneburDominio)
            {
                sb.AppendLine(this.RetornarConteudoAtributosTipo(AjudanteAssembly.TipoBaseDominio));
                //sb.AppendLine(this.RetornarDeclaracaoTipoBaseDominio(AjudanteAssembly.TipoEntidade, "Snebur.Dominio"));
            }

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TiposBaseDominio);

                foreach (var tipo in tiposOrdenados)
                {
                    sb.AppendLine(this.RetornarConteudoAtributosTipo(tipo));
                }
            }
            return sb.ToString();
        }
    }
}
