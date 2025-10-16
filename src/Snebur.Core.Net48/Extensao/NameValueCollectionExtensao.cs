using System.Collections.Specialized;

namespace System 
{
    public static class NameValueCollectionExtensao
    {
        public static string GetValue(this NameValueCollection value,
                                      string chave)
        {
            return value[chave];
        }
    }
}
