using Snebur.Dominio;
using Snebur.Utilidade;
using System.IO;
using System.Threading.Tasks;

namespace Zyoncore.Imagens
{
    public static class FormatoImagemUtil
    {
        public static Task<string> RetornarMimeTypeAsync(this FileInfo fileInfo)
        {
            return TaskUtil.Run(() =>
            {
                return RetornarMimeType(fileInfo);
            });
        }
        public static string RetornarMimeType(string caminhoArquivo)
        {
            var mimeTypeEnum = RetornarMimeTypeEnum(new FileInfo(caminhoArquivo));
            return MimeTypeUtil.RetornarMimeType(mimeTypeEnum);
        }
        public static string RetornarMimeType(FileInfo fileInfo)
        {
            var mimeTypeEnum = RetornarMimeTypeEnum(fileInfo);
            return MimeTypeUtil.RetornarMimeType(mimeTypeEnum);
        }

        public static Task<EnumMimeType> RetornarMimeTypeEnumAsync(this FileInfo fileInfo)
        {
            return TaskUtil.Run(() =>
            {
                return RetornarMimeTypeEnum(fileInfo);
            });
        }

        public static Task<EnumMimeType> RetornarMimeTypeEnumAsync(this Stream stream)
        {
            return TaskUtil.Run(() =>
            {
                return RetornarMimeTypeEnum(stream);
            });
        }

        private static EnumMimeType RetornarMimeTypeEnum(FileInfo fileInfo)
        {
            using (var fs = fileInfo.OpenRead())
            {
                return RetornarMimeTypeEnum(fs);
            }
        }

        private static EnumMimeType RetornarMimeTypeEnum(Stream stream)
        {
            var formatoImagem = RetornarFormatoImagem(stream);
            switch (formatoImagem)
            {
                case EnumFormatoImagem.Desconhecido:
                    return EnumMimeType.Desconhecido;
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
                case EnumFormatoImagem.HEIC:
                    return EnumMimeType.Heic;
                case EnumFormatoImagem.WEBP:
                    return EnumMimeType.Webp;
                case EnumFormatoImagem.SVG:
                    return EnumMimeType.Svg;
                case EnumFormatoImagem.AVIF:
                    return EnumMimeType.Avif;
                case EnumFormatoImagem.APNG:
                    return EnumMimeType.Apng;
                case EnumFormatoImagem.PSD:
                    return EnumMimeType.Psd;
                case EnumFormatoImagem.PSB:
                    return EnumMimeType.Psb;
                case EnumFormatoImagem.CDR:
                    return EnumMimeType.Cdr;
                case EnumFormatoImagem.PDF_AI:
                    return EnumMimeType.Pdf;
                case EnumFormatoImagem.DNG:
                    return EnumMimeType.Dng;
                case EnumFormatoImagem.CR2:
                    return EnumMimeType.Cr2;
                case EnumFormatoImagem.NEF:
                    return EnumMimeType.Nef;
                case EnumFormatoImagem.NRW:
                    return EnumMimeType.Nrw;
                case EnumFormatoImagem.ARW:
                    return EnumMimeType.Arw;
                case EnumFormatoImagem.CRW:
                    return EnumMimeType.Crw;
                case EnumFormatoImagem.CR3:
                    return EnumMimeType.Cr3;
                case EnumFormatoImagem.RAF:
                    return EnumMimeType.Raf;
                case EnumFormatoImagem.SR2:
                    return EnumMimeType.Sr2;
                case EnumFormatoImagem.ORF:
                    return EnumMimeType.Orf;
                case EnumFormatoImagem.NKSC:
                    return EnumMimeType.NKSC;
                case EnumFormatoImagem.GPR:
                    return EnumMimeType.GPR;
                case EnumFormatoImagem.SRW:
                    return EnumMimeType.Srw;
                case EnumFormatoImagem.EPS:
                    return EnumMimeType.Eps;
                default:
                    return EnumMimeType.Desconhecido;
            }
        }

        public static Task<EnumFormatoImagem> RetornarFormatoImagemAsync(this FileInfo fileInfo)
        {
            return TaskUtil.Run(() =>
            {
                return RetornarFormatoImagem(fileInfo);
            });
        }

        public static Task<EnumFormatoImagem> RetornarFormatoImagemAsync(this Stream stream)
        {
            return TaskUtil.Run(() =>
            {
                return RetornarFormatoImagem(stream);
            });
        }

