using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Snebur.Utilidade
{
    public static class XmlUtil
    {
        //
        public static T Deserializar<T>(string xml)
        {
            using (var sr = new StringReader(xml))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(sr);
            }
        }

        public static string Serializar(object obj, Encoding encoding)
        {
            using (var preparar = new PrepararSerializacao())
            using (var ms = new MemoryStream())
            {
                preparar.Preparar(obj);

                using (var sw = new StreamWriter(ms, encoding))
                {
                    var xmlSerializer = new XmlSerializer(obj.GetType());
                    xmlSerializer.Serialize(sw, obj);

                    ms.Position = 0;

                    using (var sr = new StreamReader(ms, encoding))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        private class PrepararSerializacao : IDisposable
        {
            private HashSet<object> ObjetosAnalisados = new HashSet<object>();

            public void Preparar(object objeto)
            {
                if (this.ObjetosAnalisados.Contains(objeto))
                {
                    return;
                }
                this.ObjetosAnalisados.Add(objeto);

                var type = objeto.GetType();
                var properties = type.GetProperties().Where(x => x.GetGetMethod().IsPublic && x.GetSetMethod() != null && x.GetSetMethod().IsPublic).ToList();
                if (properties.Count > 0)
                {
                    foreach (var propertyInfo in properties)
                    {
                        if (propertyInfo.PropertyType.Equals(typeof(decimal)))
                        {
                            var value = (decimal)propertyInfo.GetValue(objeto);
                            var formattedString = value.ToString("0.00", CultureInfo.InvariantCulture);
                            propertyInfo.SetValue(objeto, decimal.Parse(formattedString, CultureInfo.InvariantCulture), null);
                        }
                        else
                        {
                            if (objeto is IEnumerable lista)
                            {
                                foreach (var item in lista)
                                {
                                    if (item != null && !item.GetType().IsValueType && item.GetType() != typeof(string))
                                    {
                                        this.Preparar(item);
                                    }
                                }
                            }
                            else
                            {
                                if (!propertyInfo.PropertyType.IsValueType && propertyInfo.PropertyType != typeof(string))
                                {
                                    var valor = (object)propertyInfo.GetValue(objeto);
                                    if (valor != null)
                                    {
                                        this.Preparar(valor);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public void Dispose()
            {
                this.ObjetosAnalisados?.Clear();
                this.ObjetosAnalisados = null;
            }
        }
    }
}