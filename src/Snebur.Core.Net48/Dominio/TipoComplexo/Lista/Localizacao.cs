using Snebur.Dominio.Atributos;
using System;
using System.Linq;

namespace Snebur.Dominio
{

    //public class Localizacao : BaseTipoComplexo<Localizacao>
    public class Localizacao : BaseTipoComplexo
    {
        private double _latitude;
        private double _longitude;

        public double Latitude { get => this._latitude; set => this.NotificarValorPropriedadeAlterada(this._latitude, this._latitude = value); }

        public double Longitude { get => this._longitude; set => this.NotificarValorPropriedadeAlterada(this._longitude, this._longitude = value); }

        [IgnorarConstrutorTS]
        public Localizacao()
        {
        }

        public Localizacao(double latitude, double longitude)
        {
            this._latitude = latitude;
            this._longitude = longitude;
        }

        public static Localizacao Empty
        {
            get
            {
                return new Localizacao(0, 0);
            }
        }

        public static Localizacao Parse(string localicacaoString)
        {
            return Parse(localicacaoString, ',');
        }

        public static Localizacao Parse(string localicacaoString, char divisor)
        {
            if (!String.IsNullOrWhiteSpace(localicacaoString))
            {
                var partes = localicacaoString.Split(divisor);
                if (partes.Length == 2)
                {
                    var latituraString = partes.First();
                    var longitudeString = partes.Last();

                    Double.TryParse(latituraString, out double latitude);
                    Double.TryParse(longitudeString, out double longitude);

                    return new Localizacao(latitude, longitude);
                }
            }
            return Empty;
        }

        //public override Localizacao Clone()
        //{
        //    throw new NotImplementedException();
        //}

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new Localizacao(this.Latitude, this.Longitude);
        }
    }
}