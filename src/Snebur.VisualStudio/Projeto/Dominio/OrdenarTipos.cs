using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Reflection;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public static class HashSetEx
    {

        public static void AddRange<T>(this HashSet<T> source, List<T> lista)
        {
            foreach (var item in lista)
            {
                source.Add(item);
            }
        }
    }

    public class OrdenarTiposHeranca
    {
        private List<Type> Tipos { get; set; }
        private HashSet<Type> TiposOrdenados = new HashSet<Type>();

        public OrdenarTiposHeranca(List<Type> tipos, params Type[] tiposBase)
        {
            this.Tipos = tipos;
            foreach (var tipoBase in tiposBase)
            {
                this.TiposOrdenados.AddRange(this.RetornarTipos(tipoBase));
            }
        }

        private List<Type> RetornarTipos(Type tipoBase)
        {
            var resultado = new List<Type>();
            var tiposBases = TipoUtil.RetornarBaseIgualTipoBase(this.Tipos, tipoBase);
            if (tiposBases.Count > 0)
            {

                resultado.AddRange(tiposBases);
                foreach (var tipoBaseFilho in tiposBases)
                {
                    resultado.AddRange(this.RetornarTipos(tipoBaseFilho));
                }
            }
            else
            {
                var subTipos = TipoUtil.RetornarSubTipos(this.Tipos, tipoBase);
                if (subTipos.Count > 0)
                {
                    tiposBases = RetornarTiposBase(subTipos, tipoBase);

                    foreach(var tipoBaseFilho in tiposBases)
                    {
                        if (this.Tipos.Contains(tipoBaseFilho))
                        {
                            resultado.Add(tipoBaseFilho);
                        }
                    }

                 
                    foreach (var tipoBaseFilho in tiposBases)
                    {
                        resultado.AddRange(this.RetornarTipos(tipoBaseFilho));
                    }
                }
            }
            return resultado;
        }

        private List<Type> RetornarTiposBase(List<Type> tipos, Type tipoBaseComparar)
        {
            var tiposBase = new List<Type>();
            foreach (var tipo in tipos)
            {
                var tipoBaseAtual = tipo;
                while(TipoUtil.TipoSubTipo(tipoBaseAtual.BaseType, tipoBaseComparar))
                {
                    tipoBaseAtual = tipoBaseAtual.BaseType;
                }
                tiposBase.Add(tipoBaseAtual);
            }

            var tiposDistintos = tiposBase.Distinct().ToList();
            return tiposDistintos;
        }
        
        public List<Type> RetornarTiposOrdenados()
        {
            return this.TiposOrdenados.Distinct().ToList();
        }
    }
}
