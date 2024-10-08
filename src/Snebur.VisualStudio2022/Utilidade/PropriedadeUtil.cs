﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using System.Reflection;
using Snebur.Dominio.Atributos;

namespace Snebur.VisualStudio.Reflexao
{
    public class PropriedadeUtil
    {
        internal static bool PossuiAtributo(PropertyInfo propriedade, Type tipoAtributo)
        {
            return PossuiAtributo(propriedade, tipoAtributo.Name);
        }

        internal static bool PossuiAtributo(MethodInfo propriedade, Type tipoAtributo)
        {
            return PossuiAtributo(propriedade, tipoAtributo.Name);
        }

        internal static bool PossuiAtributo(PropertyInfo propriedade, string nomeAtributo)
        {
            return PossuiAtributo((MemberInfo)propriedade, nomeAtributo);
        }

        internal static bool PossuiAtributo(MethodInfo metodo, string nomeAtributo)
        {
            return PossuiAtributo((MemberInfo)metodo, nomeAtributo);
        }

        private static bool PossuiAtributo(MemberInfo propriedadeOuMetodo, string nomeAtributo)
        {
            return propriedadeOuMetodo.GetCustomAttributes(false).Any(k => k.GetType().Name == nomeAtributo);
        }

        public static PropertyInfo RetornarPropriedadeChaveEstrangeira(PropertyInfo propriedade)
        {
            dynamic atributoChaveEstrangeira = RetornarAtributoChaveEstrangeira(propriedade);
            if (atributoChaveEstrangeira != null)
            {
                return propriedade.DeclaringType.GetProperty(atributoChaveEstrangeira.Name);
            }
            return null;
        }

        private static Attribute RetornarAtributoChaveEstrangeira(PropertyInfo propriedade)
        {
            if (PropriedadeUtil.PossuiAtributo(propriedade, nameof(ChaveEstrangeiraAttribute)))
            {
                return RetornarAtributo(propriedade, typeof(ChaveEstrangeiraAttribute), true);
            }
            if (PropriedadeUtil.PossuiAtributo(propriedade, nameof(ChaveEstrangeiraRelacaoUmUmAttribute)))
            {
                return RetornarAtributo(propriedade, typeof(ChaveEstrangeiraRelacaoUmUmAttribute), true);
            }
            return null;
        }



        internal static Attribute RetornarAtributo(MemberInfo propriedade, Type tipoAtributo, bool atributoHerdadoTipo)
        {
            if (atributoHerdadoTipo)
            {
                return propriedade.GetCustomAttributes(false).Cast<Attribute>().Where(x => x.GetType().Name == tipoAtributo.Name || TipoUtil.TipoIgualOuSubTipo(x.GetType(), tipoAtributo)).SingleOrDefault();
            }
            else
            {
                return propriedade.GetCustomAttributes(false).Cast<Attribute>().Where(x => x.GetType().Name == tipoAtributo.Name).SingleOrDefault();
            }
        }

        internal static List<Attribute> RetornarAtributos(MemberInfo propriedade, Type tipoAtributo, bool atributoHerdadoTipo)
        {
            if (atributoHerdadoTipo)
            {
                return propriedade.GetCustomAttributes(false).Cast<Attribute>().Where(x => x.GetType().Name == tipoAtributo.Name || TipoUtil.TipoIgualOuSubTipo(x.GetType(), tipoAtributo)).ToList();
            }
            else
            {
                return propriedade.GetCustomAttributes(false).Cast<Attribute>().Where(x => x.GetType().Name == tipoAtributo.Name).ToList();
            }
        }
    }
}
