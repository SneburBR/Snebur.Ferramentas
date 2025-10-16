using Snebur.Dominio.Atributos;
using System.Linq;

namespace Snebur.Dominio
{
    [IgnorarClasseTS]

    public partial class RedeSociais : BaseTipoComplexo 
    {
        private string _facebook;
        private string _instagram;
        private string _twitter;
        private string _whatsApp;
        private string _linkedIn;
        private string _youtube;
        private string _github;

        [ValidacaoTextoTamanho(255)]
        public string Facebook { get=> this._facebook; set => this.NotificarValorPropriedadeAlterada(this._facebook, this._facebook = value); }
      
        [ValidacaoTextoTamanho(255)]
        public string Instagram { get => this._instagram; set => this.NotificarValorPropriedadeAlterada(this._instagram, this._instagram = value); }
      
        [ValidacaoTextoTamanho(255)]
        public string Twitter { get => this._twitter; set => this.NotificarValorPropriedadeAlterada(this._twitter, this._twitter = value); }
       
        [ValidacaoTextoTamanho(255)]
        public string WhatsApp { get => this._whatsApp; set => this.NotificarValorPropriedadeAlterada(this._whatsApp, this._whatsApp = value); }
       
        [ValidacaoTextoTamanho(255)]
        public string LinkedIn { get => this._linkedIn; set => this.NotificarValorPropriedadeAlterada(this._linkedIn, this._linkedIn = value); }
       
        [ValidacaoTextoTamanho(255)]
        public string Youtube { get => this._youtube; set => this.NotificarValorPropriedadeAlterada(this._youtube, this._youtube = value); }
       
        [ValidacaoTextoTamanho(255)]
        public string Github { get => this._github; set => this.NotificarValorPropriedadeAlterada(this._github, this._github = value); }

        public RedeSociais()
        {
        }

        public override string ToString()
        {
            var partes = new string[] { this.Facebook, this.Instagram, this.Twitter, this.WhatsApp, this.LinkedIn, this.Youtube, this.Github };
            return string.Join(", ", partes.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        public override bool Equals(object obj)
        {
            if (obj is RedeSociais r)
            {
                return this.Facebook == r.Facebook &&
                       this.Instagram == r.Instagram &&
                       this.Github == r.Github &&
                       this.WhatsApp == r.WhatsApp &&
                       this.Youtube == r.Youtube &&
                       this.Twitter == r.Twitter &&
                       this.LinkedIn == r.LinkedIn;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //Problemas ao definir o dpi de impressão
            return base.GetHashCode();
            //return this.Largura.GetHashCode() + this.Altura.GetHashCode();
        }
     

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new RedeSociais
            {
                Facebook = this.Facebook,
                Twitter = this.Twitter,
                Instagram = this.Instagram,
                Youtube = this.Youtube,
                LinkedIn = this.LinkedIn,
                WhatsApp = this.WhatsApp,
                Github = this.Github
            };
        }
    }
}