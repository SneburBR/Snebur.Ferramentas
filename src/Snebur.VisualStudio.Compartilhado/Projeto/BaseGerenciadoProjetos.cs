using Snebur.Utilidade;
using System;
using System.Drawing.Text;

namespace Snebur.VisualStudio
{
    public abstract class BaseGerenciadoProjetos : IGerenciadorProjetos
    {
        public static BaseGerenciadoProjetos _instancia;
        public static BaseGerenciadoProjetos TryIntancia => _instancia;
        protected BaseGerenciadoProjetos()
        {
            if (_instancia != null)
            {
                throw new Exception("Instancia única permitida");
            }
        }

        public string DiretorioProjetoTypescriptInicializacao { get; private set; }
        public ConfiguracaoProjetoTypeScript ConfiguracaoProjetoTypesriptInicializacao { get; private set; }
        public bool IsReiniciarGerenciadorPendente { get; private set; }

        public abstract void AtualizarProjetoTS(ProjetoTypeScript projetoTypeScript);
        public abstract void AtualizarProjetoSass(ProjetoSass projetoEstilo);


        public void SetDiretorioProjetoTypescriptInicializacao(string diretorioProjetoTypescript)
        {
            this.DiretorioProjetoTypescriptInicializacao = diretorioProjetoTypescript;
        }

        public void SetConfiguracaoProjetoTypesriptInicializacao(ConfiguracaoProjetoTypeScript configuracaoProjetoTypeScript)
        {
            this.ConfiguracaoProjetoTypesriptInicializacao = configuracaoProjetoTypeScript;
        }

        internal void SetReiniciarGerenciadorPendente(bool value)
        {
            this.IsReiniciarGerenciadorPendente = value;
        }
    }


    public abstract class BaseGerenciadoProjetos<TGerenciadorProjeto> : BaseGerenciadoProjetos where TGerenciadorProjeto : class, IGerenciadorProjetos, new()
    {

        public static TGerenciadorProjeto Instancia
        {
            get
            {
                return LazyUtil.RetornarValorLazyComBloqueio(
                                ref _instancia,
                                () => Activator.CreateInstance<TGerenciadorProjeto>() as BaseGerenciadoProjetos) as TGerenciadorProjeto;
            }
        }

        protected BaseGerenciadoProjetos() : base()
        {

        }

    }
}


