using Snebur;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    public class ErroThread : Erro
    {

        public ErroThread(string mensagem = "",
                         Exception erroInterno = null,
                         [CallerMemberName] string nomeMetodo = "",
                         [CallerFilePath] string caminhoArquivo = "",
                         [CallerLineNumber] int linhaDoErro = 0) :
                         base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        protected ErroThread(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected override EnumNivelErro RetornarNivelErro()
        {
            return EnumNivelErro.Baixo;
        }
        #endregion
    }
}