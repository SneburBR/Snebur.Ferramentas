using Snebur;
using Snebur.Utilidade;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    public class Erro : Exception, ISerializable
    {

        internal protected virtual bool IsNotificarErro => true;

        internal protected virtual bool IsParaDepuracaoAtachada => true;

        internal bool NotificaoEnviada { get; private set; }

        public string NomeMetodo { get; }
        public string CaminhoArquivo { get; }
        public int LinhaDoErro { get; }

        public bool IsExisteNomeMetodo => !String.IsNullOrEmpty(this.NomeMetodo);

        public bool IsExisteCaminhoArquivo => !String.IsNullOrEmpty(this.CaminhoArquivo);

        public bool IsExisteLinhaDoErro => this.LinhaDoErro > 0;

        public Erro(string mensagem = "",
                    Exception erroInterno = null,
                    [CallerMemberName] string nomeMetodo = "",
                    [CallerFilePath] string caminhoArquivo = "",
                    [CallerLineNumber] int linhaDoErro = 0) : base(mensagem, erroInterno)
        {
            this.NomeMetodo = nomeMetodo;
            this.CaminhoArquivo = caminhoArquivo;
            this.LinhaDoErro = linhaDoErro;

#if !EXTENSAO_VISUALSTUDIO

            if (this.IsNotificar())
            {
                LogUtil.ErroAsync(this, nomeMetodo, caminhoArquivo, linhaDoErro);
                this.NotificaoEnviada = true;
            }

            GerenciadorErros.Instancia.AnalisarErroAsync(this);

#endif
        }
        #region Serialização 

        public Erro()
        {
        }

        protected Erro(SerializationInfo info, StreamingContext context)
#if NET6_0
            : base(info, context)
#endif
        {
        }
#endregion

        protected virtual bool IsNotificar()
        {
            if (this.IsNotificarErro && !DebugUtil.IsAttached)
            {
                if (this.InnerException is Erro erro && erro.NotificaoEnviada)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
#if !EXTENSAO_VISUALSTUDIO

        public EnumNivelErro NivelErro
        {
            get
            {
                return this.RetornarNivelErro();
            }
        }

        protected virtual EnumNivelErro RetornarNivelErro()
        {
            return EnumNivelErro.Normal;
        }
#endif
    }
}