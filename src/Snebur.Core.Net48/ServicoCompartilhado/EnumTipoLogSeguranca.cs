using Snebur.Dominio.Atributos;

namespace Snebur.Servicos
{
    public enum EnumTipoLogSeguranca
    {
        [Rotulo("Token expirado")]
        TokenExpirado = 1,

        [Rotulo("Token invalido")]
        TokenInvalido = 2,

        [Rotulo("Chave do token inválida")]
        TokenChaveInvalida = 3,

        [Rotulo("Credencial do serviço não autorizada")]
        CredencialServicoNaoAutorizada = 4,

        [Rotulo("Credencial do usuario não autorizada")]
        CredencialUsuarioNaoAutorizada = 5,

        [Rotulo("Cabeçalho inválido")]
        CabecalhoInvalido = 6,

        [Rotulo("Comando não autorizado")]
        ComandoNaoAutorizado = 7,

        [Rotulo("Alteração de senha")]
        AlteracaoSenha = 8,

        [Rotulo("Tentativa de autenticação")]
        TentativaAutenticacao = 9,

        [Rotulo("Url não autorizada")]
        UrlNaoAutorizada = 10,

        [Rotulo("O contrato inválido")]
        ContratoInvalido = 11,

        [Rotulo("O método da operação não foi encontrado")]
        MetodoOperacaoNaoEncontrado = 12,

        [Rotulo("Permissão ao acesso da dados")]
        PermissaoAcessoDados = 13,

        [Rotulo("Token data futura fora dos limites")]
        TokenExpiradoDataFuturaUltrapassada = 14,

        [Rotulo("Identificador do proprietario inválido")]
        IdentificadorProprietarioInvalido = 15,

        [Rotulo("Identificador do proprietario global não é autorizado pelo usuario logado")]
        IdentificadorProprietarioGlobalNaoAutorizado = 16,

        [Rotulo("O identificador amigavel do usuario é desconhecido, esperado e-mail ou telefone")]
        TipoIdentificadorAmigavelDesconhecido = 17,

        [Rotulo("Parametros de comunicao invalidos")]
        ParametrosComunicacaoInvalidos = 18,

        [Rotulo("Tentativa de alteração de propriedade somente leitura")]
        AlterarandoPropriedadeSomenteLeitura = 19,

        [Rotulo("Tentativa alteranção entidade somente leitura")]
        AlterarandoEntidadeSomenteLeitura = 20,

        [Rotulo("Identificador da aplicacao não encontrado")]
        IdentificadorAplicacaoNaoEncontrado = 21,

        [Rotulo("O usuario é diferente do usuario da sessão")]
        UsuarioDiferenteSessao = 22,

        OrigemPossivelAtaque = 23,

        AplicacaoNaoAutorizada = 24,

        AcessoArquivoSuspeito = 25,

        ArquivoVersaoPublicacaoNaoEncontrado = 26,

        ServicoNaoEncontrado = 27,

        [Rotulo("Tentativa de executar sql")]
        TentativaExecutarSql = 28,
    }
}
