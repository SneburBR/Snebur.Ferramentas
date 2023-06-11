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

        
        public bool IsReiniciarGerenciadorPendente { get; private set; }

        public abstract void AtualizarProjetoTS(ProjetoTypeScript projetoTypeScript);
        public abstract void AtualizarProjetoSass(ProjetoSass projetoEstilo);
 
        

 
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


