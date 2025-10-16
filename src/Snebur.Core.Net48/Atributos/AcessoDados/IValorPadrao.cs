namespace Snebur.Dominio.Atributos
{

    public interface IBaseValorPadrao 
    {
        bool IsTipoNullableRequerido { get; }
        bool IsValorPadraoOnUpdate { get; }
    }

    [IgnorarInterfaceTS]
    public interface IConverterValorPadrao
    {
        object RetornarValorPadrao(object contexto,
                                   Entidade entidadeCorrente,
                                   object valorPropriedade);
    }

    [IgnorarInterfaceTS]
    public interface IValorPadrao : IConverterValorPadrao, IBaseValorPadrao
    {
     
    }

 
}