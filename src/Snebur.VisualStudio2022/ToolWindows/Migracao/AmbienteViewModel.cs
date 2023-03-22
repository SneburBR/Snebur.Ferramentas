using System;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public class AmbienteViewModel
    {
        public EnumAmbienteServidor AmbienteServidor { get; }  
        public string Nome { get; }

        public AmbienteViewModel(EnumAmbienteServidor AmbienteServidor)
        {
            this.AmbienteServidor = AmbienteServidor;
            this.Nome = EnumUtil.RetornarDescricao(AmbienteServidor);
        }

        public override bool Equals(object obj)
        {
            if(obj is AmbienteViewModel objTipado)
            {
                return this.AmbienteServidor == objTipado.AmbienteServidor;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}