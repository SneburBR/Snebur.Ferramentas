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
    public class ProjetoContextoDados : BaseProjeto
    {
        private const string REGION_CONSULTAS = "#region Consultas";

        private const string REGION = "#region";

        private const string END_REGION = "#endregion";

        private const string REGION_CONSTRUTOR_CONSULTAS_TS = "//#region Construtor Consultas";

        public ConfiguracaoProjetoContextoDados Configuracao { get; set; }

        public ProjetoContextoDados(Project projetoVS, string caminhoProjeto, string caminhoConfiguracao) :
                base(projetoVS, caminhoProjeto, caminhoConfiguracao)
        {

            this.Configuracao = this.RetornarConfiguracao();

        }

        public override void Configurar()
        {

            var linhasContextoEntity = File.ReadAllLines(this.Configuracao.CaminhoContextoDadosEntity, Encoding.UTF8).ToList();
            var posicaoInicioConsultaEntity = this.RetornarPosicaoInicioConsulta(this.Configuracao.CaminhoContextoDadosEntity, linhasContextoEntity);
            var posicaoFimConsultaEntity = 1 + (this.RetornarPosicaoFimConsulta(this.Configuracao.CaminhoContextoDadosEntity, linhasContextoEntity) - posicaoInicioConsultaEntity);

            var linhasConsultaEntity = linhasContextoEntity.GetRange(posicaoInicioConsultaEntity, posicaoFimConsultaEntity);

            this.SalvarContextoNET(linhasConsultaEntity);
            this.SalvarContextoTS(linhasConsultaEntity);

        }

        #region Métodos privados

        private void SalvarContextoNET(List<string> linhasConsultaEntity)
        {
            var linhasContextoDadosNET = File.ReadAllLines(this.Configuracao.CaminhoContextoDadosNET, Encoding.UTF8).ToList();
            var posicaoInicioConsultaNET = this.RetornarPosicaoInicioConsulta(this.Configuracao.CaminhoContextoDadosNET, linhasContextoDadosNET);
            var posicaoFimConsultaNET = (this.RetornarPosicaoFimConsulta(this.Configuracao.CaminhoContextoDadosNET, linhasContextoDadosNET) - posicaoInicioConsultaNET);

            var linhasConsultaNET = linhasContextoDadosNET.GetRange(posicaoInicioConsultaNET, posicaoFimConsultaNET);
            linhasContextoDadosNET.RemoveRange(posicaoInicioConsultaNET, posicaoFimConsultaNET + 1);
            linhasContextoDadosNET.InsertRange(posicaoInicioConsultaNET, this.RetornarLinhasConsultaNET(linhasConsultaEntity));

            var conteudoNET = String.Join("\n", linhasContextoDadosNET);

            ArquivoUtil.SalvarArquivoTexto(this.Configuracao.CaminhoContextoDadosNET, conteudoNET, true);
        }

        private void SalvarContextoTS(List<string> linhasConsultaEntity)
        {

            if (!File.Exists(this.Configuracao.CaminhoContextoDadosTS))
            {
                throw new ErroArquivoNaoEncontrado(this.Configuracao.CaminhoContextoDadosTS);
            }

            var linhasContextoDadosTS = File.ReadAllLines(this.Configuracao.CaminhoContextoDadosTS, Encoding.UTF8).ToList();
            var posicaoInicioConsultaTS = this.RetornarPosicaoInicioConsulta(this.Configuracao.CaminhoContextoDadosTS, linhasContextoDadosTS);
            var posicaoFimConsultaTS = (this.RetornarPosicaoFimConsulta(this.Configuracao.CaminhoContextoDadosTS, linhasContextoDadosTS) - posicaoInicioConsultaTS);
            var linhasConsultaTS = linhasContextoDadosTS.GetRange(posicaoInicioConsultaTS, posicaoFimConsultaTS);

            linhasContextoDadosTS.RemoveRange(posicaoInicioConsultaTS, posicaoFimConsultaTS + 1);
            linhasContextoDadosTS.InsertRange(posicaoInicioConsultaTS, this.RetornarLinhasConsultaTS(linhasConsultaEntity));

            var posicaoInicioConstrutorConsultaTS = this.RetornarPosicaoInicioConstrutorConsultaTS(linhasContextoDadosTS);
            var posicaoFimConstrutorConsultaTS = this.RetornarPosicaoFimConstrutorConsultaTS(linhasContextoDadosTS) - posicaoInicioConstrutorConsultaTS;

            linhasContextoDadosTS.RemoveRange(posicaoInicioConstrutorConsultaTS, posicaoFimConstrutorConsultaTS + 1);
            linhasContextoDadosTS.InsertRange(posicaoInicioConstrutorConsultaTS, this.RetornarLinhasConstrutorConsultaTS(linhasConsultaEntity));

            var conteudoTS = String.Join("\n", linhasContextoDadosTS);

            ArquivoUtil.SalvarArquivoTexto(this.Configuracao.CaminhoContextoDadosTS, conteudoTS, true);
        }

        private List<string> RetornarLinhasConsultaNET(List<string> linhasConsultaEntity)
        {
            var linhasNET = new List<string>();

            foreach (var linha in linhasConsultaEntity)
            {
                if (linha.Contains("DbSet"))
                {
                    var linhaNET = linha.Replace("DbSet", "IConsultaEntidade");
                    linhasNET.Add(linhaNET);
                }
                else
                {
                    linhasNET.Add(linha);
                }

            }
            //linhasNET.Add("\n");
            return linhasNET;
        }

        private List<string> RetornarLinhasConsultaTS(List<string> linhasConsultaEntity)
        {
            var linhasTS = new List<string>();
            var espacoBranco = "        ";

            foreach (var linha in linhasConsultaEntity)
            {
                if (linha.Contains("DbSet"))
                {
                    //public AtividadesUsuario: Snebur.AcessoDados.IConsultaEntidade<PhotosApp.Entidades.AtividadeUsuario>;
                    //public DbSet<AtividadeUsuario> AtividadesUsuario { get; set; }

                    var nomeConsulta = linha.Substring(linha.IndexOf(">") + 1, linha.IndexOf("{") - linha.IndexOf(">") - 1).Trim();
                    var nomeTipoEntidade = linha.Substring(linha.IndexOf("<") + 1, linha.IndexOf(">") - linha.IndexOf("<") - 1).Trim();

                    var linhaTS = String.Format("{0}public {1} : Snebur.AcessoDados.IConsultaEntidade<{2}.{3}>; ", espacoBranco, nomeConsulta, this.Configuracao.NamespaceEntidades, nomeTipoEntidade);
                    linhasTS.Add(linhaTS);
                }
                else
                {

                    if (linha.Trim().StartsWith(REGION))
                    {
                        var linhaComentada = linha.Replace(REGION, String.Format("//{0}", REGION));
                        linhasTS.Add(linhaComentada);
                    }
                    else if (linha.Trim().StartsWith(END_REGION))
                    {
                        var linhaComentada = linha.Replace(END_REGION, String.Format("//{0}", END_REGION));
                        linhasTS.Add(linhaComentada);
                    }
                    else
                    {
                        linhasTS.Add(linha);
                    }
                }

            }
            //linhasTS.Add("\n");
            return linhasTS;
        }

        private List<string> RetornarLinhasConstrutorConsultaTS(List<string> linhasConsultaEntity)
        {

           

            var linhasTS = new List<string>();

            var espacoBranco = "            ";
            linhasTS.Add(String.Format("{0}{1}", espacoBranco, REGION_CONSTRUTOR_CONSULTAS_TS));
            linhasTS.Add("\n");

            foreach (var linha in linhasConsultaEntity)
            {
                if (linha.Contains("DbSet"))
                {
                    // this.AtividadesUsuario = new Snebur.AcessoDados.ConstrutorConsultaEntidade<PhotosApp.Entidades.AtividadeUsuario>(this, PhotosApp.Entidades.AtividadeUsuario.GetType() as r.TipoBaseDominio);
                    //public DbSet<AtividadeUsuario> AtividadesUsuario { get; set; }

                    var nomeConsulta = linha.Substring(linha.IndexOf(">") + 1, linha.IndexOf("{") - linha.IndexOf(">") - 1).Trim();
                    var nomeTipoEntidade = linha.Substring(linha.IndexOf("<") + 1, linha.IndexOf(">") - linha.IndexOf("<") - 1).Trim();

                    var linhaTS = String.Format("{0}this.{1} = new Snebur.AcessoDados.ConstrutorConsultaEntidade<{2}.{3}>(this, {2}.{3}.GetType() as r.TipoEntidade); ", espacoBranco, nomeConsulta, this.Configuracao.NamespaceEntidades, nomeTipoEntidade);
                    linhasTS.Add(linhaTS);
                }
                else
                {
                    //não faz nada
                    //não adicionada
                }

            }
            linhasTS.Add("\n");
            linhasTS.Add(String.Format("{0}//#endregion", espacoBranco));
            return linhasTS;
        }

        private ConfiguracaoProjetoContextoDados RetornarConfiguracao()
        {
            var json = File.ReadAllText(this.CaminhoConfiguracao, UTF8Encoding.UTF8);
            return JsonUtil.Deserializar<ConfiguracaoProjetoContextoDados>(json);
        }

        private int RetornarPosicaoInicioConsulta(string caminhoArquivo, List<string> linhas)
        {
            var linhasEncontradas = linhas.Where(x => x.Trim().Contains(REGION_CONSULTAS)).ToList();
            if (linhasEncontradas.Count == 0)
            {
                throw new Erro(string.Format("Não foi encontrado {0} em {1}", REGION_CONSULTAS, caminhoArquivo));
            }

            if (linhasEncontradas.Count > 1)
            {
                throw new Erro(string.Format("Mais de um linha encontrado com {0} em {1}", REGION_CONSULTAS, caminhoArquivo));
            }

            for (var i = 0; i < linhas.Count; i++)
            {
                var linha = linhas[i];
                if (linha.Trim().Contains(REGION_CONSULTAS))
                {
                    return i;
                }
            }

            throw new Erro("A posicao da linha consulta não foi encontrado em " + caminhoArquivo);

        }

        private int RetornarPosicaoFimConsulta(string caminhoArquivo, List<string> linhas)
        {

            bool abriuConsulta = false;
            int contador = 0;

            for (var i = 0; i < linhas.Count; i++)
            {
                var linha = linhas[i].Trim();
                if (linha.Trim().Contains(REGION_CONSULTAS))
                {
                    abriuConsulta = true;
                }

                if (abriuConsulta)
                {
                    if (linha.Contains(REGION))
                    {
                        contador += 1;
                    }
                    if (linha.Contains(END_REGION))
                    {
                        contador -= 1;
                    }


                    if (contador == 0)
                    {
                        return i;
                    }
                }
            }

            throw new Erro("O fechar consulta não foi encontrado em " + caminhoArquivo);

        }

        private int RetornarPosicaoInicioConstrutorConsultaTS(List<string> linhas)
        {

            var linhasEncontradas = linhas.Where(x => x.Trim().Contains(REGION_CONSULTAS)).ToList();
            if (linhasEncontradas.Count == 0)
            {
                throw new Erro(string.Format("Não foi encontrado {0} em {1}", REGION_CONSTRUTOR_CONSULTAS_TS, this.Configuracao.CaminhoContextoDadosTS));
            }

            if (linhasEncontradas.Count > 1)
            {
                throw new Erro(string.Format("Mais de um linha encontrado com {0} em {1}", REGION_CONSTRUTOR_CONSULTAS_TS, this.Configuracao.CaminhoContextoDadosTS));
            }

            for (var i = 0; i < linhas.Count; i++)
            {
                var linha = linhas[i];
                if (linha.Trim().Contains(REGION_CONSTRUTOR_CONSULTAS_TS))
                {
                    return i;
                }
            }

            throw new Erro("A posicao da linha consulta não foi encontrado em " + this.Configuracao.CaminhoContextoDadosTS);
        }

        private int RetornarPosicaoFimConstrutorConsultaTS(List<string> linhas)
        {

            bool abriuConsulta = false;
            int contador = 0;

            for (var i = 0; i < linhas.Count; i++)
            {
                var linha = linhas[i].Trim();
                if (linha.Trim().Contains(REGION_CONSTRUTOR_CONSULTAS_TS))
                {
                    abriuConsulta = true;
                }

                if (abriuConsulta)
                {
                    if (linha.Contains(REGION))
                    {
                        contador += 1;
                    }
                    if (linha.Contains(END_REGION))
                    {
                        contador -= 1;
                    }


                    if (contador == 0)
                    {
                        return i;
                    }
                }
            }

            throw new Erro("O fechar consulta não foi encontrado em " + this.Configuracao.CaminhoContextoDadosTS);

        }
        #endregion


    }
}
