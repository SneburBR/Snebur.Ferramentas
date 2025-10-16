using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParametroOpcionalTSAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class PropriedadeOpcionalTSAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class PropriedadeGravacaoTSAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class MetodoOpcionalTSAttribute : Attribute
    {
    }
}