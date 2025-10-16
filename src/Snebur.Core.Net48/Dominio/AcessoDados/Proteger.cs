using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
    [IgnorarAtributoTS]
    public class Proteger : Attribute
    {
        public bool MostrarConsultaPelaChavePrimaria { get; set; }

        public string MascaraProtecao { get; set; } = "###*##";

        public int MaximoExibicao { get; set; } = 50;

        public TimeSpan EspacoTempo { get; set; } = TimeSpan.FromHours(1);
    }
}
