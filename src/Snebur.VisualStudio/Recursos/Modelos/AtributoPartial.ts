
namespace $namespace
{
    export interface $nomeAtributo
    {
        RetornarMensagemValidacao(propriedade: Snebur.Reflexao.Propriedade, valorPropriedade: any): string;

        IsValido(paiPropriedade: Snebur.Dominio.BaseDominio, propriedade: Snebur.Reflexao.Propriedade, valorPropriedade: any): boolean;

    }

    $nomeAtributo.prototype.RetornarMensagemValidacao = function (propriedade: Snebur.Reflexao.Propriedade, valorPropriedade: any): string
    {
        throw new Error("Não implementado");
    }

    $nomeAtributo.prototype.IsValido = function (paiPropriedade: Snebur.Dominio.BaseDominio, propriedade: Snebur.Reflexao.Propriedade, valorPropriedade: any): boolean
    {
        throw new Error("Não implementado");
    }
}

