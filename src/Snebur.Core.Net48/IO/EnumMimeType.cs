using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public enum EnumMimeType
    {
        [Rotulo("Desconhecido")]
        Desconhecido = 0,
        [Rotulo("Arquivo de audio AAC")]
        Aac = 100,
        [Rotulo("Documento AbiWord")]
        Abw = 200,
        [Rotulo("Abode Illustrator")]
        Ai = 300,
        [Rotulo("Animated Portable Network Graphics")]
        Apng = 320,
        [Rotulo("Documento Arquivado (varios arquivos embutidos)")]
        Arc = 400,
        [Rotulo("Arquivo de audio e vídeo Intercalar AVI")]
        Avi = 500,
        [Rotulo("Formato AV1 Image File Format")]
        Avif = 520,
        [Rotulo("Formato eBook do Amazon Kindle")]
        Azw = 600,
        [Rotulo("Qualquer tipo de dados binários")]
        Bin = 700,
        [Rotulo("Arquivo compactado BZip")]
        Bz = 800,
        [Rotulo("Windows OS/2 Bitmap Graphics")]
        Bmp = 900,
        [Rotulo("Arquivo compactado BZip2")]
        Bz2 = 1000,
        [Rotulo("Corel Draw")]
        Cdr = 1100,
        [Rotulo("Configuração de aplicação")]
        Config = 1200,
        [Rotulo("Script C-Shell")]
        Csh = 1300,
        [Rotulo("Cascading Style Sheets (CSS)")]
        Css = 1400,
        [Rotulo("Valores separados por vírgula (CSV)")]
        Csv = 1500,
        [Rotulo("Arquivo DLL do windows")]
        Dll = 1600,
        [Rotulo("Microsoft Word")]
        Doc = 1700,
        [Rotulo("MS Embedded OpenType fonts")]
        Eot = 1800,
        [Rotulo("Eps")]
        Eps = 1850,
        [Rotulo("Publicação eletrônica (EPUB)")]
        Epub = 1900,
        [Rotulo("Graphics Interchange Format (GIF)")]
        Exe = 2000,
        [Rotulo("Arquivo Executável do Windows")]
        Gif = 2100,
        [Rotulo("HyperText Markup Language (HTML)")]
        Htm = 2200,
        [Rotulo("HyperText Markup Language (HTML)")]
        Html = 2300,
        [Rotulo("Icon format")]
        Ico = 2400,
        [Rotulo("iCalendar format")]
        Ics = 2500,
        [Rotulo("Java Archive (JAR)")]
        Jar = 2600,
        [Rotulo("JPEG images")]
        Jpeg = 2700,
        [Rotulo("JPEG images")]
        Jpg = 2800,
        [Rotulo("JavaScript (ECMAScript)")]
        Js = 2900,
        [Rotulo("JSON format")]
        Json = 3000,
        [Rotulo("Musical Instrument Digital Interface (MIDI)")]
        Mid = 3100,
        [Rotulo("Musical Instrument Digital Interface (MIDI)")]
        Midi = 3200,
        [Rotulo("MPEG Video")]
        Mpeg = 3300,
        [Rotulo("Apple Installer Package")]
        Mpkg = 3400,
        [Rotulo("OpenDocument presentation document")]
        Odp = 3500,
        [Rotulo("OpenDocument spreadsheet document")]
        Ods = 3600,
        [Rotulo("OpenDocument text document")]
        Odt = 3700,
        [Rotulo("OGG audio")]
        Oga = 3800,
        [Rotulo("OGG video")]
        Ogv = 3900,
        [Rotulo("OGG")]
        Ogx = 4000,
        [Rotulo("OpenType font")]
        Otf = 4100,
        [Rotulo("Portable Network Graphics")]
        Png = 4200,
        [Rotulo("Adobe Portable Document Format (PDF)")]
        Pdf = 4300,
        [Rotulo("Arquivo de depuração .NET")]
        Pdb = 4250,
        [Rotulo("Microsoft PowerPoint")]
        Ppt = 4400,
        [Rotulo("Adobe Photoshop")]
        Ps = 4500,
        [Rotulo("Adobe Photoshop")]
        Psb = 4550,
        [Rotulo("Adobe Photoshop")]
        Psd = 4600,
        [Rotulo("RAR archive")]
        Rar = 4700,
        [Rotulo("Rich Text Format (RTF)")]
        Rtf = 4800,
        [Rotulo("Bourne shell script")]
        Sh = 4900,
        [Rotulo("Scalable Vector Graphics (SVG)")]
        Svg = 5000,
        [Rotulo("Small web format (SWF) or Adobe Flash document")]
        Swf = 5100,
        [Rotulo("Tape Archive (TAR)")]
        Tar = 5200,
        [Rotulo("Tagged Image File Format (TIFF)")]
        Txt = 5210,
        [Rotulo("Text/Plain (TXT)")]
        Text = 5220,
        [Rotulo("Text/Plain (TEXT)")]
        Tif = 5300,
        [Rotulo("Tagged Image File Format (TIFF)")]
        Tiff = 5400,
        [Rotulo("Typescript file")]
        Ts = 5500,
        [Rotulo("TrueType Font")]
        Ttf = 5600,
        [Rotulo("Microsoft Visio")]
        Vsd = 5700,
        [Rotulo("Web Embedding - WebAssembly")]
        Wasm = 5750,
        [Rotulo("Waveform Audio Format")]
        Wav = 5800,
        [Rotulo("WEBM audio")]
        Weba = 5900,
        [Rotulo("WEBM video")]
        Webm = 6000,
        [Rotulo("WEBP image")]
        Webp = 6100,
        [Rotulo("Web Open Font Format (WOFF)")]
        Woff = 6200,
        [Rotulo("Web Open Font Format (WOFF)")]
        Woff2 = 6300,
        [Rotulo("XHTML")]
        Xhtml = 6400,
        [Rotulo("Microsoft Excel")]
        Xls = 6500,
        [Rotulo("Microsoft Excel")]
        Xlsx = 6600,
        [Rotulo("XML")]
        Xml = 6700,
        [Rotulo("XUL")]
        Xul = 6800,
        [Rotulo("ZIP archive")]
        Zip = 6900,
        [Rotulo("3GPP audio/video container")]
        _3gp = 7000,
        [Rotulo("3GPP2 audio/video container")]
        _3g2 = 7100,
        [Rotulo("7-zip archive")]
        _7z = 7200,

        Dng = 7201,
        Cr2 = 7202,
        Nef = 7203,
        Nrw = 7204,
        Arw = 7205,
        Crw = 7206,
        Cr3 = 7207,
        Raf = 7208,
        Sr2 = 7209,
        Orf = 7210,
        NKSC = 7211,
        GPR = 7212,
        Srw = 7213,
        Heic = 7214,

    }
}
