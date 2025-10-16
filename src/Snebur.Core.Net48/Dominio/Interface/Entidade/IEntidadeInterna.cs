using Snebur.Dominio.Atributos;
using System.Collections.Generic;

namespace Snebur.Dominio
{
    [IgnorarInterfaceTS]
    public interface IEntidadeInterna : IEntidade
    {
        List<string> __PropriedadesAbertas { get; }

        List<string> __PropriedadesAutorizadas { get; }

        //void AtivarControladorPropriedadeAlterada();

        void AtribuirPropriedadesAbertas(List<string> PropriedadesAberta);

        void AtribuirPropriedadesAutorizadas(List<string> PropriedadesAutorizadas);

        void AdicionarProprieadeAberta(string nomePropriedade);
       
        void DesativarValidacaoProprieadesAbertas();

        void AtivarValidacaoProprieadesAbertas();

        void NotifyIsNotNewEntity();
    }
}