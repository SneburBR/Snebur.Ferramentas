using Snebur.AcessoDados;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
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
    public class ProjetoRegrasNegocioCSharp : BaseProjeto<ConfiguracaoProjetoRegrasNegocio>
    {
        public ProjetoRegrasNegocioCSharp(ProjetoViewModel projetoVM, 
                                          ConfiguracaoProjetoRegrasNegocio configuracaoProjeto,
                                          FileInfo arquivoProjeto,
                                          string caminhoConfiguracao) : base(projetoVM, configuracaoProjeto, arquivoProjeto, caminhoConfiguracao)
        {
        }

        protected override void AtualizarInterno()
        {
            ValidacaoUtil.ValidarReferenciaNula(this.ConfiguracaoProjeto.CaminhoExtensaoTypeScript, nameof(this.ConfiguracaoProjeto.CaminhoExtensaoTypeScript));
            var caminhoExtensao = CaminhoUtil.RetornarCaminhoAbsoluto(this.ConfiguracaoProjeto.CaminhoExtensaoCSharp, this.CaminhoProjeto);
            if (!File.Exists(caminhoExtensao))
            {
                throw new FileNotFoundException(caminhoExtensao);
            }

            var tipos = this.RetornarTodosTipo();
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Reflection;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Snebur;");
            sb.AppendLine("using Snebur.AcessoDados;");
            sb.AppendLine("");

            var tiposExtensaoEntidade = tipos.Where(x => this.IsExtensaoEntidade(x)).ToList();

            var gruposEntidade = tiposExtensaoEntidade.GroupBy(x => this.RetornarTipoEntidade(x).Namespace);
            foreach (var grupoNamespace in gruposEntidade)
            {

                sb.AppendLine($"namespace " + grupoNamespace.Key);
                sb.AppendLine("{");
                foreach (var tipo in grupoNamespace)
                {
                    var conteudoClasse = this.RetornarConteudoExtensao(tipo);
                    sb.AppendLine(conteudoClasse);
                    sb.AppendLine("");
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
                    sb.AppendLine(this.RetornarConteudoaClasseNegocio(tipo));
                }
                sb.AppendLine("}");
            }

            var conteudo = sb.ToString();
            LocalProjetoUtil.SalvarDominio(caminhoExtensao, conteudo);
        }

        #region Extensao entidade

        private bool IsExtensaoEntidade(Type tipo)
        {
            return tipo.IsAbstract && tipo.IsSealed && tipo.IsPublic && tipo.GetMethods().Any(x => this.IsMetodoNegocioExtensaoEntidade(tipo, x));
        }

        private string RetornarConteudoExtensao(Type tipo)
        {
            var sb = new StringBuilder();

            var assemblyQualifiedName = $"{tipo.FullName}, {tipo.Assembly.GetName().Name}";
            var tipoEntidade = this.RetornarTipoEntidade(tipo);
            sb.AppendLine($"\tpublic static class {TipoUtil.RetornarNomeTipoTS(tipoEntidade)}Extensao");
            sb.AppendLine("\t{");

            var metodos = tipo.GetMethods().Where(x => this.IsMetodoNegocioExtensaoEntidade(tipo, x));
            foreach (var metodo in metodos)
            {
                //var conteudoMetodo = this.RetornarConteudoMetodoExtensao(tipo.)
                var parametrosExtensao = this.RetornarParametrosExtensao(metodo);
                var parametrosSemContexto = this.RetornarPagametrosExtesaoSemContexto(metodo);

                var parametrosTipoNomeConcatenados = String.Join(", ", parametrosExtensao.Select(x => $"{TipoCSUtil.RetornarNomeTipo(x.ParameterType)} {x.Name}"));
                var parametrosNomeConcatenados = String.Join(",", parametrosSemContexto.Select(x => x.Name));

                var seperadorDeclaracao = (parametrosExtensao.Count > 0) ? ", " : String.Empty;
                var seperadorChamada = (metodo.GetParameters().Count() > 0) ? ", " : String.Empty;

                var parametroEntidade = metodo.GetParameters().First();
                var nomeVariavelEntidade = parametroEntidade.Name;


                //await
                sb.AppendLine($"\t\tpublic static Task{TipoCSUtil.RetornarNomeTipo(metodo.ReturnType, true)} {metodo.Name}Async(this {tipoEntidade.Name} {nomeVariavelEntidade}{seperadorDeclaracao}{parametrosTipoNomeConcatenados})");
                sb.AppendLine("\t\t{");
                sb.AppendLine($"\t\t\treturn Task.Factory.StartNew{TipoCSUtil.RetornarNomeTipo(metodo.ReturnType, true)}(()=>");
                sb.AppendLine("\t\t\t{");

                sb.Append("\t\t\t\t");
                if (metodo.ReturnType != typeof(void))
                {
                    sb.Append("return ");
                }
                sb.Append($"{tipo.Name}.{metodo.Name}({parametrosNomeConcatenados});");
                sb.AppendLine("");

                sb.AppendLine("\t\t\t});");
                sb.AppendLine("\t\t}");
                sb.AppendLine("");

                //sync

                sb.AppendLine($"\t\tpublic static {TipoCSUtil.RetornarNomeTipo(metodo.ReturnType, false, false)} {metodo.Name}(this {tipoEntidade.Name} {nomeVariavelEntidade}{seperadorDeclaracao}{parametrosTipoNomeConcatenados})");
                sb.AppendLine("\t\t{");

                sb.AppendLine($"\t\t\tvar chamadaRegraNegocio = new Snebur.Comunicacao.ChamadaRegraNegocio");
                sb.AppendLine("\t\t\t{");
                sb.AppendLine($"\t\t\t\tNomeMetodo = \"{metodo.Name}\",");
                sb.AppendLine($"\t\t\t\tAssemblyQualifiedName = \"{assemblyQualifiedName /*tipoEntidade.AssemblyQualifiedName*/}\"");
                sb.AppendLine("\t\t\t};");
                sb.AppendLine("");
                sb.AppendLine("\t\t\tif (AplicacaoSnebur.Atual is IAplicacaoServicoRegraNegocio aplicacao)");
                sb.AppendLine("\t\t\t{");
                sb.AppendLine($"\t\t\t\tvar parametros = new object[] {{ chamadaRegraNegocio {seperadorChamada}{parametrosNomeConcatenados} }};");
                sb.AppendLine("");

                sb.Append("\t\t\t\t");
                if (metodo.ReturnType != typeof(void))
                {
                    sb.Append($"return ({TipoCSUtil.RetornarNomeTipo(metodo.ReturnType, false, false)})");
                }
                sb.Append("aplicacao.ServicoRegrasNegocio.ChamarRegra(MethodBase.GetCurrentMethod(), chamadaRegraNegocio,  parametros);");
                sb.AppendLine("");

                sb.AppendLine("\t\t\t}");
                sb.AppendLine("\t\t\telse");
                sb.AppendLine("\t\t\t{");
                sb.AppendLine("\t\t\t\tthrow new Erro($\" a aplicacao { AplicacaoSnebur.Atual.GetType().Name} não implementa a interrface { nameof(IAplicacaoServicoRegraNegocio)}\");");
                sb.AppendLine("\t\t\t}");
                sb.AppendLine("\t\t}");
                sb.AppendLine("");
            }
            sb.AppendLine("\t}");
            return sb.ToString();
        }

        private Type RetornarTipoEntidade(Type tipo)
        {
            var metodosNegocio = tipo.GetMethods().Where(x => this.IsMetodoNegocioExtensaoEntidade(tipo, x)).ToList();
            if (metodosNegocio.Any(x => x.GetParameters().Count() == 0))
            {
                throw new Erro($"Existe metodos vazios dentro de {tipo.Name}");
            }
            var tipos = metodosNegocio.Select(x => x.GetParameters().First().ParameterType).Distinct().ToArray();
            if (tipos.Count() != 1)
            {
                throw new Erro($"O tipos da extensão em {tipo.Name} da regra de neogico são diferents apenas um tipo por classe é suportado");
            }
            return tipos.Single();
        }

        private List<ParameterInfo> RetornarParametrosExtensao(MethodInfo metodo)
        {
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
                    tipo.GetConstructors().Any(x => x.GetParameters().Count() == 0);
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
                return false;
            }

        }

        private string RetornarConteudoaClasseNegocio(Type tipoNegocio)
        {
            var sb = new StringBuilder();
            var metodos = tipoNegocio.GetMethods().Where(x => this.IsMetodoNegocio(tipoNegocio, x));
            var assemblyQualifiedName = $"{tipoNegocio.FullName}, {tipoNegocio.Assembly.GetName().Name}";



            sb.AppendLine($"\tpublic class {TipoUtil.RetornarNomeTipoTS(tipoNegocio)}");
            sb.AppendLine("\t{");
            foreach (var metodo in metodos)
            {
                var parametros = metodo.GetParameters();
                var parametrosTipoNomeConcatenados = String.Join(", ", parametros.Select(x => $"{TipoCSUtil.RetornarNomeTipo(x.ParameterType)} {x.Name}"));
                var parametrosNomeConcatenados = String.Join(",", metodo.GetParameters().Select(x => x.Name));
                var seperadorChamada = (metodo.GetParameters().Count() > 0) ? ", " : String.Empty;

                //await
                sb.AppendLine($"\t\tpublic  Task{TipoCSUtil.RetornarNomeTipo(metodo.ReturnType, true)} {metodo.Name}Async(  {parametrosTipoNomeConcatenados})");
                sb.AppendLine("\t\t{");
                sb.AppendLine($"\t\t\treturn Task.Factory.StartNew{TipoCSUtil.RetornarNomeTipo(metodo.ReturnType, true)}(()=>");
                sb.AppendLine("\t\t\t{");

                sb.Append("\t\t\t\t");
                if (metodo.ReturnType != typeof(void))
                {
                    sb.Append("return ");
                }
                sb.Append($"this.{metodo.Name}({parametrosNomeConcatenados});");
                sb.AppendLine("");

                sb.AppendLine("\t\t\t});");
                sb.AppendLine("\t\t}");
                sb.AppendLine("");

                //sync
                sb.AppendLine($"\t\tpublic {TipoCSUtil.RetornarNomeTipo(metodo.ReturnType, false, false)} {metodo.Name}({parametrosTipoNomeConcatenados})");
                sb.AppendLine("\t\t{");

                sb.AppendLine($"\t\t\tvar chamadaRegraNegocio = new Snebur.Comunicacao.ChamadaRegraNegocio");
                sb.AppendLine("\t\t\t{");
                sb.AppendLine($"\t\t\t\tNomeMetodo = \"{metodo.Name}\",");
                sb.AppendLine($"\t\t\t\tAssemblyQualifiedName = \"{tipoNegocio.AssemblyQualifiedName}\"");
                sb.AppendLine("\t\t\t};");
                sb.AppendLine("");
                sb.AppendLine("\t\t\tif (AplicacaoSnebur.Atual is IAplicacaoServicoRegraNegocio aplicacao)");
                sb.AppendLine("\t\t\t{");
                sb.AppendLine($"\t\t\t\tvar parametros = new object[] {{ chamadaRegraNegocio {seperadorChamada}{parametrosNomeConcatenados} }};");
                sb.AppendLine("");

                sb.Append("\t\t\t\t");
                if (metodo.ReturnType != typeof(void))
                {
                    sb.Append($"return ({TipoCSUtil.RetornarNomeTipo(metodo.ReturnType, false, false)})");
                }
                sb.Append("aplicacao.ServicoRegrasNegocio.ChamarRegra(MethodBase.GetCurrentMethod(), chamadaRegraNegocio,  parametros);");
                sb.AppendLine("");

                sb.AppendLine("\t\t\t}");
                sb.AppendLine("\t\t\telse");
                sb.AppendLine("\t\t\t{");
                sb.AppendLine("\t\t\t\tthrow new Erro($\" a aplicacao { AplicacaoSnebur.Atual.GetType().Name} não implementa a interrface { nameof(IAplicacaoServicoRegraNegocio)}\");");
                sb.AppendLine("\t\t\t}");
                sb.AppendLine("\t\t}");
                sb.AppendLine("");



            }
            sb.AppendLine("\t}");
            var retorno = sb.ToString();
            return retorno;

        }

        #endregion


        //private string RetornarConteudoMetodoSync()
        //{

        //}
         
        protected override List<Type> RetornarTodosTipo()
        {
            if (!this.IsExisteDll)
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

        internal static ConfiguracaoProjetoRegrasNegocio RetornarConfiguracao(string caminhoConfiguracao)
        {
            var json = File.ReadAllText(caminhoConfiguracao, UTF8Encoding.UTF8);
            return JsonUtil.Deserializar<ConfiguracaoProjetoRegrasNegocio>(json, EnumTipoSerializacao.Javascript);
        }

        protected override void DispensarInerno()
        {

        }
    }
}
