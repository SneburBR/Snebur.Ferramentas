using Snebur.Dominio;
using System;

namespace Snebur.Utilidade
{
    public static class MimeTypeUtil
    {
        public static string RetornarMimeType(EnumMimeType tipo)
        {
            switch (tipo)
            {
                case EnumMimeType.Aac:
                    return "audio/aac";
                case EnumMimeType.Abw:
                    return "application/x-abiword";
                case EnumMimeType.Ai:
                    return "application/postscript";
                case EnumMimeType.Apng:
                    return "image/apng";
                case EnumMimeType.Arc:
                    return "application/octet-stream";
                case EnumMimeType.Avi:
                    return "video/x-msvideo";
                case EnumMimeType.Avif:
                    return "image/avif";
                case EnumMimeType.Azw:
                    return "application/vnd.amazon.ebook";
                case EnumMimeType.Bin:
                    return "application/octet-stream";
                case EnumMimeType.Bmp:
                    return "image/bmp";
                case EnumMimeType.Bz:
                    return "application/x-bzip";
                case EnumMimeType.Bz2:
                    return "application/x-bzip2";
                case EnumMimeType.Cdr:
                    return "application/cdr";
                case EnumMimeType.Config:
                    return "application/xml";
                case EnumMimeType.Csh:
                    return "application/x-csh";
                case EnumMimeType.Css:
                    return "text/css";
                case EnumMimeType.Csv:
                    return "text/csv";
                case EnumMimeType.Dll:
                    return "application/octet-stream";
                case EnumMimeType.Doc:
                    return "application/msword";
                case EnumMimeType.Eot:
                    return "application/vnd.ms-fontobject";
                case EnumMimeType.Epub:
                    return "application/epub+zip";
                case EnumMimeType.Exe:
                    return "application/octet-stream";
                case EnumMimeType.Gif:
                    return "image/gif";
                case EnumMimeType.Htm:
                case EnumMimeType.Html:
                    return "text/html";
                case EnumMimeType.Ico:
                    return "image/x-icon";
                case EnumMimeType.Ics:
                    return "text/calendar";
                case EnumMimeType.Jar:
                    return "application/java-archive";
                case EnumMimeType.Jpg:
                case EnumMimeType.Jpeg:
                    return "image/jpeg";
                case EnumMimeType.Js:
                    return "application/javascript";
                case EnumMimeType.Json:
                    return "application/json";
                case EnumMimeType.Mid:
                    return "audio/midi";
                case EnumMimeType.Midi:
                    return "audio/midi";
                case EnumMimeType.Mpeg:
                    return "video/mpeg";
                case EnumMimeType.Mpkg:
                    return "application/vnd.apple.installer+xml";
                case EnumMimeType.Odp:
                    return "application/vnd.oasis.opendocument.presentation";
                case EnumMimeType.Ods:
                    return "application/vnd.oasis.opendocument.spreadsheet";
                case EnumMimeType.Odt:
                    return "application/vnd.oasis.opendocument.text";
                case EnumMimeType.Oga:
                    return "audio/ogg";
                case EnumMimeType.Ogv:
                    return "video/ogg";
                case EnumMimeType.Ogx:
                    return "application/ogg";
                case EnumMimeType.Otf:
                    return "font/otf";
                case EnumMimeType.Png:
                    return "image/png";
                case EnumMimeType.Pdf:
                    return "application/pdf";
                case EnumMimeType.Ppt:
                    return "application/vnd.ms-powerpoint";
                case EnumMimeType.Ps:
                case EnumMimeType.Psd:
                    return "application/photoshop";
                case EnumMimeType.Rar:
                    return "application/x-rar-compressed";
                case EnumMimeType.Rtf:
                    return "application/rtf";
                case EnumMimeType.Sh:
                    return "application/x-sh";
                case EnumMimeType.Svg:
                    return "image/svg+xml";
                case EnumMimeType.Swf:
                    return "application/x-shockwave-flash";
                case EnumMimeType.Tar:
                    return "application/x-tar";
                case EnumMimeType.Tif:
                case EnumMimeType.Tiff:
                    return "image/tiff";
                case EnumMimeType.Ts:
                    return "application/typescript";
                case EnumMimeType.Ttf:
                    return "font/ttf";
                case EnumMimeType.Vsd:
                    return "application/vnd.visio";
                case EnumMimeType.Wav:
                    return "audio/x-wav";
                case EnumMimeType.Weba:
                    return "audio/webm";
                case EnumMimeType.Webm:
                    return "video/webm";
                case EnumMimeType.Webp:
                    return "image/webp";
                case EnumMimeType.Woff:
                    return "font/woff";
                case EnumMimeType.Woff2:
                    return "font/woff2";
                case EnumMimeType.Xhtml:
                    return "application/xhtml+xml";
                case EnumMimeType.Xls:
                    return "application/vnd.ms-excel ";
                case EnumMimeType.Xlsx:
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case EnumMimeType.Xml:
                    return "application/xml";
                case EnumMimeType.Xul:
                    return "application/vnd.mozilla.xul+xml";
                case EnumMimeType.Zip:
                    return "application/zip";
                case EnumMimeType._3gp:
                    return "video/3gpp";
                case EnumMimeType._3g2:
                    return "video/3gpp2";
                case EnumMimeType._7z:
                    return "application/x-7z-compressed";
                case EnumMimeType.Dng:
                    return "image/x-adobe-dng";
                case EnumMimeType.Cr2:
                    return "image/x-canon-cr2";
                case EnumMimeType.Nef:
                    return "image/x-nikon-nef";
                case EnumMimeType.Arw:
                    return "image/x-sony-arw";
                case EnumMimeType.Crw:
                    return "image/x-canon-crw";
                case EnumMimeType.Cr3:
                    return "image/x-canon-cr3";
                case EnumMimeType.Raf:
                    return "image/x-fuji-raf";
                case EnumMimeType.Sr2:
                    return "image/x-sony-sr2";
                case EnumMimeType.Orf:
                    return "image/x-olympus-orf";
                case EnumMimeType.NKSC:
                    return "image/x-nikon-nksc";
                case EnumMimeType.GPR:
                    return "image/x-gopro-gpr";
                case EnumMimeType.Srw:
                    return "image/x-samsung-srw";
                case EnumMimeType.Heic:
                    return "image/heic";
                default:
                    return "application/octet-stream";
            }
        }
        public static EnumMimeType RetornarMimeTypeEnum(string extensao)
        {
            if (String.IsNullOrWhiteSpace(extensao))
            {
                throw new ArgumentNullException("A extensão não foi definida");
            }

            var descricao = TextoUtil.RetornarPrimeiraLetraMaiuscula(TextoUtil.RetornarSomentesLetrasNumeros(extensao));
            if (Enum.TryParse<EnumMimeType>(descricao, out var mimeTupe) &&
                Enum.IsDefined(typeof(EnumMimeType), mimeTupe))
            {
                return mimeTupe;
            }
            if (DebugUtil.IsAttached)
            {
                throw new Exception($"O MimeType para extensão a {extensao} é desconhecido");
            }
            return EnumMimeType.Desconhecido;
        }
        public static EnumMimeType RetornarMimeTypeEnum(EnumFormatoImagem formatoImagem)
        {
            switch (formatoImagem)
            {
                case EnumFormatoImagem.JPEG:

                    return EnumMimeType.Jpeg;

                case EnumFormatoImagem.BMP:

                    return EnumMimeType.Bmp;

                case EnumFormatoImagem.PNG:

                    return EnumMimeType.Png;

                case EnumFormatoImagem.TIFF:

                    return EnumMimeType.Tiff;

                case EnumFormatoImagem.ICO:

                    return EnumMimeType.Ico;

                case EnumFormatoImagem.GIF:

                    return EnumMimeType.Gif;

                default:

                    throw new Erro($"O formato da imagem {formatoImagem.ToString()} não é suportado");
            }
        }
    }
}