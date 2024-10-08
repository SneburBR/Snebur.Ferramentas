﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Community.VisualStudio.Toolkit;
using Snebur.Comunicacao.WebSocket.Experimental;
using Snebur.Comunicacao.WebSocket.Experimental.Classes;
using Snebur.Depuracao;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public class ServicoDepuracao
    {
        private const string NOME_ARQUIVO_ÎNFO_PORTA = ConstantesDeburacao.PARAMETRO_VS_DEPURACAO_PORTA + ".info";

        private WebSocketServer ServidorWebSocket;
        public ushort Porta { get; private set; }

        public event EventHandler<MensagemEventArgs<MensagemLog>> EventoLog;

        private ConcurrentDictionary<string, SessaoConectada> SessoesConectada { get; } = new ConcurrentDictionary<string, SessaoConectada>();

        public EnumStatusServicoDepuracao Status { get; set; }

        public ServicoDepuracao()
        {
            this.Porta = ConfiguracaoGeral.Instance.PortaDepuracao;
            //this.Iniciar();
        }

        public async Task IniciarAsync()
        {
            try
            {
                await OutputWindow.OcuparAsync();
                if (ConfiguracaoGeral.Instance.IsUtilizarPortaDepuradaRandomica)
                {
                    this.Porta = (ushort)(new Random().Next(1, UInt16.MaxValue));
                }

                LogVSUtil.Log($"Iniciando servidor de WebSocket: Porta {this.Porta}");

                if (this.ServidorWebSocket != null && this.Status == EnumStatusServicoDepuracao.Ativo)
                {
                    LogVSUtil.LogErro("O servidor de web socket já está ativo");
                    return;
                }

                ConfiguracaoVSUtil.PortaDepuracao = this.Porta;
                var servicoWebSocket = new WebSocketServer(System.Net.IPAddress.Any, this.Porta)
                {
                    OnReceive = this.ServicoWebSocket_ReceberMensagem,
                    OnSend = this.ServicoWebSocket_MensagemEnviada,
                    OnConnected = this.ServicoWebSocketTeste_SessaoConectado,
                    OnDisconnect = this.ServicoWebSocketTeste_SessaoDesconetado,
                    TimeOut = new TimeSpan(0, 5, 0),
                };
                servicoWebSocket.Inicializar();
                this.ServidorWebSocket = servicoWebSocket;
                this.Status = EnumStatusServicoDepuracao.Ativo;
                await this.SalvarPortaAsync();
                LogVSUtil.Sucesso($"Serviço de depuração inicializado: Porta {this.Porta}", null);

                await OutputWindow.AtualizarStatusServicoDepuracaoAsync();
            }
            catch (Exception ex)
            {

                this.ServidorWebSocket = null;
                LogVSUtil.LogErro($"Não possível iniciar o servidor web socket na porta {this.Porta}, o serviço deve está ativo em outra instancia do visual studio");
                LogVSUtil.LogErro($"Erro: {ex.Message}");
                this.Status = EnumStatusServicoDepuracao.Parado;
                this.Porta += 1;

            }
            finally
            {
                await OutputWindow.DesocuparAsync();
            }
        }

        private void ServicoWebSocketTeste_SessaoConectado(SessaoContexto sessaoContexto)
        {
            if (!this.SessoesConectada.ContainsKey(sessaoContexto.Identificador))
            {
                var novaSessaoConectada = new SessaoConectada(sessaoContexto);
                if (this.SessoesConectada.TryAdd(sessaoContexto.Identificador, novaSessaoConectada))
                {
                    novaSessaoConectada.Inicializar();
                    LogVSUtil.Sucesso($"A sessão de depuração {sessaoContexto.Identificador} conectada com sucesso", null);
                }
            }
        }

        private void ServicoWebSocketTeste_SessaoDesconetado(SessaoContexto sessaoContexto)
        {
            if (this.SessoesConectada.ContainsKey(sessaoContexto.Identificador))
            {
                if (this.SessoesConectada.TryRemove(sessaoContexto.Identificador, out SessaoConectada sessaoConectada))
                {
                    sessaoConectada.Dispose();
                    LogVSUtil.Alerta($"A sessão de depuração {sessaoContexto.Identificador} foi desconectada");
                }
            }
        }

        private void ServicoWebSocket_MensagemEnviada(SessaoContexto sessaoContexto)
        {
            //LogVSUtil.Log("Mensagem para: " + sessaoContexto.Identificador + "] - " + sessaoContexto.DataFrame.ToString());
        }

        private void ServicoWebSocket_ReceberMensagem(SessaoContexto sessaoContexto)
        {
            try
            {
                var contratoSerializado = sessaoContexto.DataFrame.ToString();
                if (contratoSerializado.StartsWith("{"))
                {
                    if (this.SessoesConectada.TryGetValue(sessaoContexto.Identificador, out SessaoConectada sessaoConectada))
                    {
                        var contrato = JsonUtil.Deserializar<Contrato>(contratoSerializado, EnumTipoSerializacao.DotNet);
                        var mensagem = contrato.Mensagem;
                        this.ProcessarMensagemRecebida(sessaoConectada, mensagem);
                        LogVSUtil.Log("Mensagem de: " + sessaoContexto.Identificador + "] - " + mensagem.GetType().Name);
                    }
                }

            }
            catch
            {
                //LogVSUtil.LogErro(ex);
            }
        }

        #region Salvar porta para os projetos
        public async Task SalvarPortaAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var projetosVS = await VS.Solutions.GetAllProjectsAsync(ProjectStateFilter.Loaded);
            foreach (var projetoVS in projetosVS)
            {
                var caminhoProjeto = Path.GetDirectoryName(projetoVS.FullPath);
                await this.SalvarPortaAsync(caminhoProjeto);
            }
        }
        private Task SalvarPortaAsync(string caminhoProjeto)
        {
            var porta = this.Porta;
            if (!this.IsPrecisaSalvarPorta(caminhoProjeto, porta))
            {
                return Task.CompletedTask;
            }

            return Task.Run(() =>
            {
                if (this.Status == EnumStatusServicoDepuracao.Ativo)
                {
                    var caminhoTSConfig = Path.Combine(caminhoProjeto, "tsconfig.json");
                    if (File.Exists(caminhoTSConfig))
                    {
                        var caminhoArquivo = Path.Combine(caminhoProjeto, NOME_ARQUIVO_ÎNFO_PORTA);
                        try
                        {

                            ArquivoUtil.DeletarArquivo(caminhoArquivo, false, true);
                            File.WriteAllText(caminhoArquivo, porta.ToString(), Encoding.UTF8);
                           
                            if (this._projetosPorta.TryGetValue(caminhoProjeto, out ushort portaAtua))
                            {
                                this._projetosPorta.TryUpdate(caminhoArquivo, porta, portaAtua);

                                return;
                            }
                            this._projetosPorta.TryAdd(caminhoProjeto, porta);
                            
                        }
                        catch (Exception ex)
                        {
                            LogVSUtil.LogErro($"Não foi possível salvar a porta da depuração {caminhoArquivo}", ex);
                        }
                    }
                }
            } );

        }

        private readonly ConcurrentDictionary<string, ushort> _projetosPorta = new ConcurrentDictionary<string, ushort>();
        private bool IsPrecisaSalvarPorta(string caminhoProjeto, ushort porta)
        {
            if (this._projetosPorta.TryGetValue(caminhoProjeto, out ushort portaAtual))
            {
                return portaAtual != porta;
            }
            return true;

        }

        #endregion

        internal void EnviarMensagemParaTodos(Mensagem mensagem)
        {
            if (this.Status == EnumStatusServicoDepuracao.Ativo)
            {
                var conexoes = this.SessoesConectada.Values.ToList<SessaoConectada>();
                if (conexoes.Count == 0)
                {
                    LogVSUtil.Log($"Nenhuma conexão ativa: mensagem {mensagem.GetType().Name} ignorada");
                }
                else
                {

                    foreach (var sessaoConecta in conexoes)
                    {
                        sessaoConecta.EnviarMensagem(mensagem);
                    }
                }
            }

        }

        internal void ProcessarMensagemRecebida(SessaoConectada sessaoConectada, Mensagem mensagem)
        {
            switch (mensagem)
            {
                case MensagemPing mensagemPing:

                    break;
                case MensagemLog mensagemLog:

                    var args = new MensagemEventArgs<MensagemLog>(sessaoConectada, mensagemLog);
                    this.EventoLog?.Invoke(this, args);


                    break;

                default:

                    throw new NotImplementedException();
            }
        }

        internal void Parar()
        {
            LogVSUtil.Log("Parando servidor de web socket");
            this.ServidorWebSocket?.Parar();
            this.ServidorWebSocket?.Dispose();
            this.ServidorWebSocket = null;
            this.Status = EnumStatusServicoDepuracao.Parado;
        }

        //internal void Reiniciar()
        //{
        //    this.Parar();
        //    this.Porta = PORTA_PADRAO;
        //    this.Iniciar();
        //}
    }

    public class SessaoConectada : IDisposable
    {
        private readonly System.Timers.Timer PingTimer;
        public SessaoContexto Sessao { get; }

        public SessaoConectada(SessaoContexto sessao)
        {
            this.PingTimer = new System.Timers.Timer(30000);
            this.Sessao = sessao;
        }
        public void Inicializar()
        {
            this.PingTimer.Elapsed += this.Timer_Elapsed;
            this.PingTimer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Ping();
        }

        private static object bloqueio = new object();

        private void Ping()
        {
            var menagemPing = new MensagemPing();
            this.EnviarMensagem(menagemPing);
        }

        public void EnviarMensagem(Mensagem mensagem)
        {
            lock (bloqueio)
            {
                try
                {
                    LogVSUtil.Log($"Mensagem {mensagem.GetType().Name} para  {this.Sessao.Identificador}");

                    var contrato = new Contrato(mensagem);
                    var constratoSerializado = JsonUtil.Serializar(contrato, EnumTipoSerializacao.Javascript);
                    this.Sessao.Send(constratoSerializado);
                }
                catch (Exception ex)
                {
                    LogVSUtil.Log("depuração, enviar mensagem : " + ex.Message);
                }
            }
        }

        public void Dispose()
        {
            this.PingTimer.Dispose();
        }
    }

    public enum EnumStatusServicoDepuracao
    {
        Ativo = 1,
        Parado = 2,
    }


}