        public static EnumFormatoImagem RetornarFormatoImagem(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return EnumFormatoImagem.Desconhecido;
            }

            using (var fs = fileInfo.OpenRead())
            {
                return RetornarFormatoImagem(fs);
            }
        }

        public static EnumFormatoImagem RetornarFormatoImagem(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[16];
            var lidos = stream.Read(buffer, 0, buffer.Length);
            if(lidos < buffer.Length)
            {
                return EnumFormatoImagem.Desconhecido;
            }

            stream.Seek(0, SeekOrigin.Begin);

            if (IsJpeg(buffer))
            {
                return EnumFormatoImagem.JPEG;
            }
            else if (IsPng(buffer))
            {
                return EnumFormatoImagem.PNG;
            }
            else if (IsGif(buffer))
            {
                return EnumFormatoImagem.GIF;
            }
            else if (IsBmp(buffer))
            {
                return EnumFormatoImagem.BMP;
            }
            else if (IsTiff(buffer))
            {
                return EnumFormatoImagem.TIFF;
            }
            else if (IsIco(buffer))
            {
                return EnumFormatoImagem.ICO;
            }
            else if (IsHeic(buffer))
            {
                return EnumFormatoImagem.HEIC;
            }
            else if (IsPdfOrAi(buffer))
            {
                return EnumFormatoImagem.PDF_AI;
            }
            else if (IsWebp(buffer))
            {
                return EnumFormatoImagem.WEBP;
            }
            else if (IsSvg(buffer))
            {
                return EnumFormatoImagem.SVG;
            }
            else if (IsAvif(buffer))
            {
                return EnumFormatoImagem.AVIF;
            }
            else if (IsApng(buffer))
            {
                return EnumFormatoImagem.APNG;
            }
            else if (IsPsd(buffer))
            {
                return EnumFormatoImagem.PSD;
            }
            else if (IsPsb(buffer))
            {
                return EnumFormatoImagem.PSB;
            }
            else if (IsCdr(buffer))
            {
                return EnumFormatoImagem.CDR;
            }
            else if (IsDng(buffer))
            {
                return EnumFormatoImagem.DNG;
            }
            else if (IsCr2(buffer))
            {
                return EnumFormatoImagem.CR2;
            }
            else if (IsNef(buffer))
            {
                return EnumFormatoImagem.NEF;
            }
            else if (IsNrw(buffer))
            {
                return EnumFormatoImagem.NRW;
            }
            else if (IsArw(buffer))
            {
                return EnumFormatoImagem.ARW;
            }
            else if (IsCrw(buffer))
            {
                return EnumFormatoImagem.CRW;
            }
            else if (IsCr3(buffer))
            {
                return EnumFormatoImagem.CR3;
            }
            else if (IsRaf(buffer))
            {
                return EnumFormatoImagem.RAF;
            }
            else if (IsSr2(buffer))
            {
                return EnumFormatoImagem.SR2;
            }
            else if (IsOrf(buffer))
            {
                return EnumFormatoImagem.ORF;
            }
            else if (IsNKSC(buffer))
            {
                return EnumFormatoImagem.NKSC;
            }
            else if (IsGpr(buffer))
            {
                return EnumFormatoImagem.GPR;
            }
            else if (IsSrw(buffer))
            {
                return EnumFormatoImagem.SRW;
            }
            else if (IsEps(buffer))
            {
                return EnumFormatoImagem.EPS;
            }
            else
            {
                return EnumFormatoImagem.Desconhecido;
            }
        }

        public static bool IsJpeg(byte[] buffer)
        {
            return buffer[0] == 0xFF && buffer[1] == 0xD8;
        }

