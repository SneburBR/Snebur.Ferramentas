using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public abstract class BaseArquivoTypeScript : IDisposable
    {
        //public ProjetoTypeScript Projeto2 { get; set; }

        public FileInfo Arquivo { get; }
        public string SubExtensao { get; }

        public EnumTipoArquivoTypeScript TipoArquivoTypeScript { get; }

        public int PrioridadeProjeto { get; } = 0;

        //public string DiretorioProjeto { get; set; }

        public string Namespace { get; }

        public string NomeTipoBase { get; }

        public string CaminhoTipoBase { get; }

        public string NomeTipo { get; }

        public string CaminhoTipo { get; }

        public bool IsExisteTipo => !String.IsNullOrWhiteSpace(this.CaminhoTipo);

        public bool IsExisteTipoBase => !String.IsNullOrWhiteSpace(this.CaminhoTipoBase);


        //public string NomeTipoBase { get; }

        //public string NamespaceBase { get; }

        public List<string> Linhas { get; }

        public string CaminhoArquivo { get; }

        public BaseArquivoTypeScript(FileInfo arquivo, 
                                    int prioridadeProjeto  )
        {
            //LogUtils.Log("Analisando arquivo {0} - {1} ", arquivo.Name, arquivo.FullName);
            if (!arquivo.Exists)
            {
                throw new FileNotFoundException(String.Format("Não foi encontrado o arquivo : {0} ", arquivo.FullName));
            }

            this.Arquivo = arquivo;
            this.CaminhoArquivo = ArquivoUtil.NormalizarCaminhoArquivo(arquivo.FullName).ToLower();
            this.SubExtensao = ArquivoUtil.RetornarSubExtenmsao(arquivo.Name).ToLower();
            this.Linhas = File.ReadAllLines(this.Arquivo.FullName, System.Text.UTF8Encoding.UTF8).ToList();
            this.TipoArquivoTypeScript = this.RetornarTipoArquivoTypeScript();

            this.Namespace = this.RetornarNamespace();
            this.NomeTipo = this.RetornarNomeTipo();

            if (!String.IsNullOrEmpty(this.NomeTipo))
            {
                this.CaminhoTipo = (String.IsNullOrEmpty(this.Namespace)) ? this.NomeTipo : String.Format("{0}.{1}", this.Namespace, this.NomeTipo);
            }

            this.CaminhoTipoBase = this.RetornarCaminhoClasseBase();
            this.PrioridadeProjeto = prioridadeProjeto;

            if (!String.IsNullOrWhiteSpace(this.CaminhoTipoBase))
            {
                var partesBase = this.CaminhoTipoBase.Split('.');
                this.NomeTipoBase = partesBase.Last();
                if (partesBase.Length == 1)
                {
                    this.CaminhoTipoBase = $"{this.Namespace}.{this.NomeTipoBase}";
                }
             }
        }

        protected abstract EnumTipoArquivoTypeScript RetornarTipoArquivoTypeScript();

        protected abstract string RetornarNamespace();

        protected abstract string RetornarCaminhoClasseBase();

        protected virtual string RetornarNomeTipo()
        {
            return null;
        }

        public override string ToString()
        {
            return $"{this.Arquivo.Name}-{this.TipoArquivoTypeScript.ToString()}-{this.GetType().Name}";
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (this == obj) return true;
                if (obj is BaseArquivoTypeScript arquivo &&
                    this.GetType() == obj.GetType()  )
                {
                    return this.CaminhoArquivo.Equals(arquivo.CaminhoArquivo);
                        //ArquivoUtil.CaminhoIgual(this.CaminhoArquivo, 
                        //                            arquivo.Arquivo.FullName);
                }
            }
            return false;
        }
        public override int GetHashCode()
        {
            return this.CaminhoArquivo.GetHashCode();
        }
          
        #region IDisposable

        public void Dispose()
        {
            this.Linhas.Clear();
            //Array.Clear(this.Linhas, 0, this.Linhas.Count);
        }

        #endregion
    }
}
