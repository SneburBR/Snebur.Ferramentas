using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    public class ErroNaoImplementado : Erro
    {

        public ErroNaoImplementado(string mensagem = "",
                                   Exception erroInterno = null,
                                   [CallerMemberName] string nomeMetodo = "",
                                   [CallerFilePath] string caminhoArquivo = "",
                                   [CallerLineNumber] int linhaDoErro = 0) : base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroNaoImplementado()
        {
        }

        protected ErroNaoImplementado(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}