using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Snebur.Tarefa
{
    [Serializable]
    public class ErroTarefa : Erro
    {
        public ErroTarefa(string mensagem = "",
                          Exception erroInterno = null,
                          [CallerMemberName] string nomeMetodo = "",
                          [CallerFilePath] string caminhoArquivo = "",
                          [CallerLineNumber] int linhaDoErro = 0) :
                          base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroTarefa()
        {
        }

        protected ErroTarefa(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}