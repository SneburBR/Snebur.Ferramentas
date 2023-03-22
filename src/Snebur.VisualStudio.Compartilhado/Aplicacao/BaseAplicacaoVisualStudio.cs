using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    public abstract class BaseAplicacaoVisualStudio : AplicacaoSnebur
    {
        private IConfiguracaoGeral _configuracaoGeral;
        private ILogVS _logVS;
        private IGerenciadorProjetos _gerenciadorProjetos;

        public IConfiguracaoGeral ConfiguracaoGeral => LazyUtil.RetornarValorLazyComBloqueio(ref this._configuracaoGeral,
                                                                                             this.RetornarConfiguracaoGeral);
        public ILogVS LogVS => LazyUtil.RetornarValorLazyComBloqueio(ref this._logVS,
                                                                     this.RetornarLogVS);
        public IGerenciadorProjetos GerenciadorProjetos => LazyUtil.RetornarValorLazyComBloqueio(ref this._gerenciadorProjetos,
                                                                                                 this.RetornarGerenciadorProjetos);

        protected abstract IConfiguracaoGeral RetornarConfiguracaoGeral();
        protected abstract ILogVS RetornarLogVS();
        protected abstract IGerenciadorProjetos RetornarGerenciadorProjetos();
        internal protected abstract Task CompilarProjetoAsync(BaseProjeto baseProjeto);

        internal protected abstract Task<IEnumerable<string>> RetornarTodosArquivosProjetoAsync(object projetoVS, string caminhoProjeto, bool isLowerCase);

        public abstract bool CheckAccess();
        
        #region Static


        private static BaseAplicacaoVisualStudio _instancia;
        public static BaseAplicacaoVisualStudio Instancia => LazyUtil.RetornarValorLazyComBloqueio(ref _instancia, () =>
        {
            if (Atual is BaseAplicacaoVisualStudio instancia)
            {
                return instancia;
            }
            throw new Exception($"A aplicação atual não é do tipo {nameof(BaseAplicacaoVisualStudio)}");
        });
        #endregion


    }
}
