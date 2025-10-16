using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Snebur.Utilidade
{
    public static class ThreadUtil
    {
        //private static bool DEBUG_ATTACH_SINCRONO = false;
        //private static bool ATIVAR_CONTROLADOR_THREAD = false;
        //public static Dictionary<string, ThreadInfo> DicionarioThreadsInfo = new Dictionary<string, ThreadInfo>();

        private static int ContadorThread = 0;
        private static readonly Dictionary<string, object> _bloqueios = new Dictionary<string, object>();

        private readonly static object _bloqueio = new object();

        public static object RetornarBloqueio(Guid chave)
        {
            return RetornarBloqueio(chave.ToString());
        }

        public static object RetornarBloqueio(long chave)
        {
            return RetornarBloqueio(chave.ToString());
        }

        public static object RetornarBloqueio(string chave)
        {
            if (!_bloqueios.ContainsKey(chave))
            {
                lock (_bloqueio)
                {
                    if (!_bloqueios.ContainsKey(chave))
                    {
                        _bloqueios.Add(chave, new object());
                    }
                }
            }
            return _bloqueios[chave];
        }

        public static void ExecutarDepoisAsync(Action acao, TimeSpan tempoExperar, bool ignorarErro = true, [CallerMemberName] string nomeMetodo = "", [CallerFilePath] string caminhoArquivo = "")
        {
            TaskUtil.Run(() =>
            {
                Thread.Sleep((int)tempoExperar.TotalMilliseconds);
                ExecutarAsync(acao, ignorarErro, nomeMetodo, caminhoArquivo);
            });
        }

        public static Task EsperarAsync(TimeSpan tempo)
        {
            return EsperarAsync((int)tempo.TotalMilliseconds);
        }

        public static Task EsperarAsync(int totalMilesegundos)
        {
            return TaskUtil.Run(() =>
            {
                Thread.Sleep(totalMilesegundos);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="acao"></param>
        /// <param name="ignorarErro">Em caso de erro, a aplicação não será finaliza, mas o log de erro será acionado</param>
        /// <param name="nomeMetodo"></param>
        /// <param name="debugAttachedSincrono"></param>
        public static void ExecutarAsync(Action acao, bool? ignorarErro = null, [CallerMemberName] string nomeMetodo = "", [CallerFilePath] string caminhoArquivo = "")
        {
            if (ignorarErro == null)
            {
                ignorarErro = AplicacaoSnebur.Atual.IgnorarErros;
            }
            if (ignorarErro.Value)
            {
                var nomeClasse = System.IO.Path.GetFileNameWithoutExtension(caminhoArquivo);
                var nomeThread = RetornarNomeThread(nomeClasse, nomeMetodo);

                var controleThead = new ThreadControleErro(acao, nomeThread);
                controleThead.ExecutarAsync();
            }
            else
            {
                ExecutarInternoAsync(acao, nomeMetodo, caminhoArquivo);
            }
        }

        public static void ExecutarMainThread(Action acao, [CallerMemberName] string nomeMetodo = "",
                                                           [CallerFilePath] string caminhoArquivo = "")
        {
            if (AplicacaoSnebur.Atual.DispatcherObject != null)
            {
                AplicacaoSnebur.Atual.DispatcherObject.Invoke(acao);
            }
            else
            {
                ExecutarStaAsync(acao, nomeMetodo, caminhoArquivo);
            }
        }

        public static bool IsMainThread()
        {
            if (AplicacaoSnebur.Atual.DispatcherObject != null)
            {
                return AplicacaoSnebur.Atual.DispatcherObject.CheckAccess();
            }
            return false;
        }

        public static void ExecutarStaAsync(Action acao, [CallerMemberName] string nomeMetodo = "",
                                                         [CallerFilePath] string caminhoArquivo = "")
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new Exception("Thread Sta é apenas suportada pelo windows");
            }

            var nomeClasse = System.IO.Path.GetFileNameWithoutExtension(caminhoArquivo);
            var nomeThread = RetornarNomeThread(nomeMetodo, nomeClasse);

            var thread = new Thread(new ThreadStart(acao))
            {
                CurrentUICulture = AplicacaoSnebur.Atual.CulturaPadrao,
                CurrentCulture = AplicacaoSnebur.Atual.CulturaPadrao,
                IsBackground = false,
                Name = nomeClasse
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private static void ExecutarInternoAsync(Action acao,
                                                [CallerMemberName] string nomeMetodo = "",
                                                [CallerFilePath] string caminhoArquivo = "")
        {
            var nomeClasse = System.IO.Path.GetFileNameWithoutExtension(caminhoArquivo);
            var nomeThread = "W_" + RetornarNomeThread(nomeClasse, nomeMetodo);
            ExecutarThread(acao, nomeThread);
        }

        internal static void ExecutarThread(Action acao, string nomeThread)
        {
            var thread = new Thread(new ThreadStart(acao));
            thread.IsBackground = true;
            thread.CurrentCulture = AplicacaoSnebur.Atual.CulturaPadrao;
            thread.CurrentUICulture = AplicacaoSnebur.Atual.CulturaPadrao;
            thread.Name = nomeThread;
            thread.Start();

#if DEBUG

            //if (ATIVAR_CONTROLADOR_THREAD || DebugUtil.IsAttached)
            //{
            //    if (!ThreadUtil.DicionarioThreadsInfo.ContainsKey(nomeThread))
            //    {
            //        try
            //        {
            //            ThreadUtil.DicionarioThreadsInfo.Add(nomeThread, new ThreadInfo(thread));
            //        }
            //        catch
            //        {
            //        }
            //    }
            //}
#endif
        }

        public static void ControlarErro(Action acao, [CallerMemberName] string nomeMetodo = "",
                                                      [CallerFilePath] string caminhoArquivo = "")
        {
            var nomeClasse = System.IO.Path.GetFileNameWithoutExtension(caminhoArquivo);
            var nomeThread = "ACAO_" + RetornarNomeThread(nomeClasse, nomeMetodo);
            var controleErro = new ThreadControleErro(acao, nomeThread);
            controleErro.Executar();
        }
        //private static void ExecutarAcaoControlarErro(Action acao, string nomeThread)
        //{
        //    try
        //    {
        //        acao.Invoke();
        //    }
        //    catch (Exception ex)
        //    {
        //        var mensagem = $"Erro na thread {nomeThread}";
        //        new ErroThread(mensagem, ex);
        //    }
        //}

        private static object bloqueio = new object();
        private static string RetornarNomeThread(string nomeClasse, string nomeMetodo)
        {
            lock (bloqueio)
            {
                ContadorThread += 1;
                return String.Format("{0}-{1}.{2}", ContadorThread, nomeClasse.RetornarPrimeirosCaracteres(20), nomeMetodo.RetornarPrimeirosCaracteres(20));
            }
        }
#if NET45
        public static async void SleepAsync(double totalMilesegundos)
        {
            await Task.Factory.StartNew(() =>
            {
                Thread.Sleep((int)totalMilesegundos);
            });
        }
#endif

        [Obsolete("Use " + nameof(LazyUtil) + "." + nameof(LazyUtil.RetornarValorLazyComBloqueio))]
        public static T RetornarValorComBloqueio<T>(ref T? valor, Func<T> retornarValor) where T : struct => LazyUtil.RetornarValorLazyComBloqueio(ref valor, retornarValor);

        [Obsolete("Use " + nameof(LazyUtil) + "." + nameof(LazyUtil.RetornarValorLazyComBloqueio))]
        public static T RetornarValorComBloqueio<T>(ref T valor, Func<T> retornarValor) where T : class => LazyUtil.RetornarValorLazyComBloqueio(ref valor, retornarValor);

        /// <summary>
        /// Tenta executar uma exação, um tempo máximo pode ser dinido
        /// </summary>
        /// <param name="p"></param>
        /// <param name="tempoMaximo"> tempo máximo em milissegundos, -1 para infinito</param>
        internal static void TenteExecute(Action acao, int tempoMaximo = Timeout.Infinite)
        {
            try
            {
                var are = new AutoResetEvent(false);
                ExecutarAsync(() =>
                {
                    try
                    {
                        acao.Invoke();
                    }
                    catch
                    {
                    }
                    are.Set();
                    are.Dispose();
                }, false);
                are.WaitOne(tempoMaximo);
            }
            catch
            {
            }
        }

        #region Timeout
        public static Task<T> ExecuteWithTimeout<T>(Func<CancellationToken, T>[] operations, double timeout)
        {
            return OperationWithTimeout.ExecuteWithTimeout(operations, timeout);
        }

        public static Task<T> ExecuteWithTimeout<T>(Func<CancellationToken, T> operation,
                                               double timeout)
        {
            return OperationWithTimeout.ExecuteWithTimeout(operation, timeout);
        }

        public static int SetTimeout(Action acao, int timeout)
        {
            return GerenadorTimeout.SetTimeout(acao, timeout);
        }

        public static void ClearTimeout(int identificador)
        {
            GerenadorTimeout.ClearTimeout(identificador);
        }
        #endregion
    }
    internal static class GerenadorTimeout
    {
        private static readonly ConcurrentDictionary<int, Thread> Threads = new ConcurrentDictionary<int, Thread>();
        private static int _contador;

        internal static int SetTimeout(Action action, int timeout)
        {
            var identificador = Interlocked.Increment(ref _contador);
            var thread = new Thread(new ThreadStart(delegate
            {
                Thread.Sleep(timeout);
                if (Threads.ContainsKey(identificador))
                {
                    Threads.TryRemove(identificador, out var _);
                    TaskUtil.Run(action);
                }
            }));
            Threads.TryAdd(identificador, thread);
            thread.Start();
            return identificador;
        }

        internal static void ClearTimeout(int identificador)
        {
            if (Threads.TryRemove(identificador, out var thread))
            {
#if NET6_0_OR_GREATER == false
                try
                {
                    thread.Abort();
                }
                catch
                {

                }
#endif
            }
        }
    }
}