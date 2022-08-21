using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using EnvDTE;
using System.IO;
using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio.Projeto
{
    public class ProjetoSass : BaseProjeto
    {
        private const string INICIO = "//#region  _SCSS";

        private const string FIM = "//#endregion SCSS";

        public ConfiguracaoProjetoSass Configuracao { get; set; }

        private string CaminhoArquivoScss { get; set; }

        public ProjetoSass(Project projetoVS, string caminhoProjeto, string caminhoConfiguracao) :
                base(projetoVS, caminhoProjeto, caminhoConfiguracao)
        {

            this.Configuracao = this.RetornarConfiguracao();
            this.CaminhoArquivoScss = Path.Combine(this.CaminhoProjeto, this.Configuracao.inputFile);

        }

        public override void Configurar()
        {
            var arquivosScss = this.RetornaArquivosScss();

            var linhas = File.ReadAllLines(this.CaminhoArquivoScss, Encoding.UTF8).ToList();
            var inicio = this.RetornarPosicaoInicio(linhas);
            var fim = this.RetornarPosicaoFim(linhas);

            var linhasArquivo = arquivosScss.Select(x => String.Format("@import \"{0}\";", x.Replace("\\", "\\\\"))).ToList();

            linhas.RemoveRange(inicio + 1, fim - inicio - 1);
            linhas.InsertRange(inicio + 1, linhasArquivo);

            var conteudo = String.Join("\n", linhas);
            ArquivoUtil.SalvarArquivoTexto(this.CaminhoArquivoScss, conteudo, true);
        }

        private List<string> RetornaArquivosScss()
        {
            var arquivos = Directory.EnumerateFiles(this.CaminhoProjeto, "*.scss", SearchOption.AllDirectories).ToList();
            arquivos = arquivos.Where(x =>
                                     {
                                         var nomeArquivo = Path.GetFileName(x);
                                         return nomeArquivo.StartsWith("_") || nomeArquivo.EndsWith(".shtml.scss");
                                     }
            ).ToList();
            return arquivos;
        }

        private ConfiguracaoProjetoSass RetornarConfiguracao()
        {
            var json = File.ReadAllText(this.CaminhoConfiguracao, UTF8Encoding.UTF8);
            var configuracoes = JsonUtil.Deserializar<List<ConfiguracaoProjetoSass>>(json);
            if (configuracoes.Count == 0)
            {
                throw new ErroNaoDefinido("O configuracao não foi definida");
            }
            if (configuracoes.Count > 1)
            {
                throw new ErroNaoSuportado("Não é suportado mais de um arquivo de configuracao por projeto");
            }
            return configuracoes.Single();
        }

        private int RetornarPosicaoInicio(List<string> linhas)
        {
            for (var i = 0; i < linhas.Count; i++)
            {
                var linha = linhas[i];
                if (linha.Trim().Contains(INICIO))
                {
                    return i;
                }
            }
            throw new Erro(String.Format("A posicao do inicio '{0}' não foi encontrado em {1}", INICIO, this.CaminhoArquivoScss));
        }

        private int RetornarPosicaoFim(List<string> linhas)
        {
            for (var i = 0; i < linhas.Count; i++)
            {
                var linha = linhas[i];
                if (linha.Trim().Contains(FIM))
                {
                    return i;
                }
            }
            throw new Erro(String.Format("A posicao do fim '{0}' não foi encontrado em {1}", FIM, this.CaminhoArquivoScss));
        }
    }
}
