using Snebur.Servicos;

namespace Snebur.Utilidade
{
    public static class SegurancaUtil
    {
        public static bool IsGerarLogErro(EnumTipoLogSeguranca tipo)
        {
            switch (tipo)
            {
                case EnumTipoLogSeguranca.TentativaAutenticacao:
                case EnumTipoLogSeguranca.IdentificadorProprietarioInvalido:
                case EnumTipoLogSeguranca.TipoIdentificadorAmigavelDesconhecido:
                    return true;
                default:
                    return false;
            }
        }
    }
}
