using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public enum EnumFormatoImagem
    {
        Desconhecido = 0,
        [Rotulo("Joint Photographic Experts Group")]
        JPEG = 1,
        [Rotulo("BMP file format")]
        BMP = 2,
        [Rotulo("Portable Network Graphics")]
        PNG = 3,
        [Rotulo("Tag Image File Format")]
        TIFF = 4,
        [Rotulo("Microsoft Icon")]
        ICO = 5,
        [Rotulo("Graphics Interchange Format")]
        GIF = 6,
        [Rotulo("High Efficiency Image ")]
        HEIC = 7,
        [Rotulo("WebP - Google")]
        WEBP = 8,
        [Rotulo("Scalable Vector Graphics (SVG)")]
        SVG = 9,
        [Rotulo("AV1 Image File Format")]
        AVIF = 10,
        [Rotulo("Animated Portable Network Graphics ")]
        APNG = 11,
        [Rotulo("Abobe Photoshop")]
        PSD = 12,
        [Rotulo("Abobe Photoshop")]
        PSB = 13,
        [Rotulo("CorelDraw")]
        CDR = 14,
        [Rotulo("Abobe  Acrobat Reader or Illustrator")]
        PDF_AI = 15,
        [Rotulo("Abobe RAW DNG")]
        DNG = 16,
        [Rotulo("Canon RAW 2 Image")]
        CR2 = 17,
        [Rotulo("Nikon RAW Image")]
        NEF = 18,
        [Rotulo("Nikon RAW Image")]
        NRW = 19,
        [Rotulo("Sony RAW Image")]
        ARW = 20,
        [Rotulo("Canon RAW Image")]
        CRW = 21,
        [Rotulo("Canon RAW Image")]
        CR3 = 22,
        [Rotulo("Fujifilm RAW Image")]
        RAF = 23,
        [Rotulo("Sony RAW Image")]
        SR2 = 24,
        [Rotulo("Olympus RAW Image")]
        ORF = 25,
        [Rotulo("Nikon RAW Image")]
        NKSC = 26,
        [Rotulo("GoPro RAW Image")]
        GPR = 27,
        [Rotulo("GoPro RAW Image")]
        SRW = 28,
        [Rotulo("Encapsulated PostScript")]
        EPS = 29,
    }
}
