using System;
using System.Xml.Serialization;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NaoSerializarAttribute : XmlIgnoreAttribute
    {

    }
}
