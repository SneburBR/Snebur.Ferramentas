using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Snebur.AcessoDados
{
    [Serializable]
    public class ErroFalhaAutenticacao : Erro
    {
        public ErroFalhaAutenticacao(string mensagem = "",
                                     Exception erroInterno = null,
                                     [CallerMemberName] string nomeMetodo = "",
                                     [CallerFilePath] string caminhoArquivo = "",
                                     [CallerLineNumber] int linhaDoErro = 0) :
                                     base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroFalhaAutenticacao()
        {
        }

        protected ErroFalhaAutenticacao(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}