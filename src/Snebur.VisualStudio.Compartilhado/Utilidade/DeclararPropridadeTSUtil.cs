namespace Snebur.VisualStudio
{
    public static class DeclararPropridadeTSUtil
    {
        private const string NOME_TIPO = "_NOME_TIPO_";
        public static string RetornarMelhorNomeTipoDaPropriedadeTypescript(string nomePropriedade, bool isConstrutor = false)
        {
            nomePropriedade = nomePropriedade.ToLower();
            if (nomePropriedade.StartsWith("is"))
            {
                return isConstrutor ? "Boolean" : "boolean";
            }

            if (nomePropriedade.StartsWith("nome") ||
                nomePropriedade.StartsWith("descricao") ||
                nomePropriedade.StartsWith("name") ||
                nomePropriedade.StartsWith("rotulo") ||
                nomePropriedade.StartsWith("legenda") ||
                nomePropriedade.StartsWith("description"))
            {
                return isConstrutor ? "String" : "string";
            }

            if (nomePropriedade.StartsWith("total") ||
                nomePropriedade.StartsWith("count") ||
                nomePropriedade.StartsWith("valor") ||
                nomePropriedade.StartsWith("Id") ||
                nomePropriedade.EndsWith("_Id") ||
                nomePropriedade.StartsWith("preco"))
            {
                return isConstrutor ? "Number" : "number";
            }

            if (nomePropriedade.StartsWith("Data"))
            {
                return "Date";
            }

            return NOME_TIPO;
        }
    }
}
