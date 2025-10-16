using Snebur.Comunicacao;
using Snebur.Servicos;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Snebur.Utilidade
{
    public static class LogUtil
    {
        #region Métodos públicos

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ErroGlobal(Exception ex,
                                    [CallerMemberName] string nomeMetodo = "",
                                    [CallerFilePath] string caminhoArquivo = "",
                                    [CallerLineNumber] int linhaDoErro = 0)
        {
            if (!(ex is Erro))
            {
                var stackTrace = ex.StackTrace;
                if (String.IsNullOrEmpty(stackTrace))
                {
                    stackTrace = Environment.StackTrace;
                }
                var nivelErro = (ex is Erro) ? (ex as Erro).NivelErro : EnumNivelErro.Normal;
                var informacaoAdicional = ServicoCompartilhadoUtil.RetornarInformacaoAdicionalServicoCompartilhado();
                Erro(ex, stackTrace, nivelErro, informacaoAdicional, nomeMetodo, caminhoArquivo, linhaDoErro);
            }
        }

        public static void ErroAsync(Exception ex,
                                    [CallerMemberName] string nomeMetodo = "",
                                    [CallerFilePath] string caminhoArquivo = "",
                                    [CallerLineNumber] int linhaDoErro = 0)
        {
            try
            {
                if (DebugUtil.IsAttached && IsParaErroDepuracaoAtachada(ex))
                {
                    throw ex;
                }
                if (ex is Erro erro && erro.NotificaoEnviada)
                {
                    return;
                }
                var stackTrace = ex.StackTrace;
                if (String.IsNullOrEmpty(stackTrace))
                {
                    stackTrace = Environment.StackTrace;
                }
                stackTrace += "Nome thread :  '" + Thread.CurrentThread.Name + "' \n" + stackTrace;
                var nivelErro = (ex is Erro) ? (ex as Erro).NivelErro : EnumNivelErro.Normal;
                var informacaoAdicional = ServicoCompartilhadoUtil.RetornarInformacaoAdicionalServicoCompartilhado();
                ThreadUtil.ExecutarAsync((Action)(() =>
                {
                    Erro(ex, stackTrace, nivelErro, informacaoAdicional, nomeMetodo, caminhoArquivo, linhaDoErro);
                }), true);
            }
            catch
            {

            }
        }

        private static bool IsParaErroDepuracaoAtachada(Exception exception)
        {
            //if (exception is Erro erro)
            //{
            //    return erro.IsParaDepuracaoAtachada;
            //}
            //return true;
            return false;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static Guid Erro(Exception ex,
                                   string stackTrace,
                                   EnumNivelErro nivelErro,
                                   BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional,
                                   [CallerMemberName] string nomeMetodo = "",
                                   [CallerFilePath] string caminhoArquivo = "",
                                   [CallerLineNumber] int linhaDoErro = 0)
        {

            var nomeTipoErro = ex.GetType().Name;
            var descricaoCompleta = ErroUtil.RetornarDescricaoCompletaErro(ex, nomeMetodo, caminhoArquivo, linhaDoErro, true);
            var mensagem = ex.Message;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                LogWindows(descricaoCompleta, stackTrace, EventLogEntryType.Error);
            }

            if (String.IsNullOrEmpty(mensagem))
            {
                mensagem = descricaoCompleta.RetornarPrimeirosCaracteres(100);
            }
            try
            {
                return AplicacaoSnebur.Atual.ServicoErro.NotificarErro(nomeTipoErro, 
                                                                       mensagem, 
                                                                       stackTrace, 
                                                                       descricaoCompleta, 
                                                                       nivelErro, 
                                                                       informacaoAdicional);
            }
            catch
            {
                if (!(AplicacaoSnebur.Atual.ServicoErro is ServicoErroLocal))
                {
                    return new ServicoErroLocal().NotificarErro(nomeTipoErro,
                                                                mensagem,
                                                                ex.StackTrace, 
                                                                descricaoCompleta,
                                                                nivelErro, 
                                                                informacaoAdicional);
                }
            }
            return Guid.Empty;
        }

        public static void SegurancaAsync(string mensagem, EnumTipoLogSeguranca tipo, bool erroIsAttach = true)
        {
            var infoRequisicao = AplicacaoSnebur.Atual.AspNet.RetornarInfoRequisicao();

            if (SegurancaUtil.IsGerarLogErro(tipo))
            {
                if (DebugUtil.IsAttached)
                {
                    if (!erroIsAttach)
                    {
                        return;
                    }
                    throw new Exception(mensagem);
                }
                ErroAsync(new Erro(mensagem));
            }

            ThreadUtil.ExecutarAsync(() =>
            {
                var informacaoAdicional = ServicoCompartilhadoUtil.RetornarInformacaoAdicionalServicoCompartilhado();
                Seguranca(mensagem,
                                  infoRequisicao,
                                  tipo,
                                  informacaoAdicional);

            }, true);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        private static Guid Seguranca(string mensagem,
                                      InfoRequisicao infoRequisicao,
                                      EnumTipoLogSeguranca tipoLogSeguranca,
                                      BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional)
        {
            var stackTrace = Environment.StackTrace;
            try
            {
                return AplicacaoSnebur.Atual.ServicoSeguranca.NotificarLogSeguranca(mensagem,
                                                                                    stackTrace,
                                                                                    infoRequisicao,
                                                                                    tipoLogSeguranca,
                                                                                    informacaoAdicional);
            }
            catch
            {
                return new ServicoLogSegurancaLocal().NotificarLogSeguranca(mensagem,
                                                                            stackTrace,
                                                                            infoRequisicao,
                                                                            tipoLogSeguranca,
                                                                            informacaoAdicional);
            }
        }

        public static void LogAsync(string mensagem)
        {
            var informacaoAdicional = ServicoCompartilhadoUtil.RetornarInformacaoAdicionalServicoCompartilhado();
            ThreadUtil.ExecutarAsync((Action)(() =>
            {
                Log(mensagem, informacaoAdicional, AplicacaoSnebur.Atual.AtivarLogServicoOnline);
            }), true);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void LogAsync(string mensagem, bool salvarLogLocal)
        {
            var informacaoAdicional = ServicoCompartilhadoUtil.RetornarInformacaoAdicionalServicoCompartilhado();
            ThreadUtil.ExecutarAsync((() =>
            {
                Log(mensagem, informacaoAdicional, salvarLogLocal);

            }), true);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Guid Log(string mensagem, BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional)
        {
            return Log(mensagem, informacaoAdicional, AplicacaoSnebur.Atual.AtivarLogServicoOnline);
        }
      
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Guid Log(string mensagem, BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional, bool salvarLogLocal)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                LogWindows(mensagem, String.Empty, EventLogEntryType.Information);
            }
            try
            {
                if (!salvarLogLocal)
                {
                    return AplicacaoSnebur.Atual.ServicoLogAplicacao.NotificarLogAplicacao(mensagem, informacaoAdicional);
                }
                else
                {
                    return new ServicoLogAplicacaoLocal().NotificarLogAplicacao(mensagem, informacaoAdicional);
                }
            }
            catch
            {
                if (!(AplicacaoSnebur.Atual.ServicoLogAplicacao is ServicoLogAplicacaoLocal))
                {
                    return new ServicoLogAplicacaoLocal().NotificarLogAplicacao(mensagem, informacaoAdicional);
                }
            }
            return Guid.Empty;
        }

        public static void DesempenhoAsync(string mensagem, Stopwatch tempo, EnumTipoLogDesempenho tipo, bool erroIsAttach = true, Action<Guid> callback = null)
        {
            var informacaoAdicional = ServicoCompartilhadoUtil.RetornarInformacaoAdicionalServicoCompartilhado();
            if (DebugUtil.IsAttached && erroIsAttach)
            {
                return;
            }
            ThreadUtil.ExecutarAsync((Action)(() =>
            {
                var identificadorLog = Desempenho(mensagem, tipo, informacaoAdicional);
                callback?.Invoke(identificadorLog);
            }), true);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        private static Guid Desempenho(string mensagem, EnumTipoLogDesempenho tipo, BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional)
        {
            var stackTrace = Environment.StackTrace;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                LogWindows(mensagem, stackTrace, EventLogEntryType.Warning);
            }
            try
            {
                return AplicacaoSnebur.Atual.ServicoDesempenho.NotificarLogDesempenho(mensagem, stackTrace, tipo, informacaoAdicional);
            }
            catch
            {
                return new ServicoLogDesempenhoLocal().NotificarLogDesempenho(mensagem, stackTrace, tipo, informacaoAdicional);
            }
        }
        //public static Guid Desempenho()
        //{

        //}

        #endregion region

        #region Métodos internos

        internal static void LogAplicacaoAtivaAsync()
        {
            ThreadUtil.ExecutarAsync(LogAplicacaoAtiva, true);
        }

        internal static void LogAplicacaoAtiva()
        {
            var informacaoAdicional = ServicoCompartilhadoUtil.RetornarInformacaoAdicionalServicoCompartilhado();
            AplicacaoSnebur.Atual.ServicoLogAplicacao.NotificarAplicacaoAtiva(informacaoAdicional);
        }

        internal static void AtivarLogServicoOnlineAsync(Action<bool> callback)
        {
            ThreadUtil.ExecutarAsync((Action)(() =>
            {
                AtivarLogServicoOnline(callback);
            }), true);
        }

        private static void AtivarLogServicoOnline(Action<bool> callback)
        {
            try
            {
                var informacaoAdicional = ServicoCompartilhadoUtil.RetornarInformacaoAdicionalServicoCompartilhado();
                var resultado = AplicacaoSnebur.Atual.ServicoLogAplicacao.AtivarLogServicoOnline(informacaoAdicional);
                callback(resultado);
            }
            catch
            {
                callback(false);
            }
        }
        #endregion

        private const string ESPACO_LOG_SNEBUR = "Snebur";
        private const string ESPACO_LOG_ZSEGURO = "ZSeguro";

        public static bool CriarEspacoSneburVisualizadorEventos()
        {
            try
            {
                CriarEspaco(ESPACO_LOG_SNEBUR);
                CriarEspaco(ESPACO_LOG_ZSEGURO);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void LogWindows(string mensagem, string stackTrace, EventLogEntryType tipo)
        {
            var conteudo = mensagem + "\n" + stackTrace;
            LogWindowsInterno(ESPACO_LOG_SNEBUR, conteudo, tipo);
        }

        private static void LogWindowsSeguranca(string mensagem, EnumTipoLogSeguranca tipoSeguranca)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var conteudo = EnumUtil.RetornarDescricao(tipoSeguranca) + "\n" + mensagem;
                LogWindowsInterno(ESPACO_LOG_ZSEGURO, conteudo, EventLogEntryType.FailureAudit);
            }
        }

        private static void LogWindowsInterno(string fonte,
                                              string conteudo,
                                              EventLogEntryType tipo)
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    var nomeEspaco = RetornarNomeEspaco(fonte);
                    using (var eventLog = new EventLog(nomeEspaco))
                    {
                        eventLog.Source = nomeEspaco;
                        eventLog.WriteEntry(conteudo, tipo);
                    }
                }
                catch
                {
                }
            }

        }

        private static string RetornarNomeEspaco(string nomeEspaco)
        {
            return "Application";
            //try
            //{
            //    CriarEspaco(nomeEspaco);
            //    return nomeEspaco;
            //}
            //catch
            //{
            //    return "Application";
            //}
        }

        private static void CriarEspaco(string nomeEspaco)
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                EventSourceCreationData eventSourceData = new EventSourceCreationData(nomeEspaco, nomeEspaco);
                if (!EventLog.Exists(nomeEspaco))
                {
                    EventLog.CreateEventSource(eventSourceData);
                }
            }
        }
    }
}

