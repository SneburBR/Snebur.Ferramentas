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
        public string CaminhoAssembly { get; }

        public bool IsProjetoSneburDominio { get; set; }

        public List<Type> TodosTipos { get; }

        public bool IsExisteDll
        {
            get
            {
                return File.Exists(this.CaminhoAssembly);
            }
        }

        public ProjetoDominio(ConfiguracaoProjetoDominio configuracaoProjeto,
                              string caminhoProjeto,
                              string caminhoConfiguracao) :
                              base(configuracaoProjeto, caminhoProjeto, caminhoConfiguracao)
        {

            //ProjetoUtil.CompilarProjeto(dte, projetoVS);

            this.CaminhoAssembly = AjudanteAssembly.RetornarCaminhoAssembly(configuracaoProjeto);
            this.AtualizarAssemblyDominiosDependentes();
            this.IsProjetoSneburDominio = this.ConfiguracaoProjeto.Namespace == "Snebur.Dominio";
            this.TodosTipos = this.RetornarTodosTipo();
        }

        internal static ConfiguracaoProjetoDominio RetornarConfiguracaoDominio(string caminhoConfiguracao)
        {
            var json = File.ReadAllText(caminhoConfiguracao, UTF8Encoding.UTF8);
            var teste = TextoUtil.RemoverAcentos(json);
            return JsonUtil.Deserializar<ConfiguracaoProjetoDominio>(json, true);
        }

        protected override void AtualizarInterno()
        {
            this.AtualizarAtribuirPropriedades();
            if (this.IsExisteDll)
            {
                var geradores = this.RetornarGeradoresDominio();
                foreach (var gerador in geradores)
                {
                    gerador.Gerar();
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
                    throw new FileNotFoundException(dominioDependente.Caminho);
                }

                if (!AjudanteAssembly.AssemblyCaminhos.ContainsKey(dominioDependente.Nome))
                {
                    AjudanteAssembly.AssemblyCaminhos.Add(dominioDependente.Nome, new string[] { caminhoAbsoluto });
                }
            }
        }

        private List<Type> RetornarTodosTipo()
        {
            if (this.IsExisteDll)
            {
                if (this.IsProjetoSneburDominio)
                {
                    return AjudanteAssembly.TiposSneburDominio;
                }
                else
                {
                    try
                    {
                        var assembly = AjudanteAssembly.RetornarAssembly(this.CaminhoAssembly);
                        return assembly.GetAccessibleTypes().Where(x => x != null && x.IsPublic).ToList();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        return e.Types.Where(x => x != null && x.IsPublic).ToList();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return new List<Type>();
        }
    }
}
