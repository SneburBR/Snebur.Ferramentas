using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Snebur.Utilidade;

namespace AtualizarIconesMaterial
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var icones = RetornarIconesDoHtml();
            SalvarEnumIcones(icones);
        }

        private static void SalvarEnumIcones(HashSet<(string name, string pascalCase, string snakeCase)> icones)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Snebur.Dominio.Atributos;");
            sb.AppendLine("");
            sb.AppendLine("namespace Snebur.UI");
            sb.AppendLine("{");
            sb.AppendLine("\tpublic enum EnumIcone");
            sb.AppendLine("\t{");
            var contador = 0;
            foreach (var (name, pascalCase, snakeCase) in icones)
            {
                contador += 1;
                var prefixo = Char.IsNumber(name[0]) ? "_" : "";
                sb.AppendLine($"\t\t[Rotulo(\"{pascalCase}\")]");
                sb.AppendLine($"\t\t{prefixo}{pascalCase} = {contador},");
                sb.AppendLine();
            }
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            var conteudo = sb.ToString();
            File.WriteAllText("EnumIcone.cs", conteudo, Encoding.UTF8);
        }

        private static HashSet<(string name, string pascalCase, string snakeCase)> RetornarIconesDoHtml()
        {
            var caminhoHtml = "Html-31-07-2023.html";
            if (!File.Exists(caminhoHtml))
            {
                throw new FileNotFoundException(caminhoHtml);
            }

            var retorno = new HashSet<(string, string, string)>();
            using (var fs = StreamUtil.OpenRead(caminhoHtml))
            {
                var document = new HtmlDocument();
                document.Load(fs);

                var buttons = document.DocumentNode.Descendants("button");
                foreach (var button in buttons)
                {
                    var spans = button.ChildNodes.Where(x => x.Name == "span" && x.InnerText.Trim().Length > 0).ToList();
                    if(spans.Count()== 2)
                    {
                        var snakeCase = spans.First().InnerText.Trim();
                        var name = spans.Last().InnerText.Trim();
                        if (IsSnakeCaseValido(name, snakeCase))
                        {
                            var pascalCase = FormatarPascalCase(name);
                            retorno.Add((name, pascalCase, snakeCase));
                        }

                    }


                    //var spans = spanFirst.Descendants("span");
                    //if (spans.Count() == 2)
                    //{
                    //    var snakeCase = spans.First().InnerHtml.Trim();
                    //    var name = spans.Last().InnerHtml.Trim();

                    //    if (IsSnakeCaseValido(name, snakeCase))
                    //    {
                    //        var pascalCase = FormatarPascalCase(name);
                    //        retorno.Add((name, pascalCase, snakeCase));
                    //    }
                    //}
                }
            }
            return retorno;
        }


        private static bool IsSnakeCaseValido(string name, string snakeName)
        {
            var partes = new Regex(@"\s+").Split(name).Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
            var snake = String.Join("_", partes.Select(x => x.ToLower()));
            if (snake != snakeName)
            {
                throw new Exception($"Name diferente do snake {name} - {snakeName}");
            }
            return true;
        }

        private static string FormatarPascalCase(string name)
        {
            var partes = new Regex(@"\s+").Split(name).Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
            return String.Join("", partes.Select(x => TextoUtil.RetornarPrimeiraLetraMaiuscula(x)));
        }

    }
}