        public static bool IsPng(byte[] buffer)
        {
            byte[] pngSignature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
            for (int i = 0; i < pngSignature.Length; i++)
            {
                if (buffer[i] != pngSignature[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsGif(byte[] buffer)
        {
            return buffer[0] == 'G' && buffer[1] == 'I' && buffer[2] == 'F' && buffer[3] == '8'
                   && (buffer[4] == '7' || buffer[4] == '9') && buffer[5] == 'a';
        }

        public static bool IsBmp(byte[] buffer)
        {
            return buffer[0] == 'B' && buffer[1] == 'M';
        }

        public static bool IsTiff(byte[] buffer)
        {
            return (buffer[0] == 'I' && buffer[1] == 'I' && buffer[2] == 0x2A && buffer[3] == 0x00)
                   || (buffer[0] == 'M' && buffer[1] == 'M' && buffer[2] == 0x00 && buffer[3] == 0x2A);
        }

        public static bool IsIco(byte[] buffer)
        {
            return buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0x01 && buffer[3] == 0x00;
        }

        public static bool IsWebp(byte[] buffer)
        {
            return buffer[8] == 'W' && buffer[9] == 'E' && buffer[10] == 'B' && buffer[11] == 'P';
        }

        public static bool IsSvg(byte[] buffer)
        {
            return buffer[0] == '<' && buffer[1] == '?' && buffer[2] == 'x' && buffer[3] == 'm'
                   && buffer[4] == 'l' && buffer[5] == ' ' && buffer[6] == 'v' && buffer[7] == 'e';
        }

        public static bool IsAvif(byte[] buffer)
        {
            byte[] signature = { 0x66, 0x74, 0x79, 0x70, 0x61, 0x76, 0x69, 0x66 };
            int offset = 4;
            if (buffer.Length >= offset + signature.Length)
            {
                for (int i = 0; i < signature.Length; i++)
                {
                    if (buffer[offset + i] != signature[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool IsApng(byte[] buffer)
        {
            return buffer[0] == 0x89 && buffer[1] == 'A' && buffer[2] == 'P' && buffer[3] == 'N'
                   && buffer[4] == 'G' && buffer[5] == 0x0D && buffer[6] == 0x0A && buffer[7] == 0x1A
                   && buffer[8] == 0x0A;
        }

        public static bool IsHeic(byte[] buffer)
        {
            // Check if the first 12 bytes match the HEIF file format
            return buffer[4] == 'f' && buffer[5] == 't' && buffer[6] == 'y' && buffer[7] == 'p'
                   && buffer[8] == 'h' && buffer[9] == 'e' && buffer[10] == 'i' && buffer[11] == 'c';
        }

        public static bool IsPsd(byte[] buffer)
        {
            if (buffer.Length < 4)
            {
                return false;
            }
            return (buffer[0] == '8' && buffer[1] == 'B' && buffer[2] == 'P' && buffer[3] == 'S');
        }

        public static bool IsPsb(byte[] buffer)
        {
            return (buffer[0] == '8' && buffer[1] == 'B' && buffer[2] == 'P' && buffer[3] == 'T');
        }

        public static bool IsCdr(byte[] buffer)
        {
            if (buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46)
            {
                return true;
            }
            return false;
        }

        public static bool IsPdfOrAi(byte[] buffer)
        {
            byte[] pdfSignature = { 0x25, 0x50, 0x44, 0x46, 0x2d };
            int pdfSignatureLength = pdfSignature.Length;
            if (buffer.Length < pdfSignatureLength)
                return false;
            for (int i = 0; i < pdfSignatureLength; i++)
            {
                if (buffer[i] != pdfSignature[i])
                    return false;
            }
            return true;
        }
        public static bool IsPdf2(byte[] buffer)
        {
            // Check for PDF header "25 50 44 46"
            return buffer != null && buffer.Length >= 4 && buffer[0] == 0x25 && buffer[1] == 0x50 && buffer[2] == 0x44 && buffer[3] == 0x46;
        }

        public static bool IsDng(byte[] buffer)
        {
            byte[] dngSignature = new byte[] { 0x49, 0x49, 0x2A, 0x00, 0x10, 0x00, 0x00, 0x00 };

            for (int i = 0; i < dngSignature.Length; i++)
            {
                if (buffer[i] != dngSignature[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsCr2(byte[] buffer)
        {
            byte[] cr2Signature = new byte[] { 0x49, 0x49, 0x2A, 0x00, 0x10, 0x00, 0x00, 0x00, 0x43, 0x52, 0x02, 0x00 };

            for (int i = 0; i < cr2Signature.Length; i++)
            {
                if (buffer[i] != cr2Signature[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsNef(byte[] buffer)
        {
            byte[] nefSignature = new byte[] { 0x4D, 0x4D, 0x00, 0x2A, 0x00, 0x00, 0x00, 0x08, 0x00, 0x01 };

            for (int i = 0; i < nefSignature.Length; i++)
            {
                if (buffer[i] != nefSignature[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsNrw(byte[] buffer)
        {
            byte[] nrwSignature = new byte[] { 0x49, 0x49, 0x2A, 0x00, 0x10, 0x00, 0x00, 0x00 };

            for (int i = 0; i < nrwSignature.Length; i++)
            {
                if (buffer[i] != nrwSignature[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsArw(byte[] buffer)
        {
            byte[] arwSignature = new byte[] { 0x49, 0x49, 0x2A, 0x00, 0x08, 0x00, 0x00, 0x00, 0x43, 0x52, 0x57, 0x52 };

            for (int i = 0; i < arwSignature.Length; i++)
            {
                if (buffer[i] != arwSignature[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsCrw(byte[] buffer)
        {
            byte[] crwSignature1 = new byte[] { 0x49, 0x49, 0x2A, 0x00, 0x10, 0x00, 0x00, 0x00 };
            byte[] crwSignature2 = new byte[] { 0x4D, 0x4D, 0x00, 0x2A, 0x00, 0x00, 0x00, 0x08 };

            bool hasFirstSignature = true;
            for (int i = 0; i < crwSignature1.Length; i++)
            {
                if (buffer[i] != crwSignature1[i])
                {
                    hasFirstSignature = false;
                    break;
                }
            }

            bool hasSecondSignature = true;
            for (int i = 0; i < crwSignature2.Length; i++)
            {
                if (buffer[i] != crwSignature2[i])
                {
                    hasSecondSignature = false;
                    break;
                }
            }

            return hasFirstSignature || hasSecondSignature;
        }

        public static bool IsCr3(byte[] buffer)
        {
            byte[] cr3Signature = new byte[] { 0x48, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };

            bool hasSignature = true;
            for (int i = 0; i < cr3Signature.Length; i++)
            {
                if (buffer[i] != cr3Signature[i])
                {
                    hasSignature = false;
                    break;
                }
            }

            return hasSignature;
        }

        public static bool IsRaf(byte[] buffer)
        {
            byte[] rafSignature = new byte[] { 0x46, 0x55, 0x4A, 0x49, 0x46, 0x49, 0x4C, 0x4D, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            bool hasSignature = true;
            for (int i = 0; i < rafSignature.Length; i++)
            {
                if (buffer[i] != rafSignature[i])
                {
                    hasSignature = false;
                    break;
                }
            }

            return hasSignature;
        }

        public static bool IsSr2(byte[] buffer)
        {
            byte[] sr2Signature = new byte[] { 0x53, 0x4F, 0x4E, 0x59, 0x2D, 0x52, 0x41, 0x57, 0x20, 0x49, 0x4D, 0x41 };

            bool hasSignature = true;
            for (int i = 0; i < sr2Signature.Length; i++)
            {
                if (buffer[i] != sr2Signature[i])
                {
                    hasSignature = false;
                    break;
                }
            }
            return hasSignature;
        }

        public static bool IsOrf(byte[] buffer)
        {
            byte[] orfSignature = new byte[] { 0x49, 0x49, 0x2A, 0x00 };
            bool hasSignature = true;

            for (int i = 0; i < orfSignature.Length; i++)
            {
                if (buffer[i] != orfSignature[i])
                {
                    hasSignature = false;
                    break;
                }
            }
            return hasSignature;
        }

        public static bool IsNKSC(byte[] buffer)
        {
            byte[] nkscSignature = new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, 0x76, 0x65 };

            bool hasSignature = true;
            for (int i = 0; i < nkscSignature.Length; i++)
            {
                if (buffer[i] != nkscSignature[i])
                {
                    hasSignature = false;
                    break;
                }
            }
            return hasSignature;
        }

        public static bool IsGpr(byte[] buffer)
        {
            byte[] gprSignature = new byte[] { 0x47, 0x50, 0x52, 0x0A, 0x00, 0x01, 0x00, 0x00 };

            bool hasSignature = true;
            for (int i = 0; i < gprSignature.Length; i++)
            {
                if (buffer[i] != gprSignature[i])
                {
                    hasSignature = false;
                    break;
                }
            }

            return hasSignature;
        }

        public static bool IsSrw(byte[] buffer)
        {
            return buffer[0] == 'I' && buffer[1] == 'I' && buffer[2] == 0 && buffer[3] == 0 &&
                   buffer[8] == 'S' && buffer[9] == 'R' && buffer[10] == 'W' && buffer[11] == 0;
        }

        public static bool IsEps(byte[] buffer)
        {
            if (buffer[0] == 197 &&
                buffer[1] == 208 &&
                buffer[2] == 211 &&
                buffer[3] == 198)
            {
                return true;
            }
            return (buffer[0] == 0x25 && buffer[1] == 0x21 && buffer[2] == 0x50 && buffer[3] == 0x53);
        }
    }
}