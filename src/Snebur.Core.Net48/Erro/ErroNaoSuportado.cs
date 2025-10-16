using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    public class ErroNaoSuportado : Erro
    {
        public ErroNaoSuportado(Type tipo,
                              Exception erroInterno = null,
                              [CallerMemberName] string nomeMetodo = "",
                              [CallerFilePath] string caminhoArquivo = "",
                              [CallerLineNumber] int linhaDoErro = 0) :
                              base(RetornarMensagem(tipo), erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }

        public ErroNaoSuportado(object objeto,
                                Exception erroInterno = null,
                                [CallerMemberName] string nomeMetodo = "",
                                [CallerFilePath] string caminhoArquivo = "",
                                [CallerLineNumber] int linhaDoErro = 0) :
                                base(RetornarMensagem(objeto?.GetType() ?? null), erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }

        public ErroNaoSuportado(string mensagem = "",
                                Exception erroInterno = null,
                                [CallerMemberName] string nomeMetodo = "",
                                [CallerFilePath] string caminhoArquivo = "",
                                [CallerLineNumber] int linhaDoErro = 0) :
                                base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }

        private static string RetornarMensagem(Type tipo)
        {
            if (tipo != null)
            {
                return $"O tipo {tipo.Name} não é suportado.";
            }
            return "o tipo não é suportado";
        }
        #region Serializacao 

        public ErroNaoSuportado()
        {
        }

        protected ErroNaoSuportado(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}