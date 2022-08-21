using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.IO;
using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio.Projeto.TypeScript
{
    public abstract class BaseArquivoTypeScript : IDisposable
    {
        public ProjetoTypeScript Projeto { get; set; }

        public FileInfo Arquivo { get; set; }

        public EnumTipoArquivoTypeScript TipoArquivoTypeScript { get; set; }

        public int PrioridadeProjeto { get; set; } = 0;

        //public string DiretorioProjeto { get; set; }

        public string Namespace { get; set; }

        public string CaminhoClasseSuperior { get; set; }

        public string[] Linhas { get; set; }

        public BaseArquivoTypeScript(ProjetoTypeScript projeto, FileInfo arquivo, int prioridadeProjeto)
        {
            //LogUtils.Log("Analisando arquivo {0} - {1} ", arquivo.Name, arquivo.FullName);
            if (!arquivo.Exists)
            {
                throw new FileNotFoundException(String.Format("Não foi encontrado o arquivo : {0} ", arquivo.FullName));
            }

            this.Arquivo = arquivo;
            this.Linhas = File.ReadAllLines(this.Arquivo.FullName, System.Text.UTF8Encoding.UTF8);
            this.TipoArquivoTypeScript = this.RetornarTipoArquivoTypeScript();
            this.Namespace = this.RetornarNamespace();
            this.CaminhoClasseSuperior = this.RetornarClasseSuperior();
            this.PrioridadeProjeto = prioridadeProjeto;

        }

        protected abstract EnumTipoArquivoTypeScript RetornarTipoArquivoTypeScript();

        protected abstract string RetornarNamespace();

        protected abstract string RetornarClasseSuperior();

        #region IDisposable

        public void Dispose()
        {
            Array.Clear(this.Linhas, 0, this.Linhas.Length);
        }

        #endregion
    }
}
