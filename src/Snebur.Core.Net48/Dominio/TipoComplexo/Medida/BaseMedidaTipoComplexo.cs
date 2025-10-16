using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;

namespace Snebur.Dominio
{
    [IgnorarClasseTS]
    [IgnorarGlobalizacao]
    public abstract class BaseMedidaTipoComplexo : BaseTipoComplexo, IDpiVisualizacao
    {
        protected Func<double?, double> FuncaoNormalizarDpiVisualizacao;

        [NaoMapear]
        [IgnorarPropriedade]
        Func<double?, double> IDpiVisualizacao.FuncaoNormamlizarDpiVisualizacao
        {
            get => this.FuncaoNormalizarDpiVisualizacao;
            set => this.FuncaoNormalizarDpiVisualizacao = value;
        }

        public double DpiVisualizacao => this.FuncaoNormalizarDpiVisualizacao?.Invoke(null) ?? 0;

        public void SetFuncaoDpiVisualizacao(Func<double?, double> funcaoNormalizarDpiVisualizacao)
        {
            this.FuncaoNormalizarDpiVisualizacao = funcaoNormalizarDpiVisualizacao;
        }

        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        //[NaoMapear]
        //public double DpiVisualizacao
        //{
        //    get
        //    {
        //        if (this._funcaoNormalizarDpiVisualizacao != null)
        //        {
        //            return this._funcaoNormalizarDpiVisualizacao.Invoke();
        //        }
        //        return 0;
        //    }
        //    set
        //    {
        //        var retorno = value;
        //        this._funcaoNormalizarDpiVisualizacao = (null) =>
        //        {
        //            return retorno;
        //        };
        //    }
        //}

        protected int? RetornarValorVisualizacao(double? valor)
        {
            if (valor == null)
            {
                return null;
            }
            return this.RetornarValorVisualizacao(valor.Value);
        }

        protected int RetornarValorVisualizacao(double valor)
        {
            if (this.FuncaoNormalizarDpiVisualizacao == null)
            {
                if (!this.IsSerializando)
                {
                    if (DebugUtil.IsAttached)
                    {
                        throw new Exception($"O DPI visualização não foi definido, utilizar o método estático {nameof(MedidaUtil)}.{nameof(MedidaUtil.DefinirDpiVisualizacao)} e passar o bojeto");
                    }
                }
                return 0;
            }
            var dpi = this.FuncaoNormalizarDpiVisualizacao(valor);
            return MedidaUtil.RetornarPixelsVisualizacao(valor, dpi);
        }

        internal protected override BaseTipoComplexo BaseClone()
        {
            //var retorno = this.BaseClone();
            //retorno.DpiVisualizacao = this.DpiVisualizacao;
            //return retorno;

            throw new NotImplementedException();
            //retorno.DpiVisualizacao = this.DpiVisualizacao;
        }
        [NaoMapear]
        public abstract bool IsEmpty { get; }
    }
}