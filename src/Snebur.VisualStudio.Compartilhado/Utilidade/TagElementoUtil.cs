using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public static class TagElementoUtil
    {
        public const string ATRIBUTO_CONTROLE = "sn-controle";
        public const string TAB_CONTROLE_USUARIO = "sn-controle-usuario";

        private static Dictionary<string, string> _tiposControles = null;
        private static Dictionary<string, string> _tiposComponentes = null;
        private static Dictionary<string, string> _tiposHtml = null;
        private static Dictionary<string, string> _tiposSvg = null;

        private static Dictionary<string, string> TagsControles = LazyUtil.RetornarValorLazyComBloqueio(ref TagElementoUtil._tiposControles,
                                                                                                          TagElementoUtil.RetornarTagsControle);
        private static Dictionary<string, string> TagsComponentes = LazyUtil.RetornarValorLazyComBloqueio(ref TagElementoUtil._tiposComponentes,
                                                                                                            TagElementoUtil.RetornarTagsComponentes);

        private static Dictionary<string, string> TagsHtml = LazyUtil.RetornarValorLazyComBloqueio(ref TagElementoUtil._tiposHtml,
                                                                                                     TagElementoUtil.RetornarTagsHtml);

        private static Dictionary<string, string> TagsSvg = LazyUtil.RetornarValorLazyComBloqueio(ref TagElementoUtil._tiposSvg,
                                                                                                    TagElementoUtil.RetornarTagsSvg);

        public static string RetornarTipo(string tagName)
        {
            if (String.IsNullOrEmpty(tagName))
            {
                throw new Erro("A tagname não foi definida");
            }
            tagName = tagName?.Trim().ToLower();
            if (tagName.StartsWith("sn-") || tagName.StartsWith("ap-"))
            {
                if (TagsControles.ContainsKey(tagName))
                {
                    return TagsControles[tagName];
                }
                if (TagsComponentes.ContainsKey(tagName))
                {
                    return TagsComponentes[tagName];
                }
                LogVSUtil.Alerta($"TAGNAME DESCONHECIDA {tagName}");
                return "__Desconhecido__";
            }

            if (TagsHtml.ContainsKey(tagName))
            {
                return TagsHtml[tagName];
            }

            if (TagsSvg.ContainsKey(tagName))
            {
                return TagsSvg[tagName];
            }
            return "HTMLElement";
        }

        private static Dictionary<string, string> RetornarTagsControle()
        {
            var tagsTipos = new Dictionary<string, string>();
            var procurar = "$ElementosControle.Add(\"";

            var linhas = File.ReadAllLines(CaminhosUtil.CaminhoControlesTypescript, Encoding.UTF8);
            var len = linhas.Length;
            for (var i = 0; i < len; i++)
            {
                var linha = linhas[i].Trim();
                if (!linha.StartsWith("//") &&
                     linha.StartsWith(procurar))
                {
                    var partes = linha.Split(',').Where(x => !String.IsNullOrEmpty(x)).ToArray();
                    var tag = partes.First().Replace(procurar, String.Empty).
                                             Replace("\"", String.Empty);
                    var tipo = partes[partes.Length - 2];
                    tagsTipos.Add(tag.ToLower(), $"ui.{tipo.Trim()}");
                }
            }
            return tagsTipos;
        }

        private static Dictionary<string, string> RetornarTagsComponentes()
        {
            var tagsTipos = new Dictionary<string, string>();
            var procurar = "$ComponentesApresentacao.Add(\"";

            var linhas = File.ReadAllLines(CaminhosUtil.CaminhoComponentesApresentacaoTypescript, Encoding.UTF8);
            var len = linhas.Length;
            for (var i = 0; i < len; i++)
            {
                var linha = linhas[i].Trim();
                if (!linha.StartsWith("//") &&
                     linha.StartsWith(procurar))
                {
                    var partes = linha.Split(',').Where(x => !String.IsNullOrEmpty(x)).ToArray();
                    var tag = partes.First().Replace(procurar, String.Empty).
                                             Replace("\"", String.Empty);

                    var tipo = partes.Last().Replace("))", String.Empty).
                                             Replace(";", String.Empty);
                    
                    tagsTipos.Add(tag.ToLower(), $"ui.{tipo.Trim()}");
                }
            }
            return tagsTipos;
        }

        private static Dictionary<string, string> RetornarTagsHtml()
        {
            return new Dictionary<string, string>
            {
                { "a", "HTMLAnchorElement" },
                { "abbr", "HTMLElement" },
                { "address", "HTMLElement" },
                { "applet", "HTMLAppletElement" },
                { "area", "HTMLAreaElement" },
                { "article", "HTMLElement" },
                { "aside", "HTMLElement" },
                { "audio", "HTMLAudioElement" },
                { "b", "HTMLElement" },
                { "base", "HTMLBaseElement" },
                { "basefont", "HTMLBaseFontElement" },
                { "bdi", "HTMLElement" },
                { "bdo", "HTMLElement" },
                { "blockquote", "HTMLQuoteElement" },
                { "body", "HTMLBodyElement" },
                { "br", "HTMLBRElement" },
                { "button", "HTMLButtonElement" },
                { "canvas", "HTMLCanvasElement" },
                { "caption", "HTMLTableCaptionElement" },
                { "cite", "HTMLElement" },
                { "code", "HTMLElement" },
                { "col", "HTMLTableColElement" },
                { "colgroup", "HTMLTableColElement" },
                { "data", "HTMLDataElement" },
                { "datalist", "HTMLDataListElement" },
                { "dd", "HTMLElement" },
                { "del", "HTMLModElement" },
                { "details", "HTMLDetailsElement" },
                { "dfn", "HTMLElement" },
                { "dialog", "HTMLDialogElement" },
                { "dir", "HTMLDirectoryElement" },
                { "div", "HTMLDivElement" },
                { "dl", "HTMLDListElement" },
                { "dt", "HTMLElement" },
                { "em", "HTMLElement" },
                { "embed", "HTMLEmbedElement" },
                { "fieldset", "HTMLFieldSetElement" },
                { "figcaption", "HTMLElement" },
                { "figure", "HTMLElement" },
                { "font", "HTMLFontElement" },
                { "footer", "HTMLElement" },
                { "form", "HTMLFormElement" },
                { "frame", "HTMLFrameElement" },
                { "frameset", "HTMLFrameSetElement" },
                { "h1", "HTMLHeadingElement" },
                { "h2", "HTMLHeadingElement" },
                { "h3", "HTMLHeadingElement" },
                { "h4", "HTMLHeadingElement" },
                { "h5", "HTMLHeadingElement" },
                { "h6", "HTMLHeadingElement" },
                { "head", "HTMLHeadElement" },
                { "header", "HTMLElement" },
                { "hgroup", "HTMLElement" },
                { "hr", "HTMLHRElement" },
                { "html", "HTMLHtmlElement" },
                { "i", "HTMLElement" },
                { "iframe", "HTMLIFrameElement" },
                { "img", "HTMLImageElement" },
                { "input", "HTMLInputElement" },
                { "ins", "HTMLModElement" },
                { "kbd", "HTMLElement" },
                { "label", "HTMLLabelElement" },
                { "legend", "HTMLLegendElement" },
                { "li", "HTMLLIElement" },
                { "link", "HTMLLinkElement" },
                { "main", "HTMLElement" },
                { "map", "HTMLMapElement" },
                { "mark", "HTMLElement" },
                { "marquee", "HTMLMarqueeElement" },
                { "menu", "HTMLMenuElement" },
                { "meta", "HTMLMetaElement" },
                { "meter", "HTMLMeterElement" },
                { "nav", "HTMLElement" },
                { "noscript", "HTMLElement" },
                { "object", "HTMLObjectElement" },
                { "ol", "HTMLOListElement" },
                { "optgroup", "HTMLOptGroupElement" },
                { "option", "HTMLOptionElement" },
                { "output", "HTMLOutputElement" },
                { "p", "HTMLParagraphElement" },
                { "param", "HTMLParamElement" },
                { "picture", "HTMLPictureElement" },
                { "pre", "HTMLPreElement" },
                { "progress", "HTMLProgressElement" },
                { "q", "HTMLQuoteElement" },
                { "rp", "HTMLElement" },
                { "rt", "HTMLElement" },
                { "ruby", "HTMLElement" },
                { "s", "HTMLElement" },
                { "samp", "HTMLElement" },
                { "script", "HTMLScriptElement" },
                { "section", "HTMLElement" },
                { "select", "HTMLSelectElement" },
                { "slot", "HTMLSlotElement" },
                { "small", "HTMLElement" },
                { "source", "HTMLSourceElement" },
                { "span", "HTMLSpanElement" },
                { "strong", "HTMLElement" },
                { "style", "HTMLStyleElement" },
                { "sub", "HTMLElement" },
                { "summary", "HTMLElement" },
                { "sup", "HTMLElement" },
                { "table", "HTMLTableElement" },
                { "tbody", "HTMLTableSectionElement" },
                { "td", "HTMLTableDataCellElement" },
                { "template", "HTMLTemplateElement" },
                { "textarea", "HTMLTextAreaElement" },
                { "tfoot", "HTMLTableSectionElement" },
                { "th", "HTMLTableHeaderCellElement" },
                { "thead", "HTMLTableSectionElement" },
                { "time", "HTMLTimeElement" },
                { "title", "HTMLTitleElement" },
                { "tr", "HTMLTableRowElement" },
                { "track", "HTMLTrackElement" },
                { "u", "HTMLElement" },
                { "ul", "HTMLUListElement" },
                { "var", "HTMLElement" },
                { "video", "HTMLVideoElement" },
                { "wbr", "HTMLElement" }
            };
        }

        private static Dictionary<string, string> RetornarTagsSvg()
        {
            return new Dictionary<string, string>
            {
                //SVG Elements
                {"a", "SVGAElement"},
                {"circle", "SVGCircleElement"},
                {"clipPath", "SVGClipPathElement"},
                {"defs", "SVGDefsElement"},
                {"desc", "SVGDescElement"},
                {"ellipse", "SVGEllipseElement"},
                {"feBlend", "SVGFEBlendElement"},
                {"feColorMatrix", "SVGFEColorMatrixElement"},
                {"feComponentTransfer", "SVGFEComponentTransferElement"},
                {"feComposite", "SVGFECompositeElement"},
                {"feConvolveMatrix", "SVGFEConvolveMatrixElement"},
                {"feDiffuseLighting", "SVGFEDiffuseLightingElement"},
                {"feDisplacementMap", "SVGFEDisplacementMapElement"},
                {"feDistantLight", "SVGFEDistantLightElement"},
                {"feFlood", "SVGFEFloodElement"},
                {"feFuncA", "SVGFEFuncAElement"},
                {"feFuncB", "SVGFEFuncBElement"},
                {"feFuncG", "SVGFEFuncGElement"},
                {"feFuncR", "SVGFEFuncRElement"},
                {"feGaussianBlur", "SVGFEGaussianBlurElement"},
                {"feImage", "SVGFEImageElement"},
                {"feMerge", "SVGFEMergeElement"},
                {"feMergeNode", "SVGFEMergeNodeElement"},
                {"feMorphology", "SVGFEMorphologyElement"},
                {"feOffset", "SVGFEOffsetElement"},
                {"fePointLight", "SVGFEPointLightElement"},
                {"feSpecularLighting", "SVGFESpecularLightingElement"},
                {"feSpotLight", "SVGFESpotLightElement"},
                {"feTile", "SVGFETileElement"},
                {"feTurbulence", "SVGFETurbulenceElement"},
                {"filter", "SVGFilterElement"},
                {"foreignObject", "SVGForeignObjectElement"},
                {"g", "SVGGElement"},
                {"image", "SVGImageElement"},
                {"line", "SVGLineElement"},
                {"linearGradient", "SVGLinearGradientElement"},
                {"marker", "SVGMarkerElement"},
                {"mask", "SVGMaskElement"},
                {"metadata", "SVGMetadataElement"},
                {"path", "SVGPathElement"},
                {"pattern", "SVGPatternElement"},
                {"polygon", "SVGPolygonElement"},
                {"polyline", "SVGPolylineElement"},
                {"radialGradient", "SVGRadialGradientElement"},
                {"rect", "SVGRectElement"},
                {"script", "SVGScriptElement"},
                {"stop", "SVGStopElement"},
                {"style", "SVGStyleElement"},
                {"svg", "SVGSVGElement"},
                {"switch", "SVGSwitchElement"},
                {"symbol", "SVGSymbolElement"},
                {"text", "SVGTextElement"},
                {"textPath", "SVGTextPathElement"},
                {"title", "SVGTitleElement"},
                {"tspan", "SVGTSpanElement"},
                {"use", "SVGUseElement"},
                {"view", "SVGViewElement"}

            };
        }

    }
    
     
}
