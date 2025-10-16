using Snebur.UI;
using System;
using System.Threading;

namespace Snebur.Utilidade
{
    public static class InternetUtil
    {
        private const int INTERVALO_MENSAGEM = 30;
        private static bool _mensagemAtiva = false;
        private static object _bloqueio = new object();
        //private static DateTime? _dataHoraUltimaVisualizacao;
        private static IJanelaAlerta Alerta;

        private const int TEMPO_ESPERAR = 10;

        public static void AguardarRestabelecerInternet()
        {
            if (!RedeUtil.InternetConectada())
            {
                while (!RedeUtil.InternetConectada())
                {
                    MostrarMensagemSemInternet();
                    Thread.Sleep((int)TimeSpan.FromSeconds(TEMPO_ESPERAR).TotalMilliseconds);
                }
                FecharMensagemSemInternet();
            }
        }

        public static void MostrarMensagemSemInternet()
        {
            if (AplicacaoSnebur.Atual.Alerta != null)
            {
                if (IsMostrarMensagem())
                {
                    //InternetUtil._dataHoraUltimaVisualizacao = DateTime.Now;

                    if (!_mensagemAtiva)
                    {
                        lock (_bloqueio)
                        {
                            if (!_mensagemAtiva)
                            {
                                _mensagemAtiva = true;

                                ThreadUtil.ExecutarMainThread(() =>
                                {
                                    var titulo = "Sem conexão com a internet";
                                    var nomeProduto = AplicacaoSnebur.Atual.NomeAplicacao;
                                    var conteudo = String.Format("Não foi possível conectar o {0} com a internet.\nPor gentileza, verifique sua conexão.\nApós reestabelecer a conexão o sistema voltará automaticamente.", nomeProduto);

                                    Alerta = AplicacaoSnebur.Atual.Alerta.Mostrar(conteudo,
                                                                                                 titulo,
                                                                                                 EnumTipoAlerta.Atencao,
                                                                                                 EnumBotoesAlerta.Nenhum, (resultado) =>
                                                                                                 {
                                                                                                     //InternetUtil._dataHoraUltimaVisualizacao = DateTime.Now;
                                                                                                     Thread.Sleep(100);
                                                                                                     _mensagemAtiva = false;
                                                                                                     Alerta = null;
                                                                                                 });
                                });
                            }
                        }
                    }

                }
            }
        }

        private static bool IsMostrarMensagem()
        {
            lock (_bloqueio)
            {
                if (_mensagemAtiva)
                {
                    return false;
                }
                //if (InternetUtil._dataHoraUltimaVisualizacao.HasValue)
                //{
                //    var intervalo = DateTime.Now.TimeOfDay - InternetUtil._dataHoraUltimaVisualizacao.Value.TimeOfDay;
                //    if (Math.Abs(intervalo.TotalSeconds) < INTERVALO_MENSAGEM)
                //    {
                //        return false;
                //    }
                //}
                return true;
            }

            //return !RedeUtil.InternetConectada();
        }

        public static void FecharMensagemSemInternet()
        {
            lock (_bloqueio)
            {
                if (_mensagemAtiva && Alerta != null)
                {

                    ThreadUtil.ExecutarMainThread(() =>
                    {
                        Alerta.Fechar();
                    });
                }
            }
        }
    }
}