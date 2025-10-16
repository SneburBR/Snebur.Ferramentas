using Snebur;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    public class ErroGlobal : Erro
    {

        public ErroGlobal(string mensagem = "",
                         Exception erroInterno = null,
                         [CallerMemberName] string nomeMetodo = "",
                         [CallerFilePath] string caminhoArquivo = "",
                         [CallerLineNumber] int linhaDoErro = 0) :
                         base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        protected ErroGlobal(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion

#if !EXTENSAO_VISUALSTUDIO

        protected override EnumNivelErro RetornarNivelErro()
        {
            return EnumNivelErro.Critico;
        }
#endif
    }
}