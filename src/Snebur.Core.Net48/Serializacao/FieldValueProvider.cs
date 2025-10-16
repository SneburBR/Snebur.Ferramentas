using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Snebur.Serializacao
{
    internal class FieldValueProvider : IValueProvider
    {
        private FieldInfo field;

        public FieldValueProvider(FieldInfo field)
        {
            this.field = field;
        }

        public void SetValue(object target, object value)
        {
            this.field.SetValue(target, value);
        }

        public object GetValue(object target)
        {
            return this.field.GetValue(target);
        }
    }
}
