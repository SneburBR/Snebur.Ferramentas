using Microsoft.VisualStudio.Imaging.Interop;
using Snebur.VisualStudio.Completations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Snebur.VisualStudio;
public static class HtmlIntelliSenseUtils
{
    private const string PREFIXO_SNEBUR = "sn-";
    private const string PREFIXO_APRESENTACAO = "ap-";
    private const string SUFIXO_CELULAR = "celular";
    private const string SUFIXO_TABLET = "tablet";
    private const string SUFIXO_NOTEBOOK = "notebook";
    //private const string SUFIXO_DESKTOP = "desktop";

    private const string SUFIXO_ALTURA_SUPER__PEQUENA = "super-pequena-v";
    private const string SUFIXO_ALTURA_PEQUENA = "pequena-v";
    private const string SUFIXO_ALTURA_MEDIDA = "media-v";
    private const string SUFIXO_ALTURA_GRANDE = "grande-v";

    public static readonly List<string> Responsivos = new List<string> { SUFIXO_CELULAR, SUFIXO_TABLET, SUFIXO_NOTEBOOK /*, SUFIXO_DESKTOP */};
    public static readonly List<string> ResponsivosAltura = new List<string> { SUFIXO_ALTURA_PEQUENA, SUFIXO_ALTURA_MEDIDA, SUFIXO_ALTURA_GRANDE };

    private static SneburHtmlAttribute[] _sneburAttributes;
    private static Dictionary<string, SneburHtmlAttribute> _sneburAttributesDictionary;
    private static SneburHtmlTagElement[] _sneburTagElements;
    public static SneburHtmlAttribute[] GetSneburAttributes()
        => _sneburAttributes ??= GetSneburAttributesDictionary().Values.ToArray();

    public static Dictionary<string, SneburHtmlAttribute> GetSneburAttributesDictionary()
        => _sneburAttributesDictionary ??= InternalGetSneburAttributesDictionary();

    public static SneburHtmlTagElement[] GetSneburTagElements()
        => _sneburTagElements ??= InternalGetTagElements();

    private static Dictionary<string, SneburHtmlAttribute> InternalGetSneburAttributesDictionary()
    {
        var dicAttributes = new Dictionary<string, SneburHtmlAttribute>(StringComparer.OrdinalIgnoreCase);
        var procurar = "AtributoHtml(\"";
        var linhas = File.ReadAllLines(CaminhosUtil.CaminhoAtributosTypescript, Encoding.UTF8);
        var len = linhas.Length;
        for (var i = 0; i < len; i++)
        {
            var linha = linhas[i].Trim();
            if (linha.StartsWith("//") || !linha.Contains(procurar))
            {
                continue;
            }

            var inicio = linha.IndexOf(procurar) + procurar.Length;
            var fim = linha.IndexOf(",");
            var nomeAtributo = linha.Trim().Substring(inicio, fim - inicio);
            nomeAtributo = nomeAtributo.Replace("\"", String.Empty);

            if (nomeAtributo.StartsWith(PREFIXO_SNEBUR, StringComparison.OrdinalIgnoreCase) ||
                nomeAtributo.StartsWith(PREFIXO_APRESENTACAO, StringComparison.OrdinalIgnoreCase))
            {
                linha = linha.Substring(fim + 1).Trim();
                fim = linha.IndexOf(");");
                var tipo = linha.Substring(0, fim).Trim();
                tipo = tipo.Replace("\"", String.Empty);

                if (dicAttributes.ContainsKey(nomeAtributo))
                {
                    Debugger.Break();
                    continue;
                }

                dicAttributes.Add(nomeAtributo, new(nomeAtributo, tipo));

                if (nomeAtributo.StartsWith(PREFIXO_APRESENTACAO, StringComparison.OrdinalIgnoreCase))
                {
                    var nomeAbrituo = nomeAtributo;
                    var nomeAtributoSimples = nomeAtributo.ToLower().Substring(PREFIXO_APRESENTACAO.Length);

                    //atributos.Add(new Tuple<string, string>($"debug-{nomeAtributoSimples}", tipo));

                    foreach (var responsivo in Responsivos)
                    {
                        var attributeName = $"{nomeAbrituo}--{responsivo}";
                        dicAttributes.Add(attributeName, new(attributeName, tipo));

                        //atributos.Add(new Tuple<string, string>($"debug-{nomeAtributoSimples}-{responsivo}", tipo));
                    }

                    foreach (var responsivo in ResponsivosAltura)
                    {
                        var attributeName = $"{nomeAbrituo}--{responsivo}";
                        dicAttributes.Add(attributeName, new(attributeName, tipo));

                        //atributos.Add(new Tuple<string, string>($"debug-{nomeAtributoSimples}-{responsivo}", tipo));
                    }
                }
            }
        }

        return dicAttributes;
    }

