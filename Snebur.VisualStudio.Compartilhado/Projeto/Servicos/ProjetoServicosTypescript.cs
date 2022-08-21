using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public class ProjetoServicosTypescript : BaseProjeto<ConfiguracaoProjetoServico>
    {
        private const string REGION_AUTOMATICO = "//#region Automatico";
        private const string END_REGION = "//#endregion";

        public string CaminhoDllAssembly { get; }
        public List<Type> TodosTipo { get; }

        public ProjetoServicosTypescript(ConfiguracaoProjetoServico configuracaoProjeto,
                                        string caminhoProjeto,
                                        string caminhoConfiguracao) :
                                        base(configuracaoProjeto, 
                                             caminhoProjeto, 
                                             caminhoConfiguracao)
        {
            this.CaminhoDllAssembly = AjudanteAssembly.RetornarCaminhoAssembly(configuracaoProjeto);
            this.TodosTipo = this.RetornarTodosTipo();
        }

        protected override void AtualizarInterno()
        {
            foreach (var configServico in this.ConfiguracaoProjeto.Servicos)
            {
                var tipoInterface = this.TodosTipo.Where(x => x.IsInterface && x.Name == configServico.NomeInterface).SingleOrDefault();
                if (tipoInterface == null)
                {
                    throw new Exception($"O interface {tipoInterface} não foi encontrada em {this.CaminhoDllAssembly} ");
                }
                if (!String.IsNullOrWhiteSpace(configServico.CaminhoTypeScript))
                {
                    var caminhoServico = Path.GetFullPath(Path.Combine(this.CaminhoProjeto, configServico.CaminhoTypeScript));
                    if (!File.Exists(caminhoServico))
                    {
                        throw new FileNotFoundException(configServico.CaminhoTypeScript);
                    }
                    var conteudo = ArquivoUtil.LerTexto(caminhoServico).Trim();
                    var linhas = conteudo.ToLines();

                    var sb = new StringBuilder();
                    sb.AppendLine($"\t\t{REGION_AUTOMATICO}");

                    var metodos = tipoInterface.GetMethods();
                    foreach (var metodo in metodos)
                    {
                        var m = metodo;

                        var parametros = metodo.GetParameters().Select(x => $"{x.Name} : {TipoUtil.RetornarCaminhoTipoTS(x.ParameterType)}").ToList();
                        var descricaoParametros = String.Join(", ", parametros);

                        //sb.AppendLine("");
                        //sb.AppendLine($"\t\tpublic {metodo.Name}({descricaoParametros}) : {TipoUtil.RetornarCaminhoTipoTS(metodo.ReturnType)}");
                        //sb.AppendLine("\t\t{");

                        //var linha = $"this.ChamarServico<{TipoUtil.RetornarCaminhoTipoTS(metodo.ReturnType)}>(\"{metodo.Name}\", arguments);";
                        //if (metodo.ReturnType == typeof(void))
                        //{
                        //    sb.AppendLine($"\t\t\t{linha}");
                        //}
                        //else
                        //{
                        //    sb.AppendLine($"\t\t\treturn {linha}");
                        //}
                        //sb.AppendLine("\t\t}");

                        var adicionalVirgula = (parametros.Count > 0) ? ", " : "";
                        var parametrosAsync = String.Join(", ", metodo.GetParameters().Select(x => x.Name));


                        sb.AppendLine("\t\t//async");
                        sb.AppendLine($"\t\tpublic {metodo.Name}Async({descricaoParametros}) : Promise<{TipoUtil.RetornarCaminhoTipoTS(metodo.ReturnType)}>");
                        sb.AppendLine("\t\t{");

                        sb.AppendLine($"\t\t\treturn new Promise<{TipoUtil.RetornarCaminhoTipoTS(metodo.ReturnType)}>(resolver =>");
                        sb.AppendLine("\t\t\t{");

                        sb.AppendLine($"\t\t\t\tthis.__{metodo.Name}InternoAsync({parametrosAsync} {adicionalVirgula} function(resultado: {TipoUtil.RetornarCaminhoTipoTS(metodo.ReturnType)})");
                        sb.AppendLine("\t\t\t\t{");
                        sb.AppendLine("\t\t\t\t\tresolver(resultado);");
                        sb.AppendLine("\t\t\t\t});");
                        sb.AppendLine("\t\t\t});");
                        sb.AppendLine("\t\t}");

                        //async interno";
                        sb.AppendLine($"\t\tprivate __{metodo.Name}InternoAsync({descricaoParametros}{adicionalVirgula}callback: CallbackResultado<{TipoUtil.RetornarCaminhoTipoTS(metodo.ReturnType)}>): void");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine($"\t\t\tthis.ChamarServicoAsync(\"{metodo.Name}Async\", arguments);");
                        sb.AppendLine("\t\t}");

                    }
                    sb.AppendLine($"\t\t{END_REGION}");
                    var linhasMetodos = sb.ToString().Trim().ToLines();
                    var posicaoInicioRegionAutomatico = this.RetornarPosicaoInicioRegionAutomatico(linhas, configServico.CaminhoTypeScript);
                    var posicaoFimRegiionAutomatico = posicaoInicioRegionAutomatico + this.RetornarPosicaoFimRegionAutomatico(linhas.Skip(posicaoInicioRegionAutomatico).ToList(), configServico.CaminhoTypeScript);

                    //if(posicaoInicioRegionAutomatico > posicaoFimRegiionAutomatico)
                    //{
                    //    throw new Exception($"A region  {REGION_AUTOMATICO} não serviço '{configServico.NomeInterface}' " +
                    //                        $"{configServico.CaminhoTypeScript} está em posição inversa")
                    //}
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
            var linhaInicioRegionAutomatico = linhas.Where(x => x.Contains(REGION_AUTOMATICO)).SingleOrDefault();
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
            linhas.InsertRange(posicaoInicio + 1, new List<string> { $"\t\t{REGION_AUTOMATICO}", $"\t\t{ProjetoServicosTypescript.END_REGION}" });
            return this.RetornarPosicaoInicioRegionAutomatico(linhas, caminhoServico);
        }

        private int RetornarPosicaoFimRegionAutomatico(List<string> linhas, string caminhoServico)
        {
            var fimInicioRegionAutomatico = linhas.Where(x => x.Contains(END_REGION)).FirstOrDefault();
            if (fimInicioRegionAutomatico == null)
            {
                throw new Exception($"O arquivo do serviço é invalido {caminhoServico} ");
            }
            return linhas.IndexOf(fimInicioRegionAutomatico);
        }

        #region Métodos privados

        private List<Type> RetornarTodosTipo()
        {
            var assembly = AjudanteAssembly.RetornarAssembly(this.CaminhoDllAssembly);
            var diretorio = Path.GetDirectoryName(this.CaminhoDllAssembly);
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

            return assembly.GetAccessibleTypes().ToList();
        }

        public static ConfiguracaoProjetoServico RetornarConfiguracao(string caminhoConfiguracao)
        {
            var json = File.ReadAllText(caminhoConfiguracao, UTF8Encoding.UTF8);
            return JsonUtil.Deserializar<ConfiguracaoProjetoServico>(json);
        }

        protected override void DispensarInerno()
        {

        }
        #endregion

    }
}