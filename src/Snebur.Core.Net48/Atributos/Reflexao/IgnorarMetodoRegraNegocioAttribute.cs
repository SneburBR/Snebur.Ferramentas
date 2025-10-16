using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnorarMetodoRegraNegocioAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class IgnoraClasseRegraNegocioAttribute : Attribute
    {
    }
}