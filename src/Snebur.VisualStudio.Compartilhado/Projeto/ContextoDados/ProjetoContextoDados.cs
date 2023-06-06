using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Snebur.VisualStudio
{
    public class ProjetoContextoDados : BaseProjeto<ConfiguracaoProjetoContextoDados>
    {
        private const string REGION_CONSULTAS = "#region Consultas";

        private const string REGION = "#region";

        private const string END_REGION = "#endregion";

        private const string REGION_CONSTRUTOR_CONSULTAS_TS = "//#region Construtor Consultas";


        public ProjetoContextoDados(ProjetoViewModel projetoVM,
                                    ConfiguracaoProjetoContextoDados configuracaoProjeto,
                                    FileInfo arquivoProjeto,
                                    string caminhoConfiguracao) :
                                    base(projetoVM, configuracaoProjeto, arquivoProjeto, caminhoConfiguracao)
        {

        }

        protected override void AtualizarInterno()
        {
            var caminhoAbsolutoContextoDadosEntity = CaminhoUtil.RetornarCaminhosAbsoluto(this.ConfiguracaoProjeto.CaminhoContextoDadosEntity, this.CaminhoProjeto);
            var linhasContextoEntity = File.ReadAllLines(caminhoAbsolutoContextoDadosEntity, Encoding.UTF8).ToList();
            var posicaoInicioConsultaEntity = this.RetornarPosicaoInicioConsulta(caminhoAbsolutoContextoDadosEntity, linhasContextoEntity);
            var posicaoFimConsultaEntity = 1 + (this.RetornarPosicaoFimConsulta(caminhoAbsolutoContextoDadosEntity, linhasContextoEntity) - posicaoInicioConsultaEntity);

            var linhasConsultaEntity = linhasContextoEntity.GetRange(posicaoInicioConsultaEntity, posicaoFimConsultaEntity);

            this.SalvarContextoNET(this.ConfiguracaoProjeto.CaminhoContextoDadosNETCliente, linhasConsultaEntity);
            this.SalvarContextoNET(this.ConfiguracaoProjeto.CaminhoContextoDadosNETServidor, linhasConsultaEntity);
            this.SalvarContextoNET(this.ConfiguracaoProjeto.CaminhoContextoDadosNETInterface, linhasConsultaEntity, true);

            this.SalvarContextoTS(linhasConsultaEntity);
        }

        #region Métodos privados

        private void SalvarContextoNET(string caminhoArquivo, List<string> linhasConsultaEntity, bool isIntarface = false)
        {
            if (String.IsNullOrWhiteSpace(caminhoArquivo))
            {
                return;
            }

            var caminhoAbsolutoContextoDadosNET = CaminhoUtil.RetornarCaminhosAbsoluto(caminhoArquivo, this.CaminhoProjeto);

            var linhasContextoDadosNET = File.ReadAllLines(caminhoAbsolutoContextoDadosNET, Encoding.UTF8).ToList();
            var posicaoInicioConsultaNET = this.RetornarPosicaoInicioConsulta(caminhoAbsolutoContextoDadosNET, linhasContextoDadosNET);
            var posicaoFimConsultaNET = (this.RetornarPosicaoFimConsulta(caminhoAbsolutoContextoDadosNET, linhasContextoDadosNET) - posicaoInicioConsultaNET);

            var linhasConsultaNET = linhasContextoDadosNET.GetRange(posicaoInicioConsultaNET, posicaoFimConsultaNET);
            linhasContextoDadosNET.RemoveRange(posicaoInicioConsultaNET, posicaoFimConsultaNET + 1);
            linhasContextoDadosNET.InsertRange(posicaoInicioConsultaNET, this.RetornarLinhasConsultaNET(linhasConsultaEntity));

            if (isIntarface)
            {
                for (var i = 0; i < linhasContextoDadosNET.Count; i++)
                {
                    var linha = linhasContextoDadosNET[i];
                    if (linha.TrimStart().StartsWith("public") && linha.Contains("IConsultaEntidade"))
                    {
                        linha = linha.Replace(" public ", " ");
                        linhasContextoDadosNET[i] = linha;
                    }
                }
            }

            var conteudoNET = String.Join("\n", linhasContextoDadosNET);
            ArquivoUtil.SalvarArquivoTexto(caminhoAbsolutoContextoDadosNET, conteudoNET);
        }

        //private void SalvarContextoNETCliente(List<string> linhasConsultaEntity)
        //{
        //    //var caminhoAbsolutoContextoDadosNETCliente = CaminhoUtil.RetornarCaminhosAbsoluto(this.ConfiguracaoProjeto.CaminhoContextoDadosNETCliente, this.CaminhoProjeto);

        //    //if (File.Exists(caminhoAbsolutoContextoDadosNETCliente))
        //    //{
        //    //    var linhasContextoDadosNET = File.ReadAllLines(caminhoAbsolutoContextoDadosNETCliente, Encoding.UTF8).ToList();
        //    //    var posicaoInicioConsultaNET = this.RetornarPosicaoInicioConsulta(caminhoAbsolutoContextoDadosNETCliente, linhasContextoDadosNET);
        //    //    var posicaoFimConsultaNET = (this.RetornarPosicaoFimConsulta(caminhoAbsolutoContextoDadosNETCliente, linhasContextoDadosNET) - posicaoInicioConsultaNET);

        //    //    var linhasConsultaNET = linhasContextoDadosNET.GetRange(posicaoInicioConsultaNET, posicaoFimConsultaNET);
        //    //    linhasContextoDadosNET.RemoveRange(posicaoInicioConsultaNET, posicaoFimConsultaNET + 1);
        //    //    linhasContextoDadosNET.InsertRange(posicaoInicioConsultaNET, this.RetornarLinhasConsultaNET(linhasConsultaEntity));

        //    //    var conteudoNET = String.Join("\n", linhasContextoDadosNET);

        //    //    ArquivoUtil.SalvarArquivoTexto(caminhoAbsolutoContextoDadosNETCliente, conteudoNET, true);
        //    //}

        //}

        private void SalvarContextoTS(List<string> linhasConsultaEntity)
        {
            var caminhoAbsolutoContextoDadosTS = CaminhoUtil.RetornarCaminhosAbsoluto(this.ConfiguracaoProjeto.CaminhoContextoDadosTS, this.CaminhoProjeto);

            if (!File.Exists(caminhoAbsolutoContextoDadosTS))
            {
                throw new ErroArquivoNaoEncontrado(caminhoAbsolutoContextoDadosTS);
            }

            var linhasContextoDadosTS = File.ReadAllLines(caminhoAbsolutoContextoDadosTS, Encoding.UTF8).ToList();
            var posicaoInicioConsultaTS = this.RetornarPosicaoInicioConsulta(caminhoAbsolutoContextoDadosTS, linhasContextoDadosTS);
            var posicaoFimConsultaTS = (this.RetornarPosicaoFimConsulta(caminhoAbsolutoContextoDadosTS, linhasContextoDadosTS) - posicaoInicioConsultaTS);
            var linhasConsultaTS = linhasContextoDadosTS.GetRange(posicaoInicioConsultaTS, posicaoFimConsultaTS);

            linhasContextoDadosTS.RemoveRange(posicaoInicioConsultaTS, posicaoFimConsultaTS + 1);
            linhasContextoDadosTS.InsertRange(posicaoInicioConsultaTS, this.RetornarLinhasConsultaTS(linhasConsultaEntity));

            var posicaoInicioConstrutorConsultaTS = this.RetornarPosicaoInicioConstrutorConsultaTS(linhasContextoDadosTS);
            var posicaoFimConstrutorConsultaTS = this.RetornarPosicaoFimConstrutorConsultaTS(linhasContextoDadosTS) - posicaoInicioConstrutorConsultaTS;

            linhasContextoDadosTS.RemoveRange(posicaoInicioConstrutorConsultaTS, posicaoFimConstrutorConsultaTS + 1);
            linhasContextoDadosTS.InsertRange(posicaoInicioConstrutorConsultaTS, this.RetornarLinhasConstrutorConsultaTS(linhasConsultaEntity));

            var conteudoTS = String.Join("\n", linhasContextoDadosTS);

            ArquivoUtil.SalvarArquivoTexto(caminhoAbsolutoContextoDadosTS, conteudoTS);
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


                    var comentar = linha.TrimStart().StartsWith("//") ? "//" : "";
                    var nomeConsulta = linha.Substring(linha.IndexOf(">") + 1, linha.IndexOf("{") - linha.IndexOf(">") - 1).Trim();
                    var nomeTipoEntidade = linha.Substring(linha.IndexOf("<") + 1, linha.IndexOf(">") - linha.IndexOf("<") - 1).Trim();
                    var linhaTS = $"{espacoBranco}{comentar}public readonly {nomeConsulta} : Snebur.AcessoDados.IConsultaEntidade<{this.ConfiguracaoProjeto.NamespaceEntidades}.{nomeTipoEntidade}>;";
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
                    var comentar = linha.TrimStart().StartsWith("//") ? "//" : "";
                    // this.AtividadesUsuario = new Snebur.AcessoDados.ConstrutorConsultaEntidade<PhotosApp.Entidades.AtividadeUsuario>(this, PhotosApp.Entidades.AtividadeUsuario.GetType() as r.TipoBaseDominio);
                    //public DbSet<AtividadeUsuario> AtividadesUsuario { get; set; }

                    var nomeConsulta = linha.Substring(linha.IndexOf(">") + 1, linha.IndexOf("{") - linha.IndexOf(">") - 1).Trim();
                    var nomeTipoEntidade = linha.Substring(linha.IndexOf("<") + 1, linha.IndexOf(">") - linha.IndexOf("<") - 1).Trim();

                    var linhaTS = $"{espacoBranco}{comentar}this.{nomeConsulta} = new Snebur.AcessoDados.ConstrutorConsultaEntidade<{this.ConfiguracaoProjeto.NamespaceEntidades}.{nomeTipoEntidade}>(this, {this.ConfiguracaoProjeto.NamespaceEntidades}.{nomeTipoEntidade}.GetType() as r.TipoEntidade); ";
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

        public static ConfiguracaoProjetoContextoDados RetornarConfiguracao(string caminhoConfiguracao)
        {
            var json = File.ReadAllText(caminhoConfiguracao, UTF8Encoding.UTF8);
            return JsonUtil.Deserializar<ConfiguracaoProjetoContextoDados>(json, EnumTipoSerializacao.Javascript);
        }

        private int RetornarPosicaoInicioConsulta(string caminhoArquivo, List<string> linhas)
        {
            var linhasEncontradas = linhas.Where(x => x.Trim().Contains(REGION_CONSULTAS)).ToList();
            if (linhasEncontradas.Count == 0)
            {
                throw new Erro(String.Format("Não foi encontrado {0} em {1}", REGION_CONSULTAS, caminhoArquivo));
            }

            if (linhasEncontradas.Count > 1)
            {
                throw new Erro(String.Format("Mais de um linha encontrado com {0} em {1}", REGION_CONSULTAS, caminhoArquivo));
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
                throw new Erro(String.Format("Não foi encontrado {0} em {1}", REGION_CONSTRUTOR_CONSULTAS_TS, this.ConfiguracaoProjeto.CaminhoContextoDadosTS));
            }

            if (linhasEncontradas.Count > 1)
            {
                throw new Erro(String.Format("Mais de um linha encontrado com {0} em {1}", REGION_CONSTRUTOR_CONSULTAS_TS, this.ConfiguracaoProjeto.CaminhoContextoDadosTS));
            }

            for (var i = 0; i < linhas.Count; i++)
            {
                var linha = linhas[i];
                if (linha.Trim().Contains(REGION_CONSTRUTOR_CONSULTAS_TS))
                {
                    return i;
                }
            }

            throw new Erro("A posicao da linha consulta não foi encontrado em " + this.ConfiguracaoProjeto.CaminhoContextoDadosTS);
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

            throw new Erro("O fechar consulta não foi encontrado em " + this.ConfiguracaoProjeto.CaminhoContextoDadosTS);

        }

        protected override void DispensarInerno()
        {

        }
        #endregion


    }
}
