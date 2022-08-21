using System;
using System.Collections.Generic;
using System.Linq;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public class AtribuirValorProprieadades
    {
        private List<Type> TodosTipos { get; }

        public AtribuirValorProprieadades(string caminhoProjeto,
                                    List<Type> todosTipos,
                                    string nomeArquivo)
        {
            this.TodosTipos = todosTipos;
        }

       


    }
}
