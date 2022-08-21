using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.IO;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public abstract class BaseProjeto : Snebur.Dominio.BaseViewModel, IDisposable
    {
        private List<string> SufixosProtegidos { get; } = new List<string> { ".Completo", ".Debug", ".Temp", ".Teste" };

        //public Project ProjetoVS { get; private set; }

        public string NomeProjeto { get; private set; }

        public System.IO.DirectoryInfo DiretorioProjeto { get; }

        public string CaminhoProjeto { get; }

        public string CaminhoProjetoCaixaBaixa { get; }

        public string CaminhoConfiguracao { get; }

        public bool Globalizar { get; }

        public bool IsProjetoDebug { get; }

        //public DTE2 DTE { get; }

        public Version VersaoProjeto => AssemblyInfoUtil.RetornarVersaoAssemblyInfo(this.CaminhoAssemblyInfo);

        public string CaminhoAssemblyInfo { get; }
        public BaseProjeto( string caminhoProjeto, string caminhoConfiguracao)
        {
            //this.DTE = dte;
            //this.ProjetoVS = projetoVS;

            this.CaminhoProjeto = caminhoProjeto;
            this.DiretorioProjeto = new DirectoryInfo(this.CaminhoProjeto);
            this.CaminhoConfiguracao = caminhoConfiguracao;
            
            this.CaminhoProjetoCaixaBaixa = this.CaminhoProjeto.ToLower();
            //this.IsProjetoDebug = projetoVS.Name.EndsWith("Debug");
            this.CaminhoAssemblyInfo = AssemblyInfoUtil.RetornarCaminhoAssemblyInfo(this.CaminhoProjeto);

            throw new ErroNaoImplementado();
            //this.NomeProjeto = this.NormalizarNomeProjeto(this.ProjetoVS.Name);
        }

        private string NormalizarNomeProjeto(string nomeProjeto)
        {
            var nomeDiretorio = this.DiretorioProjeto.Name;
            var sufixosEncontrados = this.SufixosProtegidos.Where(x => nomeProjeto.Contains(x)).ToList();

            if (sufixosEncontrados.Count > 1)
            {
                throw new Exception($"Foram encontrados mais de um sufixo protegido no nome do projeto {nomeProjeto}, encontrados {String.Join(",", sufixosEncontrados)} ");
            }

            var sufixoEncontrado = sufixosEncontrados.SingleOrDefault();
            if (sufixoEncontrado != null)
            {
                if (!nomeDiretorio.Contains(sufixoEncontrado))
                {
                    var fim = nomeProjeto.LastIndexOf(sufixoEncontrado);
                    return nomeProjeto.Substring(0, fim);
                }
            }

            if (nomeProjeto.EndsWith(".Typescript"))
            {
                nomeProjeto = nomeProjeto.Substring(0, nomeProjeto.Length -  ".Typescript".Length);

            }
            if (nomeProjeto.EndsWith(".TS"))
            {
                nomeProjeto = nomeProjeto.Substring(0, nomeProjeto.Length - ".TS".Length);
            }
            return nomeProjeto;
        }

        #region Abstratodos
        public void NormalizarReferencias(bool compilar)
        {
            try
            {
                if (compilar)
                {
                    this.Compilar();
                }

                this.AtualizarInterno();
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(new Exception($"Não foi possível atualizar o projeto {this.NomeProjeto}", ex));
            }
        }

        protected abstract void AtualizarInterno();

        protected abstract void DispensarInerno();

        public ConfiguracaoProjeto ConfiguracaoProjeto
        {
            get
            {
                return this.RetornarConfiguracaoProjeto<ConfiguracaoProjeto>();
            }
        }

        protected abstract TConfiguracao RetornarConfiguracaoProjeto<TConfiguracao>() where TConfiguracao : ConfiguracaoProjeto;

        #endregion

        public void Compilar()
        {
            throw new ErroNaoImplementado();
            //ProjetoUtil.CompilarProjeto(this.DTE, this.ProjetoVS);
        }



        public virtual void InscrementarVersao()
        {
            AssemblyInfoUtil.InscrementarVersao(this.CaminhoAssemblyInfo);
            this.NotificarPropriedadeAlterada(nameof(this.VersaoProjeto));
            //var versao = this.VersaoProjeto;
            //var novaVersao = new Version(versao.Major, versao.Minor, versao.Build, versao.Revision + 1);
            //var caminhoAssemblyInfo = this.CaminhoAssemblyInfo;
            //if (File.Exists(caminhoAssemblyInfo))
            //{
            //    var linhas = File.ReadAllLines(caminhoAssemblyInfo, Encoding.UTF8);
            //    for (var i = 0; i < linhas.Count(); i++)
            //    {
            //        var linha = linhas[i];
            //        if (linha.Trim().StartsWith(PROCURAR_LINHA_VERSAO))
            //        {

            //            var novaLinha = $"{PROCURAR_LINHA_VERSAO}(\"{novaVersao.Major}.{novaVersao.Minor}.{novaVersao.Build}.{novaVersao.Revision}\")]";
            //            linhas[i] = novaLinha;
            //        }
            //        if (linha.Trim().StartsWith(PROCURAR_LINHA_VERSAO_ARQUIVO))
            //        {
            //            linhas[i] = $"{PROCURAR_LINHA_VERSAO_ARQUIVO}(\"{novaVersao.Major}.{novaVersao.Minor}.{novaVersao.Build}.{novaVersao.Revision}\")]";
            //        }
            //    }
            //    File.WriteAllLines(caminhoAssemblyInfo, linhas, Encoding.UTF8);
            //}
            //this.NotificarPropriedadeAlterada(nameof(this.VersaoProjeto));
        }

        public void Dispose()
        {
            //this.ProjetoVS = null;
            this.DispensarInerno();

        }
    }

    public abstract class BaseProjeto<TConfiguracaoProjeto> : BaseProjeto where TConfiguracaoProjeto : ConfiguracaoProjeto
    {
        public new TConfiguracaoProjeto ConfiguracaoProjeto { get; }

        public List<string> NomesProjetoDepedencia => this.ConfiguracaoProjeto.ProjetoDepedencia;

        public BaseProjeto(TConfiguracaoProjeto configuracaoProjeto, 
                           string caminhoProjeto, string caminhoConfiguracao) :
                           base(caminhoProjeto, caminhoConfiguracao)
        {
            this.ConfiguracaoProjeto = configuracaoProjeto;
        }

        protected override TConfiguracao RetornarConfiguracaoProjeto<TConfiguracao>()
        {
            return this.ConfiguracaoProjeto as TConfiguracao;
        }

       

    }


}
