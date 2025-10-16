using Snebur.Dominio.Atributos;

namespace Snebur.UI
{
    public enum BaseEnumComprimento
    {
        [Rotulo("Vazio")]
        Vazio = BaseEnumApresentacao.Vazio,

        Auto = 0,

        #region Porcentagem 

        /// <summary>
        ///   8.33333%
        /// </summary>
        [RotuloVSIntelliSense("8.3%")]
        _8P = 833333,

        /// <summary>
        ///  16.66667%
        /// </summary>
        [RotuloVSIntelliSense("16.6%")]
        _17P = 1666667,

        /// <summary>
        ///  25%;
        /// </summary>
        [RotuloVSIntelliSense("25%")]
        _25P = 2500000,

        /// <summary>
        ///  33.33333%
        /// </summary>
        [RotuloVSIntelliSense("33.3%")]
        _33P = 3333333,

        /// <summary>
        ///  41.66667%
        /// </summary>
        [RotuloVSIntelliSense("41.6%")]
        _42P = 4166667,

        /// <summary>
        ///  50%;
        /// </summary>
        [RotuloVSIntelliSense("50%")]
        _50P = 5000000,

        /// <summary>
        ///  58.33333%;
        /// </summary>
        [RotuloVSIntelliSense("58.3%")]
        _58P = 5833333,

        /// <summary>
        ///  66.66667%;
        /// </summary>
        [RotuloVSIntelliSense("66.6%")]
        _67P = 6666667,

        /// <summary>
        ///  75%;
        /// </summary>
        [RotuloVSIntelliSense("75%")]
        _750P = 7500000,

        /// <summary>
        ///  83.33333%;
        /// </summary>
        [RotuloVSIntelliSense("83.3%")]
        _83P = 8333333,

        /// <summary>
        ///  91.66667%;
        /// </summary>
        [RotuloVSIntelliSense("91.6%")]
        _92P = 9166667,

        /// <summary>
        ///  100%;
        /// </summary>
        [RotuloVSIntelliSense("100%")]
        _100P = 10000000,

        #endregion

        #region Pixels

        //pixels

        [RotuloVSIntelliSense("4px")]
        _4px = 4,

        [RotuloVSIntelliSense("8px")]
        _8px = 8,

        [RotuloVSIntelliSense("16px")]
        _16px = 16,

        [RotuloVSIntelliSense("20px")]
        _20px = 20,

        [RotuloVSIntelliSense("24px")]
        _24px = 24,

        [RotuloVSIntelliSense("28px")]
        _28px = 28,

        [RotuloVSIntelliSense("32px")]
        _32px = 32,

        [RotuloVSIntelliSense("36px")]
        _36px = 36,

        [RotuloVSIntelliSense("40px")]
        _40px = 40,

        [RotuloVSIntelliSense("44px")]
        _44px = 44,

        [RotuloVSIntelliSense("48px")]
        _48px = 48,

        [RotuloVSIntelliSense("60px")]
        _60px = 60,

        [RotuloVSIntelliSense("80px")]
        _80px = 80,

        [RotuloVSIntelliSense("96px")]
        _96px = 96,

        [RotuloVSIntelliSense("112px")]
        _112px = 112,

        [RotuloVSIntelliSense("128px")]
        _128px = 128,

        [RotuloVSIntelliSense("144px")]
        _144px = 144,

        [RotuloVSIntelliSense("160px")]
        _160px = 160,

        [RotuloVSIntelliSense("192px")]
        _192px = 192,

        [RotuloVSIntelliSense("224px")]
        _224px = 224,

        [RotuloVSIntelliSense("256px")]
        _256px = 256,

        [RotuloVSIntelliSense("512px")]
        _512px = 512

        #endregion
    }
}