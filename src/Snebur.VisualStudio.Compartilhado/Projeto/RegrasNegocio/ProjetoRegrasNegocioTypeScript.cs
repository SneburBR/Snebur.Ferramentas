using Snebur.AcessoDados;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.RegrasNegocio;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Snebur.VisualStudio
{
    public class ProjetoRegrasNegocioTypeScript : BaseProjeto<ConfiguracaoProjetoRegrasNegocio>
    {

        public ProjetoRegrasNegocioTypeScript(ProjetoViewModel projetoVM,
                                              ConfiguracaoProjetoRegrasNegocio configuracaoProjeto, 
                                              FileInfo arquivoProjeto, 
                                              string caminhoConfiguracao) : base(projetoVM, configuracaoProjeto, arquivoProjeto, caminhoConfiguracao)
        {
        }

        protected override void AtualizarInterno()
        {
            ValidacaoUtil.ValidarReferenciaNula(this.ConfiguracaoProjeto.CaminhoExtensaoTypeScript, nameof(this.ConfiguracaoProjeto.CaminhoExtensaoTypeScript));
            var caminhoExtensao = CaminhoUtil.RetornarCaminhosAbsoluto(this.ConfiguracaoProjeto.CaminhoExtensaoTypeScript, this.CaminhoProjeto);
            if (!File.Exists(caminhoExtensao))
            {
                throw new FileNotFoundException(caminhoExtensao);
            }

            var tipos = this.RetornarTodosTipo();
            var sb = new StringBuilder();

            var tiposExtensaoEntidade = tipos.Where(x => this.IsExtensaoEntidade(x)).ToList();

            var gruposEntidades = tiposExtensaoEntidade.GroupBy(x => this.RetornarTipoEntidade(x).Namespace);
            foreach (var grupoNamespace in gruposEntidades)
            {
                sb.AppendLine($"namespace " + grupoNamespace.Key);
                sb.AppendLine("{");
                foreach (var tipo in grupoNamespace)
                {
                    sb.AppendLine(this.RetornarConteudoInterfaceExtensao(tipo));
                }
                foreach (var tipo in tiposExtensaoEntidade)
                {
                    sb.AppendLine(this.RetornarConteudoClasseExtensao(tipo));
                }
                sb.AppendLine("}");
            }

            var tiposClasses = tipos.Where(x => this.IsClasseNegocio(x)).ToList();
            var gruposClasse = tiposClasses.GroupBy(x => x.Namespace);

            foreach (var grupoNamespace in gruposClasse)
            {
                sb.AppendLine($"namespace " + grupoNamespace.Key);
                sb.AppendLine("{");
                foreach (var tipo in grupoNamespace)
                {
                    sb.AppendLine(this.RetornarConteudoNovaClasse(tipo));
                }
                sb.AppendLine("}");
            }


            var conteudo = sb.ToString();
            ArquivoUtil.SalvarTexto(caminhoExtensao, conteudo);

        }

        #region Extensao entidade

        private bool IsExtensaoEntidade(Type tipo)
        {
            return tipo.IsAbstract && tipo.IsSealed &&
                tipo.IsPublic &&
                !TipoUtil.TipoPossuiAtributo(tipo, typeof(IgnoraClasseRegraNegocioAttribute), true) &&
                tipo.GetMethods().Any(x => this.IsMetodoNegocioExtensaoEntidade(tipo, x));
        }

        private string RetornarConteudoInterfaceExtensao(Type tipo)
        {
            var sb = new StringBuilder();
            var tipoEntidade = this.RetornarTipoEntidade(tipo);
            sb.AppendLine($"\texport interface {TipoUtil.RetornarNomeTipoTS(tipoEntidade)}");
            sb.AppendLine("\t{");

            var metodos = tipo.GetMethods().Where(x => this.IsMetodoNegocioExtensaoEntidade(tipo, x));
            foreach (var metodo in metodos)
            {
                var parametrosExtensao = this.RetornarParametrosExtensao(metodo);
                var parametrosConcatenados = String.Join(", ", parametrosExtensao.Select(x => $"{x.Name} : {TipoUtil.RetornarCaminhoTipoTS(x.ParameterType)}"));
                sb.AppendLine($"\t\t{metodo.Name}Async({parametrosConcatenados}):Promise<{TipoUtil.RetornarCaminhoTipoTS(metodo.ReturnType)}>;");
            }
            sb.AppendLine("\t}");
            return sb.ToString();
        }

        private Type RetornarTipoEntidade(Type tipo)
        {
            var metodosNegocio = tipo.GetMethods().Where(x => this.IsMetodoNegocioExtensaoEntidade(tipo, x)).ToList();
            if (metodosNegocio.Any(x => x.GetParameters().Count() == 0))
            {
                throw new Erro($"Existe métodos vazios dentro de {tipo.Name}");
            }
            var tipos = metodosNegocio.Select(x => x.GetParameters().First().ParameterType).Distinct().ToArray();
            if (tipos.Count() != 1)
            {
                throw new Erro($"O tipos da extensão em {tipo.Name} da regra de negocio são diferentes apenas um tipo por classe é suportado");
            }
            return tipos.Single();
        }

        private List<ParameterInfo> RetornarParametrosExtensao(MethodInfo metodo)
        {
            ////o primeiro parametro é tipo da extensao
            //o primeiro parametro é tipo da extensao
            //o segundo é contexto dados
            var parametros = metodo.GetParameters().Skip(1).ToList();
            var parametroContexto = parametros.FirstOrDefault();
            if (parametroContexto == null || !TipoUtil.TipoIgualOuSubTipo(parametroContexto.ParameterType, typeof(__BaseContextoDados)))
            {
                throw new Erro($"O método {metodo.Name} da entenxao {metodo.DeclaringType.Name} não possui o paratro da contexto dados");
            }
            return parametros.Skip(1).ToList();
        }

        private List<ParameterInfo> RetornarPagametrosExtesaoSemContexto(MethodInfo metodo)
        {
            var parametros = metodo.GetParameters().ToList();
            var parametroContexto = parametros.Count > 1 ? parametros[1] : null;
            if (parametroContexto == null || !TipoUtil.TipoIgualOuSubTipo(parametroContexto.ParameterType, typeof(__BaseContextoDados)))
            {
                throw new Erro($"O método {metodo.Name} da entenxao {metodo.DeclaringType.Name} não possui o paratro da contexto dados");
            }
            parametros.RemoveAt(1);
            return parametros;
        }

        private bool IsMetodoNegocioExtensaoEntidade(Type tipoDeclarado, MethodInfo metodo)
        {
            return metodo.IsStatic && metodo.GetParameters().Count() > 0 &&
                   this.IsMetodoNegocio(tipoDeclarado, metodo) &&
                   TipoUtil.TipoIgualOuSubTipo(metodo.GetParameters().First().ParameterType, typeof(Entidade)) &&
                   !PropriedadeUtil.PossuiAtributo(metodo, typeof(IgnorarMetodoTSAttribute)) &&
                   !PropriedadeUtil.PossuiAtributo(metodo, typeof(IgnorarMetodoRegraNegocioAttribute));

        }

        private bool IsMetodoNegocio(Type tipoDeclarado, MethodInfo metodo)
        {
            return metodo.IsPublic && metodo.DeclaringType == tipoDeclarado &&
                   metodo.GetCustomAttribute<IgnorarMetodoTSAttribute>() == null &&
                   !PropriedadeUtil.PossuiAtributo(metodo, typeof(IgnorarMetodoTSAttribute)) &&
                   !PropriedadeUtil.PossuiAtributo(metodo, typeof(IgnorarMetodoRegraNegocioAttribute));
        }

        private string RetornarConteudoClasseExtensao(Type tipo)
        {
            var sb = new StringBuilder();

            var tipoEntidade = this.RetornarTipoEntidade(tipo);
            var metodos = tipo.GetMethods().Where(x => this.IsMetodoNegocioExtensaoEntidade(tipo, x));

            var assemblyQualifiedName = $"{tipo.FullName}, {tipo.Assembly.GetName().Name}";

            foreach (var metodo in metodos)
            {

                var parametrosExtensao = this.RetornarParametrosExtensao(metodo);
                var parametrosSemContexto = this.RetornarPagametrosExtesaoSemContexto(metodo);

                var parametrosConcatenados = String.Join(", ", parametrosExtensao.Select(x => $"{x.Name} : {TipoUtil.RetornarCaminhoTipoTS(x.ParameterType)}"));
                var parametroEntidade = metodo.GetParameters().First();

                sb.AppendLine($"\t{TipoUtil.RetornarCaminhoTipoTS(tipoEntidade)}.prototype.{metodo.Name}Async = function({parametrosConcatenados}) : Promise<{TipoUtil.RetornarCaminhoTipoTS(metodo.ReturnType)}> ");
                sb.AppendLine("\t{");
                sb.AppendLine($"\t\tlet {parametroEntidade.Name} = this");
                sb.AppendLine($"\t\treturn new Promise<{TipoUtil.RetornarCaminhoTipoTS(metodo.ReturnType)}> (resolver =>");
                sb.AppendLine("\t\t{");

                var parametrosRegras = String.Join(", ", parametrosSemContexto.Select(x => this.RetornarParametroRegraNegocio(x)));

                sb.AppendLine($"\t\t\tlet chamadaRegraNegocio = new Snebur.Comunicacao.ChamadaRegraNegocio();");
                sb.AppendLine($"\t\t\tchamadaRegraNegocio.NomeMetodo = \"{metodo.Name}\";");
                sb.AppendLine($"\t\t\tchamadaRegraNegocio.AssemblyQualifiedName = \"{assemblyQualifiedName}\";");

                if (!String.IsNullOrWhiteSpace(parametrosRegras))
                {
                    sb.AppendLine($"\t\t\t$Aplicacao.ServicoRegrasNegocio.ChamarRegraAsync(chamadaRegraNegocio, {parametrosRegras}, function (resultado: any)");
                }
                else
                {
                    sb.AppendLine($"\t\t\t$Aplicacao.ServicoRegrasNegocio.ChamarRegraAsync(chamadaRegraNegocio, function (resultado: any)");
                }

                sb.AppendLine("\t\t\t{");
                sb.AppendLine("\t\t\t\tresolver(resultado);");
                sb.AppendLine("\t\t\t});");

                sb.AppendLine("\t\t});");
                sb.AppendLine("\t}");
            }

            return sb.ToString();
        }

        private string RetornarParametroRegraNegocio(ParameterInfo parametro)
        {
            return $" {{ NomeParametro : \"{parametro.Name}\", ValorParametro : {parametro.Name} }} ";
        }

        #endregion

        #region Classe negocio

        private bool IsClasseNegocio(Type tipo)
        {
            try
            {
                return !this.IsExtensaoEntidade(tipo) && tipo.IsPublic && !tipo.IsAbstract &&
                    !TipoUtil.TipoPossuiAtributo(tipo, typeof(IgnoraClasseRegraNegocioAttribute), true) &&
                    TipoUtil.TipoSubTipo(tipo, typeof(BaseNegocio));
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
                return false;
            }

        }

        private string RetornarConteudoNovaClasse(Type tipo)
        {

            var sb = new StringBuilder();
            var metodos = tipo.GetMethods().Where(x => this.IsMetodoNegocio(tipo, x));
            var assemblyQualifiedName = $"{tipo.FullName}, {tipo.Assembly.GetName().Name}";


            sb.AppendLine($"\texport class {TipoUtil.RetornarNomeTipoTS(tipo)}");
            sb.AppendLine("\t{");
            foreach (var metodo in metodos)
            {
                var parametros = metodo.GetParameters();
                var parametrosConcatenados = String.Join(", ", parametros.Select(x => $"{x.Name} : {TipoUtil.RetornarCaminhoTipoTS(x.ParameterType)}"));
                sb.AppendLine($"\t\tpublic  {metodo.Name}Async = function({parametrosConcatenados}) : Promise<{TipoUtil.RetornarCaminhoTipoTS(metodo.ReturnType)}> ");
                sb.AppendLine("\t\t{");

                sb.AppendLine($"\t\treturn new Promise<{TipoUtil.RetornarCaminhoTipoTS(metodo.ReturnType)}> (resolver =>");
                sb.AppendLine("\t\t\t{");

                var parametrosRegras = String.Join(", ", metodo.GetParameters().Select(x => this.RetornarParametroRegraNegocio(x)));

                sb.AppendLine($"\t\t\t\tlet chamadaRegraNegocio = new Snebur.Comunicacao.ChamadaRegraNegocio();");
                sb.AppendLine($"\t\t\t\tchamadaRegraNegocio.NomeMetodo = \"{metodo.Name}\";");
                sb.AppendLine($"\t\t\t\tchamadaRegraNegocio.AssemblyQualifiedName = \"{assemblyQualifiedName}\";");
                if (!String.IsNullOrWhiteSpace(parametrosRegras))
                {
                    sb.AppendLine($"\t\t\t\t$Aplicacao.ServicoRegrasNegocio.ChamarRegraAsync(chamadaRegraNegocio, {parametrosRegras}, function (resultado: any)");
                }
                else
                {
                    sb.AppendLine($"\t\t\t\t$Aplicacao.ServicoRegrasNegocio.ChamarRegraAsync(chamadaRegraNegocio, function (resultado: any)");
                }

                sb.AppendLine("\t\t\t\t{");
                sb.AppendLine("\t\t\t\t\tresolver(resultado);");
                sb.AppendLine("\t\t\t\t});");

                sb.AppendLine("\t\t\t});");
                sb.AppendLine("\t\t}");
            }
            sb.AppendLine("\t}");
            var retorno = sb.ToString();
            return retorno;

        }

        #endregion

        private List<Type> RetornarTodosTipo()
        {
            if (!File.Exists(this.CaminhoAssembly))
            {
                throw new FileNotFoundException(this.CaminhoAssembly);
            }
            var assembly = AjudanteAssembly.RetornarAssembly(this.CaminhoAssembly);
            try
            {
                return assembly.GetAccessibleTypes().Where(x => !x.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)).ToList();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(x => x != null && !x.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)).ToList();
            }
            catch (Exception)
            {
                throw;
            }

        }


        protected override void DispensarInerno()
        {

        }
    }
}
