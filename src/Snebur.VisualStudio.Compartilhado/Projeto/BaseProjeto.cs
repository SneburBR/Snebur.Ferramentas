using Microsoft.Identity.Client;
using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{



    public abstract class BaseProjeto : BaseViewModel
    {
        private bool _isNormalizando;
        private List<string> SufixosProtegidos { get; } = new List<string> { ".Completo", ".Debug", ".Temp", ".Teste" };

        public string NomeProjeto { get; private set; }

        public FileInfo ArquivooProjeto { get; }
        public DirectoryInfo DiretorioProjeto { get; }

        public string CaminhoProjeto => this.DiretorioProjeto.FullName;

        public string CaminhoProjetoCaixaBaixa { get; }

        public string CaminhoConfiguracao { get; }

        public bool Globalizar { get; }

        public bool IsProjetoDebug { get; }

        //public DTE2 DTE { get; }

        public Version VersaoProjeto => AssemblyInfoUtil.RetornarVersaoAssemblyInfo(this.CaminhoProjeto,
                                                                                   this.CaminhoAssemblyInfo);

        public string CaminhoAssemblyInfo { get; }
        public virtual string CaminhoAssembly { get; protected set; }

        public string NomeAssembly
        {
            get
            {
                if (this is ProjetoDominio ||
                    this is ProjetoServicosTypescript || this is ProjetoServicosDotNet)
                {
                    return this.NomeProjeto;
                    //return this.NomeProjeto == "Snebur" ? "Zyoncore" : this.NomeProjeto;
                }
                if (this is ProjetoTypeScript)
                {
                    return Path.GetFileNameWithoutExtension(this.ArquivooProjeto.Name);
                }
                return this.NomeProjeto;
            }
        }
        //public string UniqueName { get; set; }
        public ProjetoViewModel ProjetoViewModel { get; }

        //public object ProjetoVS => this.ProjetoViewModel.ProjetoVS;

        public string Chave { get; }

        public bool IsNormalizado { get; private set; }
        public BaseProjeto(ProjetoViewModel projetoVM,
                           FileInfo arquivoProjeto,
                           string caminhoConfiguracao)
        {
            this.ProjetoViewModel = projetoVM;
            this.ArquivooProjeto = arquivoProjeto;
            this.DiretorioProjeto = arquivoProjeto.Directory;
            this.CaminhoConfiguracao = caminhoConfiguracao;
            this.CaminhoProjetoCaixaBaixa = this.CaminhoProjeto.ToLower();
            this.CaminhoAssemblyInfo = AssemblyInfoUtil.RetornarCaminhoAssemblyInfo(this.CaminhoProjeto);
            this.NomeProjeto = this.NormalizarNomeProjeto(Path.GetFileNameWithoutExtension(this.ArquivooProjeto.Name));
            this.CaminhoAssembly = AjudanteAssembly.RetornarCaminhoAssembly(this.ProjetoViewModel.TipoCsProj,
                                                                            this.DiretorioProjeto.FullName,
                                                                            this.NomeAssembly,
                                                                            true);

            this.Chave = BaseProjeto.RetornarChave(this.CaminhoProjeto);
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
                nomeProjeto = nomeProjeto.Substring(0, nomeProjeto.Length - ".Typescript".Length);

            }
            if (nomeProjeto.EndsWith(".TS"))
            {
                nomeProjeto = nomeProjeto.Substring(0, nomeProjeto.Length - ".TS".Length);
            }

            if (this.ProjetoViewModel.TipoCsProj == EnumTipoCsProj.MicrosoftSdk &&
                nomeProjeto.EndsWith(".Net"))
            {
                return nomeProjeto.Substring(0, nomeProjeto.Length - 4); ;
            }

            return nomeProjeto;
        }

        #region Abstratos
        public async Task NormalizarReferenciasAsync(bool compilar =false)
        {
            try
            {
                if (this._isNormalizando)
                {
                    return;
                }
                if (compilar)
                {
                    await this.CompilarAsync();
                }
                this._isNormalizando = true;
                await WorkThreadUtil.SwitchToWorkerThreadAsync();
                this.AtualizarInterno();
                 
                this.IsNormalizado = true;
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(new Exception($"Não foi possível normalizar o projeto {this.NomeProjeto}", ex));
            }
            finally
            {
                this._isNormalizando = false;
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

        public Task CompilarAsync()
        {
            return BaseAplicacaoVisualStudio.Instancia.CompilarProjetoAsync(this);
        }



        public virtual void InscrementarVersao()
        {
            AssemblyInfoUtil.InscrementarVersao(this.CaminhoProjeto,    
                                                this.CaminhoAssemblyInfo);

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
         
        public static string RetornarChave(string caminhoProjeto)
        {
            if(Path.GetExtension(caminhoProjeto).Equals(".csproj", StringComparison.InvariantCultureIgnoreCase))
            {
                caminhoProjeto = Path.GetDirectoryName(caminhoProjeto);
            }
            return ArquivoUtil.NormalizarCaminhoArquivo(caminhoProjeto).ToLower();
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
        public bool IsProjetoSneburDominio { get; set; }
        public List<string> NomesProjetoDepedencia => this.ConfiguracaoProjeto.ProjetoDepedencia;



        public BaseProjeto(ProjetoViewModel projetoVM,
                           TConfiguracaoProjeto configuracaoProjeto,
                           FileInfo arquivoProjeto, string caminhoConfiguracao) :
                           base(projetoVM, arquivoProjeto, caminhoConfiguracao)
        {
            this.ConfiguracaoProjeto = configuracaoProjeto;
        }

        protected override TConfiguracao RetornarConfiguracaoProjeto<TConfiguracao>()
        {
            return this.ConfiguracaoProjeto as TConfiguracao;
        }

        public bool IsExisteDll => File.Exists(this.CaminhoAssembly);
        
        protected virtual List<Type> RetornarTodosTipo()
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
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            return new List<Type>();
        }
    }
}


