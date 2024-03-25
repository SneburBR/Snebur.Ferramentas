using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Snebur.VisualStudio
{
    public partial class ProjetoDominio : BaseProjeto<ConfiguracaoProjetoDominio>
    {
        public List<Type> TodosTipos { get; }

      

        public ProjetoDominio(ProjetoViewModel projetoVM, 
                              ConfiguracaoProjetoDominio configuracaoProjeto,
                              FileInfo arquivoProjeto,
                              string caminhoConfiguracao) :
                              base(projetoVM, configuracaoProjeto, arquivoProjeto, caminhoConfiguracao)
        {

            //ProjetoUtil.CompilarProjeto(dte, projetoVS);

            this.AtualizarAssemblyDominiosDependentes();
            this.IsProjetoSneburDominio = this.ConfiguracaoProjeto.Namespace == "Snebur.Dominio";
            this.TodosTipos = this.RetornarTodosTipo();
        }

        public static ConfiguracaoProjetoDominio RetornarConfiguracaoDominio(string caminhoConfiguracao)
        {
            var json = File.ReadAllText(caminhoConfiguracao, UTF8Encoding.UTF8);
            var teste = TextoUtil.RemoverAcentos(json);
            return JsonUtil.Deserializar<ConfiguracaoProjetoDominio>(json, EnumTipoSerializacao.Javascript);
        }

        protected override void AtualizarInterno()
        {
            this.AtualizarAtribuirPropriedades();
            if (this.IsExisteDll)
            {
                var geradores = this.RetornarGeradoresDominio();
                foreach (var gerador in geradores)
                {
                    try
                    {
                        gerador.Gerar();
                    }
                    catch(Exception ex)
                    {
                        LogVSUtil.LogErro($"Falha no geradora {gerador.GetType().Name}", ex);
                    }
                    
                }
            }
        }

        public override void InscrementarVersao()
        {
            if (!this.ConfiguracaoProjeto.IsVersaoManual)
            {
                base.InscrementarVersao();
            }
        }

        private List<BaseGeradorDominio> RetornarGeradoresDominio()
        {
            var geradores = new List<BaseGeradorDominio>();
            geradores.Add(new GeradorDominioAtributo(this.ConfiguracaoProjeto, this.CaminhoProjeto, this.TodosTipos, this.NomeProjeto));
            geradores.Add(new GeradorDominioReflexao(this.ConfiguracaoProjeto, this.CaminhoProjeto, this.TodosTipos, this.NomeProjeto));
            geradores.Add(new GeradorDominioEnum(this.ConfiguracaoProjeto, this.CaminhoProjeto, this.TodosTipos, this.NomeProjeto));
            geradores.Add(new GeradorDominioClasse(this.ConfiguracaoProjeto, this.CaminhoProjeto, this.TodosTipos, this.NomeProjeto));
            geradores.Add(new GeradorDominioInterface(this.ConfiguracaoProjeto, this.CaminhoProjeto, this.TodosTipos, this.NomeProjeto));
            geradores.Add(new GeradorDominioConstante(this.ConfiguracaoProjeto, this.CaminhoProjeto, this.TodosTipos, this.NomeProjeto));
            return geradores;
        }

        private void AtualizarAssemblyDominiosDependentes()
        {
            foreach (var dominioDependente in this.ConfiguracaoProjeto.DominiosDepentendes)
            {
                var caminhoAbsoluto = Path.GetFullPath(Path.Combine(this.CaminhoProjeto, dominioDependente.Caminho));
                if (!File.Exists(caminhoAbsoluto))
                {
                    throw new FileNotFoundException(caminhoAbsoluto);
                }

                if (!AjudanteAssembly.AssemblyCaminhos.ContainsKey(dominioDependente.Nome))
                {
                    AjudanteAssembly.AssemblyCaminhos.Add(dominioDependente.Nome, new string[] { caminhoAbsoluto });
                }
            }
        }

       
    }
}
