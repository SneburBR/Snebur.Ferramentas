using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using EnvDTE;
using System.IO;
using System.Reflection;
using Snebur.VisualStudio.Utilidade;
using Snebur.VisualStudio.Projeto.Dominio;

namespace Snebur.VisualStudio.Projeto
{
    public class ProjetoDominio : BaseProjeto
    {

        public string CaminhoDllAssembly { get; set; }


        public ConfiguracaoProjetoDominio ConfiguracaoDominio { get; set; }

        public Assembly Assembly { get; set; }

        public bool ProjetoSneburDominio { get; set; }

        public List<Type> TodosTipos { get; set; }

        public ProjetoDominio(Project projetoVS,
                              string caminhoProjeto,
                              string caminhoConfiguracao) :
                              base(projetoVS, caminhoProjeto, caminhoConfiguracao)
        {

            this.CaminhoDllAssembly = this.RetornarCaminhoDllAssembly();
            this.ConfiguracaoDominio = this.RetornarConfiguracaoDominio();
            this.ProjetoSneburDominio = this.ConfiguracaoDominio.Namespace == "Snebur.Dominio";
            this.TodosTipos = this.RetornarTodosTipo();
        }



        private string RetornarCaminhoDllAssembly()
        {
            var nomeArquivo = String.Format("{0}.dll", this.NomeProjeto);
            var diretorioDebug = Path.Combine(this.CaminhoProjeto, "bin\\Debug");
            var caminhoDll = Path.Combine(diretorioDebug, nomeArquivo);
            if (!File.Exists(caminhoDll))
            {
                throw new FileNotFoundException(String.Format("Não foi encontrada a DLL debug do projeto {0} - caminho : {1} ", this.NomeProjeto, caminhoDll));
            }
            return caminhoDll;
        }

        private ConfiguracaoProjetoDominio RetornarConfiguracaoDominio()
        {
            var json = File.ReadAllText(this.CaminhoConfiguracao, UTF8Encoding.UTF8);
            return JsonUtil.Deserializar<ConfiguracaoProjetoDominio>(json);
        }

        public override void Configurar()
        {
            var geradores = this.RetornarGeradoresDominio();
            foreach (var gerador in geradores)
            {
                gerador.Gerar();
            }
        }

        private List<BaseGeradorDominio> RetornarGeradoresDominio()
        {
            var geradores = new List<BaseGeradorDominio>();
            geradores.Add(new GeradorDominioAtributo(this.ConfiguracaoDominio, this.TodosTipos, this.NomeProjeto));
            geradores.Add(new GeradorDominioReflexao(this.ConfiguracaoDominio, this.TodosTipos, this.NomeProjeto));
            geradores.Add(new GeradorDominioEnum(this.ConfiguracaoDominio, this.TodosTipos, this.NomeProjeto));
            geradores.Add(new GeradorDominioClasse(this.ConfiguracaoDominio, this.TodosTipos, this.NomeProjeto));
            geradores.Add(new GeradorDominioInterface(this.ConfiguracaoDominio, this.TodosTipos, this.NomeProjeto));
            geradores.Add(new GeradorDominioConstante(this.ConfiguracaoDominio, this.TodosTipos, this.NomeProjeto));
            return geradores;
        }

        private List<Type> RetornarTodosTipo()
        {
            if (this.ProjetoSneburDominio)
            {
                return AjudanteAssembly.TiposSneburDominio;
            }
            else
            {
                var assembly = AjudanteAssembly.RetornarAssembly(this.CaminhoDllAssembly);
                return assembly.GetTypes().ToList();
            }
        }
    }
}
