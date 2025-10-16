using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;

namespace Snebur.Seguranca
{
    [Plural("Credenciais")]
    public abstract class Credencial : BaseDominio, ICredencial
    {
        public string IdentificadorUsuario { get; set; }

        public string Senha { get; set; }

        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public bool IsAnonimo => Util.SaoIgual(this.IdentificadorUsuario, CredencialAnonimo.Anonimo.IdentificadorUsuario);

        public Credencial()
        {
        }

        [IgnorarConstrutorTS]
        public Credencial(string identificadorUsuario, string senha)
        {
            this.IdentificadorUsuario = identificadorUsuario;
            this.Senha = senha;
        }
 
        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if ((obj is Credencial) || (ReflexaoUtil.IsTipoImplementaInterface(obj.GetType(), typeof(ICredencial), false)))
                {
                    var credencialValidar = (ICredencial)obj;
                    return this.Validar(credencialValidar);
                }
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Validar(ICredencial credencial)
        {
            return this.Validar(credencial.IdentificadorUsuario, credencial.Senha);
        }

        public bool Validar(string identificadorUsuario, string senha)
        {
            if (this.IdentificadorUsuario != null &&
                this.IdentificadorUsuario.Equals(identificadorUsuario,
                StringComparison.InvariantCultureIgnoreCase))
            {
                if (this.Senha.Equals(senha, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                return Md5Util.RetornarHash(senha) == this.Senha.ToLower() ||
                       Md5Util.RetornarHash(this.Senha) == senha.ToLower();
            }
            return false;
        }
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(this.IdentificadorUsuario) && String.IsNullOrEmpty(this.Senha);
            }
        }
        #region ICredencial

        string ICredencial.IdentificadorUsuario { get => this.IdentificadorUsuario; set => this.IdentificadorUsuario = value; }

        string ICredencial.Senha { get => this.Senha; set => this.Senha = value; }

        #endregion

    }
}