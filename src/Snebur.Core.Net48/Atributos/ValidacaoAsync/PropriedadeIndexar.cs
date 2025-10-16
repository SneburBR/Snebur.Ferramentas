using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    public class PropriedadeIndexar
    {
        public PropertyInfo Propriedade { get; }
        public bool IsIgnorarNulo { get; }
        public bool IsIgrnorarZero { get; }
        public string NomeIndice { get; }

        public PropriedadeIndexar(PropertyInfo propriedade, string nomeIndice) : this(propriedade, false, false)
        {
            this.NomeIndice = nomeIndice;
        }

        public PropriedadeIndexar(PropertyInfo propriedade,
                                  bool isIgnorarNulo,
                                  bool isIgnorarZero)
        {
            this.Propriedade = propriedade;

            this.IsIgnorarNulo = isIgnorarNulo;
            this.IsIgrnorarZero = isIgnorarZero;
        }

        public PropriedadeIndexar(PropertyInfo propriedade,
                                  ValidacaoUnicoAttribute validacaoUnicoAttribute)
        {

        }
    }
}