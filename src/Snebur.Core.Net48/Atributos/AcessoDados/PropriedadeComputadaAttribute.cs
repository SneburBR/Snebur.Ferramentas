using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class BasePropriedadeComputadaAttribute : BaseAtributoDominio
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropriedadeComputadaBancoAttribute : BaseAtributoDominio
    {

    }

    //[AttributeUsage(AttributeTargets.Property)]
    //public abstract class PropriedadeComputadaServicoAttribute : BaseAtributoDominio
    //{
    //}

    //[AttributeUsage(AttributeTargets.Property)]
    //public class PropriedadeComputadoAttribute : BaseAtributoDominio
    //{
    //}
}