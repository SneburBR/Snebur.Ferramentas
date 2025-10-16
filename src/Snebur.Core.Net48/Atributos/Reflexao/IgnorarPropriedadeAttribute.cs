using System;
using System.Xml.Serialization;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnorarPropriedadeAttribute : XmlIgnoreAttribute
    {
    }

}