    private static SneburHtmlTagElement[] InternalGetTagElements()
    {
        var procurar = "$ElementosControle.Add(\"";
        var tags = new List<string>();
        var linhas = File.ReadAllLines(CaminhosUtil.CaminhoControlesTypescript, Encoding.UTF8);
        var len = linhas.Length;
        for (var i = 0; i < len; i++)
        {
            var linha = linhas[i].Trim();
            if (linha.StartsWith(procurar))
            {
                var fim = linha.IndexOf(",");
                var tag = linha.Trim().Substring(procurar.Length, fim - procurar.Length);
                tag = tag.Replace("\"", String.Empty);
                tags.Add(tag);
            }
        }
        return tags.Select(x => new SneburHtmlTagElement(x)).ToArray();
    }
}

public class SneburHtmlTagElement
{
    public string TagName { get; }
    public SneburHtmlTagElement(string tagName)
    {
        this.TagName = tagName.ToLower();
    }
}
public class SneburHtmlAttribute
{
    private ImageSource _imageSource;
    public string Name { get; }
    public string TypeValye { get; }
    public bool IsPresentationAttribute { get; }
    public AttributValueType AttributValueType { get; }
    public string[] EnumValues { get; }
    public SneburHtmlAttribute(string name, string type = null)
    {
        this.Name = name;
        this.TypeValye = type;
        this.IsPresentationAttribute = name.StartsWith("ap-", StringComparison.OrdinalIgnoreCase);
        this.AttributValueType = this.DetermineAttributeType();
        this.EnumValues = this.GetEnumValues();
    }

    private string[] GetEnumValues()
    {
        return AttributValueType switch
        {
            AttributValueType.Enum => GetEnumValuesInternal(),
            AttributValueType.Boolean => ["true", "false"],
            _ => Array.Empty<string>(),
        };
        throw new NotImplementedException();
    }

    private string[] GetEnumValuesInternal()
    {
        var typePath = $"{this.TypeValye}, Snebur";
        var enumType = Type.GetType(typePath);
        if (enumType == null || !enumType.IsEnum)
        {
            Debugger.Break();
            return Array.Empty<string>();
        }
        return Enum.GetNames(enumType)
            .Where(x => x != "Underfined")
            .ToArray();
    }

    private AttributValueType DetermineAttributeType()
    {
        var typeName = this.TypeValye?.ToLower();
        if (typeName == null)
        {
            Debugger.Break();
            return AttributValueType.Unknown;
        }

        if (typeName == "string")
            return AttributValueType.String;

        if (typeName == "event")
            return AttributValueType.Event;

        if (typeName == "number")
            return AttributValueType.Number;

        if (typeName == "boolean")
            return AttributValueType.Boolean;

        if (typeName.Contains("."))
        {
            var typePath = $"{typeName}, Snebur";
            var realType = Type.GetType(typePath, throwOnError: false, ignoreCase: true);
            if (realType != null && realType.IsEnum)
            {
                return AttributValueType.Enum;
            }
        }

        return AttributValueType.Unknown;
    }

    internal ImageSource GetAttributeIcon()
        => this._imageSource ??= InternalGetAttributeIcon();

    internal ImageSource GetIconImageSource()
        => this._imageSource ??= InternalGetValueIconImageSource();

    internal ImageSource GetValueIconImageSource()
      => this._imageSource ??= InternalGetValueIconImageSource();

    public ImageMoniker ImageMoniker
        => this.AttributValueType == AttributValueType.Event
            ? ImageMonikerUtils.EventIcon
            : (this.IsPresentationAttribute ? ImageMonikerUtils.PresentationIcon : ImageMonikerUtils.AttributeIcon);

    private ImageSource InternalGetAttributeIcon()
    {
        if (this.AttributValueType == AttributValueType.Event)
        {
            return ImageMonikerUtils.EventIconImage;
        }

        if (this.IsPresentationAttribute)
        {
            return ImageMonikerUtils.PresentationIconImage;
        }
        return ImageMonikerUtils.AttributeIconImage;
    }

    private ImageSource InternalGetValueIconImageSource()
    {
        if (this.AttributValueType == AttributValueType.Event)
        {
            return ImageMonikerUtils.EventIconImage;
        }
        return ImageMonikerUtils.EnumerationValueImage;
    }
}

public enum AttributValueType
{
    Event,
    String,
    Boolean,
    Enum,
    Number,
    Unknown
}