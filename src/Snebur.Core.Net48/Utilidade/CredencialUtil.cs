using Snebur.Dominio;
using System;

namespace Snebur.Utilidade
{
    public class CredencialUtil
    {
        public static bool ValidarCredencial(ICredencial credencial1, ICredencial credencial2)
        {
            if (credencial1 != null && credencial2 != null && credencial1.IdentificadorUsuario != null)
            {
                return credencial1.IdentificadorUsuario.Equals(credencial2.IdentificadorUsuario, StringComparison.InvariantCultureIgnoreCase) &&
                   credencial1.Senha == credencial2.Senha;
            }
            return false;

        }
        //public static Credencial RetornarCredencial(string caminhoArquivo)
        //{
        //    var conteudo = File.ReadAllText(caminhoArquivo, Encoding.UTF8);
        //    var json = CriptografiaUtil.Descriptografar(CHAVE_CRIPTOGRAFIA, conteudo);
        //    return JsonUtil.Deserializar<CredencialUsuario>(json);
        //}

        //public static Credencial SalvarCredencial(string identificador, Credencial credencial)
        //{

        //}
    }
}