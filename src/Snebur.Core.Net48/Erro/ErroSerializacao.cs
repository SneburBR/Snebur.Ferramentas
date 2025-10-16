using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    public class ErroSerializacao : Erro
    {
        public ErroSerializacao(string conteudo,
                              Exception erroInterno = null,
                              [CallerMemberName] string nomeMetodo = "",
                              [CallerFilePath] string caminhoArquivo = "",
                              [CallerLineNumber] int linhaDoErro = 0) :
                              base(RetornarMensagem(conteudo, erroInterno), erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }

        public ErroSerializacao(object objeto,
                                Exception erroInterno = null,
                                [CallerMemberName] string nomeMetodo = "",
                                [CallerFilePath] string caminhoArquivo = "",
                                [CallerLineNumber] int linhaDoErro = 0) :
                                base(RetornarMensagem(objeto, erroInterno), erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        //public ErroSerializacao(string mensagem = "",
        //                        Exception erroInterno = null,
        //                        [CallerMemberName] string nomeMetodo = "",
        //                        [CallerFilePath] string caminhoArquivo = "",
        //                        [CallerLineNumber] int linhaDoErro = 0) :
        //                        base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        //{

        //}

        private static string RetornarMensagem(object objeto, Exception erroInterno)
        {
            if (objeto != null)
            {
                return $"Não foi possivel serializar o objeto do  {objeto.GetType()} ." +
                       $"\r\nMensagem: {erroInterno?.Message}";
            }
            return "O objeto não foi definido para serializacao";
        }

        private static string RetornarMensagem(string conteudo, Exception erroInterno)
        {
            if (conteudo != null)
            {
                return $"Não foi possivel deserializar o json  " +
                       $"\r\n Mensagem: {erroInterno?.Message}" +
                       $"\r\n  {conteudo} .";
            }
            return "O objeto não foi definido para serializacao";
        }
        #region Serializacao 

        public ErroSerializacao()
        {
        }

        protected ErroSerializacao(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}