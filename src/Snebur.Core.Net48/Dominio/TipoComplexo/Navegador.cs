using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    //public class Navegador : BaseTipoComplexo<Navegador>
    public class Navegador : BaseTipoComplexo
    {
        private EnumNavegador _navegadorEnum;
        private string _nome;
        private string _codenome;
        private string _versao;

        public EnumNavegador NavegadorEnum { get => this._navegadorEnum; set => this.NotificarValorPropriedadeAlterada(this._navegadorEnum, this._navegadorEnum = value); }

        [ValidacaoTextoTamanho(255)]
        public string Nome { get => this._nome; set => this.NotificarValorPropriedadeAlterada(this._nome, this._nome = value); }

        [ValidacaoTextoTamanho(255)]
        public string Codenome { get => this._codenome; set => this.NotificarValorPropriedadeAlterada(this._codenome, this._codenome = value); }

        [ValidacaoTextoTamanho(255)]
        public string Versao { get => this._versao; set => this.NotificarValorPropriedadeAlterada(this._versao, this._versao = value); }

        //public override Navegador Clone()
        //{
        //    throw new NotImplementedException();
        //}

        public Navegador() : base()
        {
        }

        public Navegador(EnumNavegador navegadorEnum, string nome, string codenome, string versao) : base()
        {
            this._navegadorEnum = navegadorEnum;
            this._nome = nome;
            this._codenome = codenome;
            this._versao = versao;
        }
        protected internal override BaseTipoComplexo BaseClone()
        {
            return new Navegador(this.NavegadorEnum, this.Nome, this.Codenome, this.Versao);
        }
    }
}