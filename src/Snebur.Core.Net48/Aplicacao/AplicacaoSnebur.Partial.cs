using System;

namespace Snebur
{
    public abstract partial class AplicacaoSnebur
    {
        private readonly static object _bloqueio = new object();
        internal static int _mainThreadId;
        internal static AplicacaoSnebur _aplicacao;
        public static AplicacaoSnebur Atual
            => _aplicacao;

        public static T AtualTipada<T>() where T : AplicacaoSnebur
        {
            return (T)Atual;
        }

        private static AplicacaoSnebur RetornarAplicacaoAtual()
        {
            //var assemblyEntrada = ReflexaoUtil.AssemblyEntrada;
            //if (assemblyEntrada != null)
            //{
            //    var tipoAplicacao = assemblyEntrada.GetAccessibleTypes().Where(x => x.IsSubclassOf(typeof(AplicacaoSnebur))).SingleOrDefault();
            //    if (tipoAplicacao != null)
            //    {
            //        return (AplicacaoSnebur)Activator.CreateInstance(tipoAplicacao);
            //    }
            //}

            throw new Exception("A aplicação atual não foi inicializada, a deve ser inicializar no StartUp da aplicação");
 
            //return new AplicacaoSneburInterna();
        }

       
    }

}