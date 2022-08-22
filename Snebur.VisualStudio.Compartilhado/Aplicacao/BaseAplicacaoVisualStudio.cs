using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    public abstract class BaseAplicacaoVisualStudio : AplicacaoSnebur
    {

        private IConfiguracaoGeral _configuracaoGeral;
        private ILogVS _logVS;
        private IGerenciadorProjetos _gerenciadorProjetos;

        public IConfiguracaoGeral ConfiguracaoGeral => ThreadUtil.RetornarValorComBloqueio(ref this._configuracaoGeral,
                                                                                           this.RetornarConfiguracaoGeral);

        public ILogVS LogVS => ThreadUtil.RetornarValorComBloqueio(ref this._logVS,
                                                                   this.RetornarLogVS);

        public IGerenciadorProjetos GerenciadorProjetos => ThreadUtil.RetornarValorComBloqueio(ref this._gerenciadorProjetos,
                                                                                               this.RetornarGerenciadorProjetos);

        protected abstract IConfiguracaoGeral RetornarConfiguracaoGeral();
        protected abstract ILogVS RetornarLogVS();
        protected abstract IGerenciadorProjetos RetornarGerenciadorProjetos();
        internal protected abstract Task CompilarProjetoAsync(BaseProjeto baseProjeto);
        internal protected abstract IEnumerable<string> RetornarTodosArquivosProjeto(object projetoVS, string caminhoProjeto,  bool isLowerCase);

        #region Static


        private static BaseAplicacaoVisualStudio _instancia;
        public static BaseAplicacaoVisualStudio Instancia => ThreadUtil.RetornarValorComBloqueio(ref _instancia, () =>
        {
            if (Atual is BaseAplicacaoVisualStudio instancia)
            {
                return instancia;
            }
            throw new Exception($"A aplicação atual não é do tipo {nameof(BaseAplicacaoVisualStudio)}");
        });
        #endregion
    }

    public interface IGerenciadorProjetos
    {
        string DiretorioProjetoTypescriptInicializacao { get; }
        ConfiguracaoProjetoTypeScript ConfiguracaoProjetoTypesriptInicializacao { get; }
    }

    public interface IConfiguracaoGeral
    {
        string CaminhoProjetos { get; }
        string CaminhoInstalacaoVisualStudio { get; }
    }
}
