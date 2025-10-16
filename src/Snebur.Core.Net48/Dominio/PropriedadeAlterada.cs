
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;

namespace Snebur.Dominio
{
    [IgnorarClasseTS]
    public class PropriedadeAlterada : BaseDominio, IPropriedadeAlterada
    {
        public string NomePropriedade { get; set; }
        public object AntigoValor { get; set; }

        public object NovoValor { get; set; }

        [IgnorarConstrutorTS]
        public PropriedadeAlterada()
        {

        }

        public PropriedadeAlterada(
            string nomePropriedade,
            object antigoValor,
            object novoValor)
        {
            this.NomePropriedade = nomePropriedade;
            this.AntigoValor = antigoValor;
            this.NovoValor = novoValor;
        }

        internal static PropriedadeAlterada Create<T>(string nomePropriedade,
                                                     T antigoValor,
                                                     T novoValor,
                                                     string nomePropriedadeEntidade,
                                                     string nomePropriedadeTipoComplexo)
        {
            if (nomePropriedadeEntidade != null || nomePropriedadeTipoComplexo != null)
            {
                return new PropriedadeAlteradaTipoComplexo(
                    nomePropriedade,
                    antigoValor,
                    novoValor,
                    nomePropriedadeEntidade,
                    nomePropriedadeTipoComplexo);
            }

            return new PropriedadeAlterada(
                  nomePropriedade,
                  antigoValor,
                  novoValor);
        }

        internal static PropriedadeAlterada Create(string nomePropriedade,
                                                   long? antigoValor,
                                                   long? novoValor)
        {
            return new PropriedadeAlterada(nomePropriedade,
                                           antigoValor,
                                           novoValor);
        }
    }

    [IgnorarClasseTS]
    public class PropriedadeAlteradaTipoComplexo : PropriedadeAlterada
    {
        public string NomePropriedadeEntidade { get; set; }
        public string NomePropriedadeTipoComplexo { get; set; }

        [IgnorarConstrutorTS]
        public PropriedadeAlteradaTipoComplexo()
        {

        }

        public PropriedadeAlteradaTipoComplexo(string nomePropriedade,
                                               object antigoValor,
                                               object novoValor,
                                               string nomePropriedadeEntidade,
                                               string propriedadeAlteradaTipoComplexo) :
                                               base(nomePropriedade, antigoValor, novoValor)
        {
            ValidacaoUtil.ValidarReferenciaNula(nomePropriedadeEntidade, nameof(nomePropriedadeEntidade));
            ValidacaoUtil.ValidarReferenciaNula(propriedadeAlteradaTipoComplexo, nameof(propriedadeAlteradaTipoComplexo));

            this.NomePropriedadeEntidade = nomePropriedadeEntidade;
            this.NomePropriedadeTipoComplexo = propriedadeAlteradaTipoComplexo;
        }
    }
}