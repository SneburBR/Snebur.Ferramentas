using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Snebur.VisualStudio
{
    public class ProjetoServicosDotNet : BaseProjeto<ConfiguracaoProjetoServico>
    {
        private const string REGION_AUTOMATICO = "#region Automático";
        private const string END_REGION = "#endregion";

        public List<Type> TodosTipo { get; }


        public ProjetoServicosDotNet(ProjetoViewModel projetoVM, 
                                     ConfiguracaoProjetoServico configuracaoProjeto,
                                     FileInfo arquivoProjeto, 
                                     string caminhoConfiguracao) : base(projetoVM, configuracaoProjeto, arquivoProjeto, caminhoConfiguracao)
        {
            this.TodosTipo = this.RetornarTodosTipo();
         }

        protected override void AtualizarInterno()
        {


            foreach (var configServico in this.ConfiguracaoProjeto.Servicos)
            {
                var tipoInterface = this.TodosTipo.Where(x => x.IsInterface && x.Name == configServico.NomeInterface).SingleOrDefault();
                if (tipoInterface == null)
                {
                    throw new Exception($"O interface {tipoInterface} não foi encontrada em {this.CaminhoAssembly} ");
                }

                if (!String.IsNullOrWhiteSpace(configServico.CaminhoDotNet))
                {
                    var caminhoServico = Path.GetFullPath(Path.Combine(this.CaminhoProjeto, configServico.CaminhoDotNet));
                    if (!File.Exists(caminhoServico))
                    {
                        throw new FileNotFoundException(caminhoServico);
                    }
                    var conteudo = ArquivoUtil.LerTexto(caminhoServico).Trim();
                    var linhas = conteudo.ToLines();

                    var sb = new StringBuilder();
                    sb.AppendLine($"\t\t{REGION_AUTOMATICO}");

                    var metodos = tipoInterface.GetMethods();
                    foreach (var metodo in metodos)
                    {
                        var m = metodo;

                        var retorno = (metodo.ReturnType == typeof(void)) ? String.Empty : "return ";
                        var parametros = metodo.GetParameters().Select(x => $"{TipoCSUtil.RetornarNomeTipo(x.ParameterType)} {x.Name}").ToList();
                        var descricaoParametros = String.Join(", ", parametros);

                        sb.AppendLine("");
                        sb.AppendLine($"\t\tpublic {TipoCSUtil.RetornarNomeTipo(metodo.ReturnType)} {metodo.Name}({descricaoParametros})");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine($"\t\t\tvar parâmetros = new object[] {{ {String.Join(",", metodo.GetParameters().Select(x => x.Name)) } }}; ");
                        sb.AppendLine($"\t\t\t{retorno}this.ChamarServico{TipoCSUtil.RetornarNomeTipo(metodo.ReturnType, true, true)}(MethodBase.GetCurrentMethod(), parametros);");
                        sb.AppendLine("\t\t}");

                        sb.AppendLine("");

                        sb.AppendLine("\t\t//await");

                        sb.AppendLine($"\t\tpublic Task{TipoCSUtil.RetornarNomeTipo(metodo.ReturnType, true)} {metodo.Name}Async({descricaoParametros})");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine($"\t\t\treturn Task.Factory.StartNew{TipoCSUtil.RetornarNomeTipo(metodo.ReturnType, true)}( ()=>");
                        sb.AppendLine("\t\t\t{");


                        sb.AppendLine($"\t\t\t\t{retorno} this.{metodo.Name}( { String.Join(",", metodo.GetParameters().Select(x => x.Name))  });");

                        sb.AppendLine("\t\t\t});");

                        sb.AppendLine("\t\t}");
 
                    }
                    sb.AppendLine($"\t\t{END_REGION}");
                    var linhasMetodos = sb.ToString().TrimEnd().ToLines();
                    var posicaoInicioRegionAutomatico = this.RetornarPosicaoInicioRegionAutomatico(linhas, configServico.CaminhoTypeScript);
                    var posicaoFimRegiionAutomatico = this.RetornarPosicaoFimRegionAutomatico(linhas, configServico.CaminhoTypeScript);

                    linhas.RemoveRange(posicaoInicioRegionAutomatico, (posicaoFimRegiionAutomatico - posicaoInicioRegionAutomatico) + 1);
                    linhas.InsertRange(posicaoInicioRegionAutomatico, linhasMetodos);

                    var novoContexto = String.Join(Environment.NewLine, linhas);
                    if (novoContexto.Trim() != conteudo.Trim())
                    {
                        ArquivoUtil.SalvarTexto(caminhoServico, novoContexto);
                    }
                }


            }
        }

    

        private int RetornarPosicaoInicioRegionAutomatico(List<string> linhas, string caminhoServico)
        {
            var linhaInicioRegionAutomatico = linhas.Where(x => x.Contains(ProjetoServicosDotNet.REGION_AUTOMATICO)).SingleOrDefault();
            if (linhaInicioRegionAutomatico != null)
            {
                return linhas.IndexOf(linhaInicioRegionAutomatico);
            }
            var linhasChaves = linhas.Where(x => x.Trim() == "{").ToList();
            if (!(linhasChaves.Count > 1))
            {
                throw new Exception($"O arquivo do servico é invalido {caminhoServico} ");
            }
            var posicaoInicio = linhas.IndexOf(linhasChaves[1]);
            linhas.InsertRange(posicaoInicio + 1, new List<string> { $"\t\t{REGION_AUTOMATICO}", $"\t\t{ProjetoServicosDotNet.END_REGION}" });
            return this.RetornarPosicaoInicioRegionAutomatico(linhas, caminhoServico);
        }

        private int RetornarPosicaoFimRegionAutomatico(List<string> linhas, string caminhoServico)
        {
            var fimInicioRegionAutomatico = linhas.Where(x => x.Contains(END_REGION)).FirstOrDefault();
            if (fimInicioRegionAutomatico == null)
            {
                throw new Exception($"O arquivo do servico é invalido {caminhoServico} ");
            }
            return linhas.IndexOf(fimInicioRegionAutomatico);
        }
        #region Métodos privados

        

        private List<Type> RetornarTodosTipo()
        {
            var assembly = AjudanteAssembly.RetornarAssembly(this.CaminhoAssembly);
            var diretorio = Path.GetDirectoryName(this.CaminhoAssembly);
            AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) =>
            {
                var nome = args.Name.Split(',').First();
                var caminho = Path.Combine(diretorio, $"{nome}.dll");
                if (File.Exists(caminho))
                {
                    return AjudanteAssembly.RetornarAssembly(caminho);
                }
                return null;

            };
            //assembly.ModuleResolve += (object sender, ResolveEventArgs e) =>
            //{

            //};

            var xx = assembly.DefinedTypes;
            return assembly.GetAccessibleTypes().ToList();
        }

        public static ConfiguracaoProjetoServico RetornarConfiguracao(string caminhoConfiguracao)
        {
            var json = File.ReadAllText(caminhoConfiguracao, UTF8Encoding.UTF8);
            return JsonUtil.Deserializar<ConfiguracaoProjetoServico>(json, true);
        }

        protected override void DispensarInerno()
        {

        }
        #endregion

    }
}