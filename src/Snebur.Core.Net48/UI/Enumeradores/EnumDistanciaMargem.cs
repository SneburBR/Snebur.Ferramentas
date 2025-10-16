
using Snebur.Dominio.Atributos;

namespace Snebur.UI
{
    public enum EnumDistanciaMargem
    {
        [Rotulo("Vazio")]
        Vazio = BaseEnumApresentacao.Vazio,

        #region Porcentagem

        /// <summary>
        ///   8.33333%
        /// </summary>
        [RotuloVSIntelliSense("Coluna-1")]
        [RotuloVSIntelliSense("8.3%")]
        _8P = BaseEnumComprimento._8P,

        /// <summary>
        ///  16.66667%
        /// </summary>
        [RotuloVSIntelliSense("Coluna-2")]
        [RotuloVSIntelliSense("16.6%")]
        _17P = BaseEnumComprimento._17P,

        /// <summary>
        ///  25%;
        /// </summary>
        [RotuloVSIntelliSense("Coluna-3")]
        [RotuloVSIntelliSense("25%")]
        _25P = BaseEnumComprimento._25P,

        /// <summary>
        ///  33.33333%
        /// </summary>
        [RotuloVSIntelliSense("Coluna-4")]
        [RotuloVSIntelliSense("33.3%")]
        _33P = BaseEnumComprimento._33P,

        /// <summary>
        ///  41.66667%
        /// </summary>
        [RotuloVSIntelliSense("Coluna-5")]
        [RotuloVSIntelliSense("41.6%")]
        _42P = BaseEnumComprimento._42P,

        /// <summary>
        ///  50%;
        /// </summary>
        [RotuloVSIntelliSense("Coluna-6")]
        [RotuloVSIntelliSense("50%")]
        _50P = BaseEnumComprimento._50P,

        /// <summary>
        ///  58.33333%;
        /// </summary>
        [RotuloVSIntelliSense("Coluna-7")]
        [RotuloVSIntelliSense("58.3%")]
        _58P = BaseEnumComprimento._58P,

        /// <summary>
        ///  66.66667%;
        /// </summary>
        [RotuloVSIntelliSense("Coluna-8")]
        [RotuloVSIntelliSense("66.6%")]
        _67P = BaseEnumComprimento._67P,

        [RotuloVSIntelliSense("Coluna-9")]
        [RotuloVSIntelliSense("75%")]
        _750P = BaseEnumComprimento._750P,

        /// <summary>
        ///  83.33333%;
        /// </summary>
        [RotuloVSIntelliSense("Coluna-10")]
        [RotuloVSIntelliSense("83.3%")]
        _83P = BaseEnumComprimento._83P,

        /// <summary>
        ///  91.66667%;
        /// </summary>
        [RotuloVSIntelliSense("Coluna-11")]
        [RotuloVSIntelliSense("91.6%")]
        _92P = BaseEnumComprimento._92P,

        /// <summary>
        ///  100%;
        /// </summary>
        [RotuloVSIntelliSense("Coluna-12")]
        [RotuloVSIntelliSense("100%")]
        _100P = BaseEnumComprimento._100P,

        #endregion

        #region Pixels

        //pixels

        [RotuloVSIntelliSense("4px")]
        _4px = BaseEnumComprimento._4px,

        [RotuloVSIntelliSense("8px")]
        _8px = BaseEnumComprimento._8px,

        [RotuloVSIntelliSense("16px")]
        _16px = BaseEnumComprimento._16px,

        [RotuloVSIntelliSense("20px")]
        _20px = BaseEnumComprimento._20px,

        [RotuloVSIntelliSense("24px")]
        _24px = BaseEnumComprimento._24px,

        [RotuloVSIntelliSense("28px")]
        _28px = BaseEnumComprimento._28px,

        [RotuloVSIntelliSense("32px")]
        _32px = BaseEnumComprimento._32px,

        [RotuloVSIntelliSense("36px")]
        _36px = BaseEnumComprimento._36px,

        [RotuloVSIntelliSense("40px")]
        _40px = BaseEnumComprimento._40px,

        [RotuloVSIntelliSense("44px")]
        _44px = BaseEnumComprimento._44px,

        [RotuloVSIntelliSense("48px")]
        _48px = BaseEnumComprimento._48px,

        [RotuloVSIntelliSense("60px")]
        _60px = BaseEnumComprimento._60px,

        [RotuloVSIntelliSense("80px")]
        _80px = BaseEnumComprimento._80px,

        [RotuloVSIntelliSense("96px")]
        _96px = BaseEnumComprimento._96px,

        [RotuloVSIntelliSense("112px")]
        _112px = BaseEnumComprimento._112px,

        [RotuloVSIntelliSense("128px")]
        _128px = BaseEnumComprimento._128px,

        [RotuloVSIntelliSense("144px")]
        _144px = BaseEnumComprimento._144px,

        [RotuloVSIntelliSense("160px")]
        _160px = BaseEnumComprimento._160px,

        [RotuloVSIntelliSense("192px")]
        _192px = BaseEnumComprimento._192px,

        [RotuloVSIntelliSense("224px")]
        _224px = BaseEnumComprimento._224px,

        [RotuloVSIntelliSense("256px")]
        _256px = BaseEnumComprimento._256px,

        [RotuloVSIntelliSense("512px")]
        _512px = BaseEnumComprimento._512px

        #endregion
    }
}
