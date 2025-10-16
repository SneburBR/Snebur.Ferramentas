using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;

namespace Snebur.Comunicacao.WebSocket.Experimental.Classes
{
    /// <summary>
    /// What protocols we support
    /// </summary>
    public enum Protocol
    {
        None = -1,
        WebSocketRFC6455 = 0,
        WebSocketHybi00 = 1
    }

    /// <summary>
    /// This class implements a rudimentary HTTP header reading interface.
    /// </summary>
    public class Cabecalho
    {
        /// <summary>
        /// Regular expression to parse http header
        /// </summary>
        public static string Pattern =
            @"^(?<connect>[^\s]+)?\s?(?<path>[^\s]+)?\s?HTTP\/1\.1(.*?)?\r\n" + // HTTP Request
            @"((?<field_name>[^:\r\n]+):(?<field_value>[^\r\n]+)\r\n)+";


        private readonly NameValueCollection _fields = new NameValueCollection();

        public HttpCookieCollection Cookies = new HttpCookieCollection();

        public string Method = String.Empty;

        public Protocol Protocol = Protocol.None;

        public string RequestPath = String.Empty;

        public string[] SubProtocols;


        public Cabecalho(string data)
        {
            // Parse HTTP Header
            var regex = new Regex(Pattern, RegexOptions.IgnoreCase);
            var match = regex.Match(data);
            var matchGroups = match.Groups;
            var fieldNameCollection = matchGroups["field_name"];
            var fieldValueCollection = matchGroups["field_value"];

            if (fieldNameCollection != null && fieldValueCollection != null)
            {
                // run through every match and save them in the handshake object
                for (var i = 0; i < fieldNameCollection.Captures.Count; i++)
                {
                    var name = fieldNameCollection.Captures[i].ToString().ToLower();
                    var value = fieldValueCollection.Captures[i].ToString().Trim();

                    switch (name)
                    {
                        case "cookie":
                            var cookieArray = value.Split(';');
                            foreach (var cookie in cookieArray)
                            {
                                var cookieIndex = cookie.IndexOf('=');

                                if (cookieIndex < 0) continue;

                                var cookieName = cookie.Remove(cookieIndex).TrimStart();

                                var cookieValue = cookie.Substring(cookieIndex + 1);
                                if (cookieName != String.Empty)
                                {
                                    this.Cookies.Add(new HttpCookie(cookieName, cookieValue));
                                }
                            }
                            break;
                        default:
                            this._fields.Add(name, value);
                            break;
                    }
                }
            }

            var pathCollection = matchGroups["path"];
            var methodCollection = matchGroups["connect"];

            if (pathCollection != null)
            {
                if (pathCollection.Captures.Count > 0)
                {
                    this.RequestPath = pathCollection.Captures[0].Value.Trim();
                }
            }

            if (methodCollection != null)
            {
                if (methodCollection.Captures.Count > 0)
                {
                    this.Method = methodCollection.Captures[0].Value.Trim();
                }
            }

            int version;
            Int32.TryParse(this._fields["sec-websocket-version"], out version);

            if (!String.IsNullOrEmpty(this._fields["sec-websocket-protocol"]))
            {
                this.SubProtocols = this._fields["sec-websocket-protocol"].Split(',');
            }

            this.Protocol = version < 8 ? Protocol.WebSocketHybi00 : Protocol.WebSocketRFC6455;
        }

        /// <summary>
        /// Gets or sets the Fields object with the specified key.
        /// </summary>
        public string this[string key]
        {
            get { return this._fields[key]; }
            set { this._fields[key] = value; }
        }
    }
}