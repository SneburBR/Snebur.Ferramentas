using Snebur.Dominio.Atributos;

namespace Snebur.UI
{
    public enum EnumCor
    {
        [Rotulo("Vazio")]
        Vazio = BaseEnumApresentacao.Vazio,

        [Rotulo("Transparente")]
        Transparente = 0,

        [Rotulo("Padrão")]
        SistemaPadrao = 10,

        [Rotulo("Principal")]
        SistemaPrincipal = 20,

        [Rotulo("Secundária")]
        SistemaSecundaria = 30,

        [Rotulo("Sucesso")]
        SistemaSucesso = 40,

        [Rotulo("Falha")]
        SistemaFalha = 50,

        [Rotulo("Informação")]
        SistemaInformacao = 55,

        [Rotulo("Alerta")]
        SistemaAlerta = 60,

        [Rotulo("Editar")]
        SistemaEditar = 70,

        [Rotulo("Salvar")]
        SistemaSalvar = 80,

        [Rotulo("Novo")]
        SistemaNovo = 85,

        [Rotulo("Excluir")]
        SistemaExcluir = 90,

        [Rotulo("Vermelha")]
        Vermelha = 100,

        [Rotulo("Rosa")]
        Rosa = 110,

        [Rotulo("Roxo")]
        Roxo = 120,

        [Rotulo("RoxoClaro")]
        RoxoClaro = 125,

        [Rotulo("RoxoEscuro")]
        RoxoEscuro = 130,

        [Rotulo("Indigo")]
        Indigo = 140,

        [Rotulo("Azul")]
        Azul = 150,

        [Rotulo("AzulClaro")]
        AzulClaro = 160,

        [Rotulo("Rotulo")]
        Ciano = 170,

        [Rotulo("Turquesa")]
        Turquesa = 180,

        [Rotulo("Verde")]
        Verde = 190,

        [Rotulo("VerdeClaro")]
        VerdeClaro = 200,

        [Rotulo("Verde lima")]
        VerdeLima = 210,

        [Rotulo("Amarelo")]
        Amarelo = 220,

        [Rotulo("AmareloMostarda")]
        AmareloMostarda = 230,

        [Rotulo("Laranja")]
        Laranja = 240,

        [Rotulo("Laranja escuro")]
        LaranjaEscuro = 250,

        [Rotulo("Marrom")]
        Marron = 260,

        [Rotulo("Cinza")]
        Cinza = 270,

        [Rotulo("Cinza azulado")]
        CinzaAzulado = 280,

        [Rotulo("Branca")]
        Branca = 290,

        [Rotulo("Preta")]
        Preta = 305
    }
}