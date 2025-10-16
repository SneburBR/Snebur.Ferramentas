namespace Snebur.Utilidade
{
    public static class NormalizacaoUtil
    {
        public static string NormalizarAspasDuplas(string valor, bool apenasSeTiverEspacos = false)
        {
            if (apenasSeTiverEspacos && !ValidacaoUtil.IsPossuiEspacoEmBranco(valor))
            {
                return valor;
            }

            if (valor == null)
            {
                return "\"\"";
            }

            if (valor.StartsWith("\"") && valor.EndsWith("\""))
            {
                return valor;
            }

            else if (valor.StartsWith("\""))
            {
                return $"{valor}\"";
            }
            else if (valor.EndsWith("\""))
            {
                return $"\"{valor}";
            }
            else
            {
                return $"\"{valor}\"";
            }
        }
        public static string NormlizarHost(string _host)
        {
            var posicao = _host.IndexOf(':');
            if (posicao >= 0)
            {
                _host = _host.Substring(posicao + 1);
                posicao = _host.IndexOf(':');
                if (posicao > 0)
                {
                    _host = _host.Substring(0, posicao);
                }
            }
            return _host?.ToLower();
        }

        public static string NormalizarNomeParametro(string parametro)
        {
            if (parametro.StartsWith("_"))
            {
                return parametro.Substring(1);
            }
            return parametro;
        }
    }
}
