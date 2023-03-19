using Snebur.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snebur.VisualStudio
{
    public class ProjetoTSDepdencia : IDisposable
    {
        private const string NOME_REPOSITORIO = "src";
        private string NomeProjeto { get; }
        public string CaminhoDepedencia { get; }
        public string CaminhoProjeto { get; }
        public string CaminhoProjetoApresentacao { get; }
        public HashSet<string> Arquivos { get; private set; }
        public List<ArquivoTypeScript> ArquiviosTypescript { get; private set; }
        public string CaminhoConfiguracao { get; }
        public ConfiguracaoProjetoTypeScript Configuracao { get; }
        public string CaminhoRepositoriosArquivosTypescript { get; }

        public ProjetoTSDepdencia(string nomeProjeto, string caminhoDepedencia)
        {
            this.NomeProjeto = nomeProjeto;
            this.CaminhoDepedencia = caminhoDepedencia;
            this.CaminhoRepositoriosArquivosTypescript = this.RetornarCaminhoRepositoriosArquivosTypescript();
            this.CaminhoProjeto = Path.GetDirectoryName(this.CaminhoRepositoriosArquivosTypescript);
            this.CaminhoProjetoApresentacao = Path.Combine(this.CaminhoProjeto, "Apresentacao");
            this.CaminhoConfiguracao = this.RetornarCaminhoConfiguracaoTypescript();



            this.Configuracao = ProjetoTypeScriptUtil.RetornarConfiguracaoProjetoTypeScript(this.CaminhoConfiguracao);
            this.Arquivos = this.RetornarArquivos();
            this.ArquiviosTypescript = this.RetornarCaminhosArquivoTypescript();
        }

        private HashSet<string> RetornarArquivos()
        {
            var arquivos = Directory.GetFiles(this.CaminhoRepositoriosArquivosTypescript, "*.ts", SearchOption.AllDirectories).ToHashSet();
            if (Directory.Exists(this.CaminhoProjetoApresentacao))
            {
                var arquivosApresentacao = Directory.GetFiles(this.CaminhoProjetoApresentacao, "*.ts", SearchOption.AllDirectories).ToHashSet();
                arquivos.AddRange(arquivosApresentacao);
            }
            return arquivos;
        }

        public List<string> RetornarCaminhosClassesBase()
        {
            return this.ArquiviosTypescript.Select(x => x.CaminhoTipo).ToList();
        }

        private List<ArquivoTypeScript> RetornarCaminhosArquivoTypescript()
        {
            return TipoArquivoTypeScriptUtil.RetornarArquivosTypeScript(this.Configuracao,
                                                                        this.CaminhoProjeto, 
                                                                        this.Arquivos, null).
                                                                        OfType<ArquivoTypeScript>().
                                                                        ToList();
        }

        private string RetornarCaminhoConfiguracaoTypescript()
        {
            var caminho = Path.Combine(this.CaminhoProjeto, ConstantesProjeto.CONFIGURACAO_TYPESCRIPT);
            if (!File.Exists(caminho))
            {
                throw new Exception($"Não foi encontrado o caminho to arquivo tsconfig.json para a dependência ${Path.GetFileName(this.CaminhoDepedencia)}");
            }
            return caminho;
        }

        private string RetornarCaminhoRepositoriosArquivosTypescript()
        {
            var di = new DirectoryInfo(this.CaminhoDepedencia);

            while (!Directory.Exists(Path.Combine(di.FullName, NOME_REPOSITORIO)))
            {
                if (di.Parent == null)
                {
                    throw new Exception($"Não foi encontrado o repositório dos arquivos typescript da dependência {this.CaminhoDepedencia}");
                }
                di = di.Parent;
            }
            var caminho = Path.Combine(di.FullName, NOME_REPOSITORIO);
            //var caminho = di.FullName;
            if (!Directory.Exists(caminho))
            {
                throw new Exception($"Não foi encontrado o repositório dos arquivos typescript da dependência {this.CaminhoDepedencia} \n{caminho}");
            }
            return caminho;

        }

        public void Dispose()
        {
            this.Arquivos.Clear();
            this.ArquiviosTypescript.Clear();
            this.Arquivos = null;
            this.ArquiviosTypescript = null;
        }
    }
}