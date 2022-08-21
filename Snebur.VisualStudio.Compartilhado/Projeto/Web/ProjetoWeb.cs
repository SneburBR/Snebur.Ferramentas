using Snebur.Utilidade;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snebur.VisualStudio
{
    public abstract class ProjetoWeb<TConfiguracao> : BaseProjeto<TConfiguracao> where TConfiguracao : BaseConfiguracaoProjetoWeb
    {
        public string CaminhoBin { get; }
        public string CaminhoDll { get; }
        public FileInfo ArquivoDll => new FileInfo(this.CaminhoDll);

        public string Checksum => ChecksumUtil.RetornarChecksum(this.CaminhoDll);

        public ProjetoWeb(TConfiguracao configuracaProjeto, 
                          string caminhoProjeto, 
                          string caminhoConfiguracao) :
                          base(configuracaProjeto,   caminhoProjeto, caminhoConfiguracao)
        {
            this.CaminhoBin = Path.Combine(this.CaminhoProjeto, "bin");
            this.CaminhoDll = AjudanteAssembly.RetornarCaminhoAssembly(configuracaProjeto);
        }

        protected override void AtualizarInterno()
        {

        }

        #region Métodos  

        protected override void DispensarInerno()
        {

        }

        internal void AtualizarVersao()
        {

        }

        internal List<FileInfo> RetornarArquivosPastaBin()
        {
            var dlls = Directory.EnumerateFiles(this.CaminhoBin, "*.dll");
            var pdbs = Directory.EnumerateFiles(this.CaminhoBin, "*.pdb");
            var arquivos = new List<FileInfo>();
            arquivos.AddRange(dlls.Select(x => new FileInfo(x)));
            arquivos.AddRange(pdbs.Select(x => new FileInfo(x)));
            return arquivos;
        }

        #endregion




       

    }
}
