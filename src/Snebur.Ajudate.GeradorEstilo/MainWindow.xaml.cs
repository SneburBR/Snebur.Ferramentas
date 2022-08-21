using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Snebur.UI;
using Snebur.Utilidade;

namespace WpfApp5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            this.Loaded += this.This_Loaded;

        }

        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            this.GerarCores();
        }

        private void GerarCores()
        {
            var coresEnum = EnumUtil.RetornarValoresEnum<EnumCor>();
            var tonalides = EnumUtil.RetornarValoresEnum<EnumTonalidade>();

            var varavies = new StringBuilder();
            var classes = new StringBuilder();
            var classesTexto = new StringBuilder();

            foreach (var cor in coresEnum)
            {
                var descricao = cor.ToString();
                var partes = TextoUtil.DividirLetraMaiuscula(descricao);
                var nomeParcial = "cor-" + String.Join("-", partes.Select(x => x.ToLower()));
                var nomeParcialTexto = "cor-texto-" + String.Join("-", partes.Select(x => x.ToLower()));
                var nomeVariavel = "$" + nomeParcial;
                var nomeClasse = "zs-" + nomeParcial;
                var nomeClasseTexto = "zs-" + nomeParcialTexto;

                var declaracaoVariavel = $"{nomeVariavel}: {nomeParcial};";
                varavies.AppendLine(declaracaoVariavel);

                foreach (var tonalidade in tonalides)
                {
                    classes.AppendLine(this.RetornarClasse(nomeClasse, nomeVariavel, tonalidade, false));
                    classesTexto.AppendLine(this.RetornarClasse(nomeClasseTexto, nomeVariavel, tonalidade, true));
                }

                classes.AppendLine("");
                classes.AppendLine("");

                classesTexto.AppendLine("");
                classesTexto.AppendLine("");
            }

            //var sb = new StringBuilder();
            //sb.AppendLine(varavies.ToString());
            //sb.AppendLine(classes.ToString());
            //sb.AppendLine(classesTexto.ToString());

            var conteudoClasses = classes.ToString();
            var conteudoClassesTexto = classesTexto.ToString();
            var conteudoVariaveis = varavies.ToString();

            var conteudo = conteudoClasses + conteudoClassesTexto;
            var aqui = "";

        }

        private string RetornarClasse(string nomeInicialClasse, string nomeVariavel, EnumTonalidade tonalidade, bool isTexto)
        {
            var variacao = this.RetornarVariacao(tonalidade);
            var nomeClasse = this.RetornarNomeClasse(nomeInicialClasse, tonalidade,  isTexto);
            var conteudoEstilo = this.RetornarConteudoEstilo(nomeInicialClasse, nomeVariavel, tonalidade, variacao, isTexto);
            var sb = new StringBuilder();
            sb.AppendLine(nomeClasse + "{");
            sb.AppendLine(conteudoEstilo);
            sb.AppendLine("}");
            sb.AppendLine("");
            return sb.ToString();
        }

        private int RetornarVariacao(EnumTonalidade tonalidade)
        {
            var variacao = Math.Abs((int)tonalidade);
            if (variacao > 100)
            {
                return variacao - 100;
            }
            return (int)(variacao / 10D) * 6;
        }

        private string RetornarNomeClasse(string nomeInicialClasse, EnumTonalidade tonalidade, bool isTexto)
        {
            if (tonalidade == EnumTonalidade.Padrao)
            {
                return $".{nomeInicialClasse}";
            }
            var intTonlidade = Math.Abs((int)tonalidade);
            if (intTonlidade > 100) intTonlidade -= 100;
            var descricaoTonalidade = TextoUtil.RetornarSomenteLetras(tonalidade.ToString()).ToLower();
            return $".{nomeInicialClasse}-{descricaoTonalidade}-{intTonlidade}";
        }

        private string RetornarConteudoEstilo(string nomeInicialClasse, string nomeVariavel, EnumTonalidade tonalidade, int variacao, bool isTexto)
        {
            var nomePropriedade = (isTexto) ? "color" : "background-color";
            var valorEstilo = this.RetornarValorEstilo(nomeVariavel, tonalidade, variacao);
            return $"{nomePropriedade} : {valorEstilo} !important;";
        }

        private string RetornarValorEstilo(string nomeVariavel, EnumTonalidade tonalidade, int variacao)
        {
            if (tonalidade == EnumTonalidade.Padrao)
            {
                return nomeVariavel;
            }

            var nomeFuncao = this.RetornarNomeFuncao(tonalidade);
            var valorVariacao = this.RetornarValorVariacao(tonalidade, variacao);
            return $"{nomeFuncao}({nomeVariavel}, {valorVariacao})";
        }

        private string RetornarNomeFuncao(EnumTonalidade tonalidade)
        {
            var intTonalidade = (int)tonalidade;
            if (intTonalidade > 100)
            {
                return "rgba";
            }
            if (intTonalidade < 0)
            {
                return "darken";
            }
            return "lighten";
        }

        private string RetornarValorVariacao(EnumTonalidade tonalidade, int variacao)
        {
            var intTonalidade = (int)tonalidade;
            if (intTonalidade > 100)
            {
                var ratio = variacao / 100D;
                return ratio.ToString(CultureInfo.InvariantCulture);
            }

            return Math.Abs(variacao).ToString() + "%";
        }
    }




}

