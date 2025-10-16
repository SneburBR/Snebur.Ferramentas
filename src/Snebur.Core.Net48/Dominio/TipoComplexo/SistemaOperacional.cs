using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    //public class SistemaOperacional : BaseTipoComplexo<SistemaOperacional>
    public class SistemaOperacional : BaseTipoComplexo
    {
        private EnumSistemaOperacional _sistemaOperacionalEnum;
        private string _nome;
        private string _codenome;
        private string _versao;

        public EnumSistemaOperacional SistemaOperacionalEnum { get => this._sistemaOperacionalEnum; set => this.NotificarValorPropriedadeAlterada(this._sistemaOperacionalEnum, this._sistemaOperacionalEnum = value); }

        [ValidacaoTextoTamanho(255)]
        public string Nome { get => this._nome; set => this.NotificarValorPropriedadeAlterada(this._nome, this._nome = value); }

        [ValidacaoTextoTamanho(255)]
        public string Codenome { get => this._codenome; set => this.NotificarValorPropriedadeAlterada(this._codenome, this._codenome = value); }

        [ValidacaoTextoTamanho(255)]
        public string Versao { get => this._versao; set => this.NotificarValorPropriedadeAlterada(this._versao, this._versao = value); }

        public SistemaOperacional() : base()
        {

        }
        public SistemaOperacional(EnumSistemaOperacional sistemaOperacionalEnum, string nome, string codenome, string versao) : base()
        {
            this._sistemaOperacionalEnum = sistemaOperacionalEnum;
            this._nome = nome;
            this._codenome = codenome;
            this._versao = versao;
        }

        //public override SistemaOperacional Clone()
        //{
        //    throw new NotImplementedException();
        //}

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new SistemaOperacional(this.SistemaOperacionalEnum, this.Nome, this.Codenome, this.Versao);
        }
    }
}