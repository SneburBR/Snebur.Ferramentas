using Snebur.Utilidade;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoRequeridoAttribute : RequiredAttribute, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O campo {0} deve ser preenchido.";
        public EnumOpcoesComparacaoAuxiliar? OpcoesComparacaoAuxiliar { get; set; } = null;
        public string NomePropridadeAuxiliar { get; set; }

        public bool IsIgnorarValidacaoSeAuxiliarInvalido { get; set; } = false;

        /// <summary>
        /// Se valor comprar for diferente de null, o valor da propriedade auxiliar será comparado com este valor.
        /// Caso contrário, o valor da propriedade auxiliar será comparado com o valor da propriedade que está sendo validada.
        /// </summary>
        public object ValorComparar { get; set; }
        private PropertyInfo PropriedadeAuxiliar { get; set; }

        [IgnorarConstrutorTS]
        public ValidacaoRequeridoAttribute()
        {

        }

        /// <summary>
        /// Opções de comparação do auxiliar, quando a validação requerida depende de outra propriedade.
        /// </summary>
        /// <param name="tipoEntidade"></param>
        /// <param name="opcoesComparacaoAuxiliar"></param>
        /// <param name="nomePropridadeAuxiliar">
        /// Nome da propriedade auxiliar deve pertencer ao mesmo tipo do TipoEntidade, ambos propriedade devem ser declarada no mesmo tipo.
        /// </param>
        /// <param name="isIgnorarValidacaoSeAuxiliarInvalido">
        /// Quando á comparação é verdadeira com propriedade auxiliar for falso, o valor da propriedade não será requerido
        /// </param>
        /// <param name="valorComparar">
        /// Se o valor comparar for diferente de null, o valor da propriedade auxiliar será comparado com este valor.
        /// Caso contrário, o valor da propriedade auxiliar será comparado com o valor da propriedade que está sendo validada.
        /// </param>
        public ValidacaoRequeridoAttribute([IgnorarParametroTS] Type tipoEntidade,
                                           [ParametroOpcionalTS] EnumOpcoesComparacaoAuxiliar opcoesComparacaoAuxiliar,
                                           [ParametroOpcionalTS] string nomePropridadeAuxiliar,
                                           [ParametroOpcionalTS] bool isIgnorarValidacaoSeAuxiliarInvalido = false,
                                           [ParametroOpcionalTS] object valorComparar = null)
        {
            this.OpcoesComparacaoAuxiliar = opcoesComparacaoAuxiliar;
            this.NomePropridadeAuxiliar = nomePropridadeAuxiliar;
            this.IsIgnorarValidacaoSeAuxiliarInvalido = isIgnorarValidacaoSeAuxiliarInvalido;
            this.ValorComparar = valorComparar;

            if (opcoesComparacaoAuxiliar != EnumOpcoesComparacaoAuxiliar.Nenhuma)
            {
                this.PropriedadeAuxiliar = this.RetornarPropriedadeAuxiliar(tipoEntidade);
            }
        }

        private PropertyInfo RetornarPropriedadeAuxiliar(Type tipoEntidade)
        {
            if (String.IsNullOrWhiteSpace(this.NomePropridadeAuxiliar))
            {
                throw new Exception("O nome da propriedade auxiliar deve ser informado, quando opção de validação for diferente de nenhuma.");
            }

            //BindingFlags propriedade internal ou publica e reaonly  ou read/write
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var propriedade = tipoEntidade.GetProperty(this.NomePropridadeAuxiliar, flags);
            if (propriedade == null)
            {
                throw new Exception($"A propriedade auxiliar {this.NomePropridadeAuxiliar} informada não foi encontrada na entidade {tipoEntidade.Name}.");
            }
            return propriedade;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var resultado = base.IsValid(value, validationContext);
            if (resultado != null)
            {
                var propriedade = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                var paiPropriedade = validationContext.ObjectInstance;
                var valorPropriedade = value;
                if (this.IsValido(propriedade, paiPropriedade, valorPropriedade))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    this.ErrorMessage = this.RetornarMensagemValidacao(propriedade, paiPropriedade, valorPropriedade);
                    resultado.ErrorMessage = this.ErrorMessage;
                }
            }
            return resultado;
        }

        #region IAtributoValidacao

        public string RetornarMensagemValidacao(PropertyInfo propriedade,
                                                object paiPropriedade,
                                                object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);

            var opcao = this.OpcoesComparacaoAuxiliar;
            if (opcao.HasValue && !this.IsIgnorarValidacaoSeAuxiliarInvalido && opcao != EnumOpcoesComparacaoAuxiliar.Nenhuma)
            {
                if (!this.IsIgnorarValidacaoSeAuxiliarInvalido &&
                    !this.IsAuxiliarValido(paiPropriedade, valorPropriedade))
                {
                    var valorPropriedadeAuxiliar = this.RetornarValorPropriedadeAuxilizar(paiPropriedade);
                    var rotutloPropriedadeAuxiliar = ReflexaoUtil.RetornarRotulo(this.PropriedadeAuxiliar); 
 
                    switch (opcao.Value)
                    {
                        case EnumOpcoesComparacaoAuxiliar.Igual:

                            return $"O campo {rotulo} deve ser igual a {rotutloPropriedadeAuxiliar}: '{valorPropriedadeAuxiliar}'.";
                        case EnumOpcoesComparacaoAuxiliar.Diferente:

                            return $"O campo {rotulo} deve ser diferente de {rotutloPropriedadeAuxiliar}: '{valorPropriedadeAuxiliar}'.";

                        case EnumOpcoesComparacaoAuxiliar.Maior:

                            return $"O campo {rotulo} deve ser maior que {rotutloPropriedadeAuxiliar}: '{valorPropriedadeAuxiliar}'.";

                        case EnumOpcoesComparacaoAuxiliar.Menor:

                            return $"O campo {rotulo} deve ser menor que {rotutloPropriedadeAuxiliar}: {valorPropriedadeAuxiliar}'.";

                        case EnumOpcoesComparacaoAuxiliar.MaiorIgual:

                            return $"O campo {rotulo} deve ser maior ou igual a {rotutloPropriedadeAuxiliar}: '{valorPropriedadeAuxiliar}'.";

                        case EnumOpcoesComparacaoAuxiliar.MenorIgual:

                            return $"O campo {rotulo} deve ser menor ou igual a  {rotutloPropriedadeAuxiliar}: '{valorPropriedadeAuxiliar}'.";

                        case EnumOpcoesComparacaoAuxiliar.True:

                            return $"O campo {this.NomePropridadeAuxiliar} deve ser verdadeiro.";

                        case EnumOpcoesComparacaoAuxiliar.False:

                            return $"O campo {this.NomePropridadeAuxiliar} deve ser falso.";
                    }
                }

            }
            return String.Format(MensagemValidacao, rotulo);
        }

        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (this.IsAuxiliarValido(paiPropriedade, valorPropriedade))
            {
                return ValidacaoUtil.IsValidacaoRequerido(propriedade, valorPropriedade, paiPropriedade);
            }
            return this.IsIgnorarValidacaoSeAuxiliarInvalido;
        }

        private bool IsAuxiliarValido(object paiPropriedade, object valorPropriedade)
        {
            if (this.OpcoesComparacaoAuxiliar == null ||
                this.OpcoesComparacaoAuxiliar == EnumOpcoesComparacaoAuxiliar.Nenhuma)
            {
                return true;
            }

            if (paiPropriedade == null)
            {
                return false;
            }

            var valorPropriedadeAuxiliar = this.RetornarValorPropriedadeAuxilizar(paiPropriedade);
            var valorPropriedadeComparar = this.ValorComparar ?? valorPropriedade;

            switch (this.OpcoesComparacaoAuxiliar.Value)
            {
                case EnumOpcoesComparacaoAuxiliar.True:

                    return Convert.ToBoolean(valorPropriedadeAuxiliar);

                case EnumOpcoesComparacaoAuxiliar.False:

                    return !Convert.ToBoolean(valorPropriedadeAuxiliar);

                case EnumOpcoesComparacaoAuxiliar.Igual:

                    return Util.SaoIgual(valorPropriedadeComparar, valorPropriedadeAuxiliar);

                case EnumOpcoesComparacaoAuxiliar.Diferente:

                    return !Util.SaoIgual(valorPropriedadeComparar, valorPropriedadeAuxiliar);

                case EnumOpcoesComparacaoAuxiliar.Maior:
                    {
                        if (valorPropriedadeAuxiliar is IComparable auxiliarComparable &&
                            valorPropriedadeComparar is IComparable valorComparable)
                        {
                            return valorComparable.CompareTo(auxiliarComparable) > 0;
                        }
                        if (valorPropriedadeComparar == null && valorPropriedadeAuxiliar != null)
                        {
                            return true;
                        }
                        return false;
                        //throw new Exception("A propriedade auxiliar e a propriedade devem implementar a interface IComparable.");
                    }

                case EnumOpcoesComparacaoAuxiliar.MaiorIgual:
                    {
                        if (valorPropriedadeAuxiliar is IComparable auxiliarComparable &&
                            valorPropriedadeComparar is IComparable valorComparable)
                        {
                            return valorComparable.CompareTo(auxiliarComparable) >= 0;
                        }
                        if (valorPropriedadeComparar == null && valorPropriedadeAuxiliar != null)
                        {
                            return true;
                        }
                        return false;
                    }

                case EnumOpcoesComparacaoAuxiliar.Menor:
                    {
                        if (valorPropriedadeAuxiliar is IComparable auxiliarComparable &&
                            valorPropriedadeComparar is IComparable valorComparable)
                        {
                            return valorComparable.CompareTo(auxiliarComparable) < 0;
                        }
                        if (valorPropriedadeComparar != null && valorPropriedadeAuxiliar == null)
                        {
                            return true;
                        }
                        return false;
                    }

                case EnumOpcoesComparacaoAuxiliar.MenorIgual:
                    {
                        if (valorPropriedadeAuxiliar is IComparable auxiliarComparable &&
                        valorPropriedadeComparar is IComparable valorComparable)
                        {
                            return valorComparable.CompareTo(auxiliarComparable) <= 0;
                        }

                        if (valorPropriedadeComparar != null && valorPropriedadeAuxiliar == null)
                        {
                            return true;
                        }
                        return false;
                    }
                default:
                    throw new NotSupportedException("Opção de validação não suportada." + this.OpcoesComparacaoAuxiliar);
            }
        }

        private object RetornarValorPropriedadeAuxilizar(object paiPropriedade)
        {

            var tipoPaiPropriedade = paiPropriedade.GetType();
            var propriedadeAuxiliar = this.PropriedadeAuxiliar;
            if (propriedadeAuxiliar.DeclaringType != tipoPaiPropriedade &&
                !propriedadeAuxiliar.DeclaringType.IsSubclassOf(tipoPaiPropriedade))
            {
                throw new Exception($"A propriedade auxiliar {propriedadeAuxiliar.Name} declarada em {propriedadeAuxiliar.DeclaringType.Name} não pertence a entidade {tipoPaiPropriedade.Name}.");
            }

            return this.PropriedadeAuxiliar.GetValue(paiPropriedade);
        }

        #endregion
    }

    public enum EnumOpcoesComparacaoAuxiliar
    {
        Nenhuma,
        True,
        False,
        Igual,
        Diferente,
        Maior,
        Menor,
        MaiorIgual,
        MenorIgual
    }
}