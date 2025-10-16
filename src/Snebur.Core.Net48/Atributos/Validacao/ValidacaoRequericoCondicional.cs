//using Snebur.Utilidade;
//using System;
//using System.ComponentModel.DataAnnotations;
//using System.Reflection;

//namespace Snebur.Dominio.Atributos
//{
//    [AttributeUsage(AttributeTargets.Property)]
//    public class ValidacaoRequericoCondicionalAttribute : RequiredAttribute, IAtributoValidacao
//    {
//        [MensagemValidacao]
//        public static string MensagemValidacao { get; set; } = "O campo {0} deve ser preenchido.";
//        public EnumOpcoesComparacaoAuxiliar? OpcoesComparacaoAuxiliar { get; set; } = null;
//        public string NomePropridadeAuxiliar { get; set; }

//        public bool IsValidoSeAuxiliarInvalido { get; set; } = false;
//        private PropertyInfo PropriedadeAuxiliar { get; set; }

      
 
//        public ValidacaoRequericoCondicionalAttribute([IgnorarParametroTS] Type tipoEntidade,
//                                                      [ParametroOpcionalTS] EnumOpcoesComparacaoAuxiliar opcoesComparacaoAuxiliar,
//                                                      [ParametroOpcionalTS] string nomePropridadeAuxiliar,
//                                                      [ParametroOpcionalTS] bool isValidoSeAuxiliarInvalido)
//        {
//            this.OpcoesComparacaoAuxiliar = opcoesComparacaoAuxiliar;
//            this.NomePropridadeAuxiliar = nomePropridadeAuxiliar;
//            this.IsValidoSeAuxiliarInvalido = isValidoSeAuxiliarInvalido;

//            if (opcoesComparacaoAuxiliar != EnumOpcoesComparacaoAuxiliar.Nenhuma)
//            {
//                this.PropriedadeAuxiliar = this.RetornarPropriedadeAuxiliar(tipoEntidade);
//            }
//        }

//        private PropertyInfo RetornarPropriedadeAuxiliar(Type tipoEntidade)
//        {
//            if (String.IsNullOrWhiteSpace(this.NomePropridadeAuxiliar))
//            {
//                throw new Exception("O nome da propriedade auxiliar deve ser informado, quando opção de validação for diferente de nenhuma.");
//            }
//            var propriedade = tipoEntidade.GetProperty(this.NomePropridadeAuxiliar);
//            if (propriedade == null)
//            {
//                throw new Exception("A propriedade auxiliar informada não foi encontrada.");
//            }
//            return propriedade;
//        }

//        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//        {
//            var resultado = base.IsValid(value, validationContext);
//            if (resultado != null)
//            {
//                var propriedade = validationContext.ObjectType.GetProperty(validationContext.MemberName);
//                var paiPropriedade = validationContext.ObjectInstance;
//                var valorPropriedade = value;
//                if (this.IsValido(propriedade, paiPropriedade, valorPropriedade))
//                {
//                    return ValidationResult.Success;
//                }
//                else
//                {
//                    this.ErrorMessage = this.RetornarMensagemValidacao(propriedade, paiPropriedade, valorPropriedade);
//                    resultado.ErrorMessage = this.ErrorMessage;
//                }
//            }
//            return resultado;
//        }

//        #region IAtributoValidacao

//        public string RetornarMensagemValidacao(PropertyInfo propriedade,
//                                                object paiPropriedade,
//                                                object valorPropriedade)
//        {
//            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);

//            var opcao = this.OpcoesComparacaoAuxiliar;
//            if (opcao.HasValue && opcao != EnumOpcoesComparacaoAuxiliar.Nenhuma)
//            {
//                var valorPropriedadeAuxiliar = this.RetornarValorPropriedadeAuxilizar(paiPropriedade);
//                switch (opcao.Value)
//                {
//                    case EnumOpcoesComparacaoAuxiliar.Igual:

//                        return $"O campo {rotulo} deve ser igual a '{valorPropriedadeAuxiliar}'.";
//                    case EnumOpcoesComparacaoAuxiliar.Diferente:

//                        return $"O campo {rotulo} deve ser diferente de '{valorPropriedadeAuxiliar}'.";

//                    case EnumOpcoesComparacaoAuxiliar.Maior:

//                        return $"O campo {rotulo} deve ser maior que '{valorPropriedadeAuxiliar}'.";

//                    case EnumOpcoesComparacaoAuxiliar.Menor:

//                        return $"O campo {rotulo} deve ser menor que '{valorPropriedadeAuxiliar}'.";

//                    case EnumOpcoesComparacaoAuxiliar.MaiorIgual:

//                        return $"O campo {rotulo} deve ser maior ou igual a '{valorPropriedadeAuxiliar}'.";

//                    case EnumOpcoesComparacaoAuxiliar.MenorIgual:

//                        return $"O campo {rotulo} deve ser menor ou igual a '{valorPropriedadeAuxiliar}'.";

//                    case EnumOpcoesComparacaoAuxiliar.True:

//                        return $"O campo {this.NomePropridadeAuxiliar} deve ser verdadeiro.";

//                    case EnumOpcoesComparacaoAuxiliar.False:

//                        return $"O campo {this.NomePropridadeAuxiliar} deve ser falso.";
//                }
//            }
//            return String.Format(MensagemValidacao, rotulo);
//        }

//        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
//        {
//            if (this.IsAuxiliarValido(paiPropriedade, valorPropriedade))
//            {
//                return ValidacaoUtil.IsValidacaoRequerido(propriedade, valorPropriedade, paiPropriedade);
//            }

//            if (this.IsValidoSeAuxiliarInvalido)
//            {
//                return true;
//            }
//            return false;
//        }

//        private bool IsAuxiliarValido(object paiPropriedade, object valorPropriedade)
//        {
//            if (this.OpcoesComparacaoAuxiliar == null ||
//                this.OpcoesComparacaoAuxiliar == EnumOpcoesComparacaoAuxiliar.Nenhuma)
//            {
//                return true;
//            }

//            if (paiPropriedade == null)
//            {
//                return false;
//            }

//            var valorPropriedadeAuxiliar = this.RetornarValorPropriedadeAuxilizar(paiPropriedade);

//            switch (this.OpcoesComparacaoAuxiliar.Value)
//            {
//                case EnumOpcoesComparacaoAuxiliar.True:

//                    return Convert.ToBoolean(valorPropriedadeAuxiliar);
//                case EnumOpcoesComparacaoAuxiliar.False:

//                    return !Convert.ToBoolean(valorPropriedadeAuxiliar);

//                case EnumOpcoesComparacaoAuxiliar.Igual:

//                    return Util.SaoIgual(valorPropriedade, valorPropriedadeAuxiliar);

//                case EnumOpcoesComparacaoAuxiliar.Diferente:

//                    return !Util.SaoIgual(valorPropriedade, valorPropriedadeAuxiliar);

//                case EnumOpcoesComparacaoAuxiliar.Maior:
//                    {
//                        if (valorPropriedadeAuxiliar is IComparable auxiliarComparable &&
//                            valorPropriedade is IComparable valorComparable)
//                        {
//                            return valorComparable.CompareTo(auxiliarComparable) > 0;
//                        }
//                        if (valorPropriedade == null && valorPropriedadeAuxiliar != null)
//                        {
//                            return true;
//                        }
//                        return false;
//                        //throw new Exception("A propriedade auxiliar e a propriedade devem implementar a interface IComparable.");
//                    }

//                case EnumOpcoesComparacaoAuxiliar.MaiorIgual:
//                    {
//                        if (valorPropriedadeAuxiliar is IComparable auxiliarComparable &&
//                            valorPropriedade is IComparable valorComparable)
//                        {
//                            return valorComparable.CompareTo(auxiliarComparable) >= 0;
//                        }
//                        if (valorPropriedade == null && valorPropriedadeAuxiliar != null)
//                        {
//                            return true;
//                        }
//                        return false;
//                    }

//                case EnumOpcoesComparacaoAuxiliar.Menor:
//                    {
//                        if (valorPropriedadeAuxiliar is IComparable auxiliarComparable &&
//                            valorPropriedade is IComparable valorComparable)
//                        {
//                            return valorComparable.CompareTo(auxiliarComparable) < 0;
//                        }
//                        if (valorPropriedade != null && valorPropriedadeAuxiliar == null)
//                        {
//                            return true;
//                        }
//                        return false;
//                    }

//                case EnumOpcoesComparacaoAuxiliar.MenorIgual:
//                    {
//                        if (valorPropriedadeAuxiliar is IComparable auxiliarComparable &&
//                        valorPropriedade is IComparable valorComparable)
//                        {
//                            return valorComparable.CompareTo(auxiliarComparable) <= 0;
//                        }

//                        if (valorPropriedade != null && valorPropriedadeAuxiliar == null)
//                        {
//                            return true;
//                        }
//                        return false;
//                    }
//                default:
//                    throw new NotSupportedException("Opção de validação não suportada." + this.OpcoesComparacaoAuxiliar);
//            }
//        }

//        private object RetornarValorPropriedadeAuxilizar(object paiPropriedade)
//        {
//            var tipoPaiPropriedade = paiPropriedade.GetType();
//            var propriedadeAuxiliar = this.PropriedadeAuxiliar;
//            if (propriedadeAuxiliar.DeclaringType != tipoPaiPropriedade &&
//                !propriedadeAuxiliar.DeclaringType.IsSubclassOf(tipoPaiPropriedade))
//            {
//                throw new Exception($"A propriedade auxiliar {propriedadeAuxiliar.Name} declarada em {propriedadeAuxiliar.DeclaringType.Name} não pertence a entidade {tipoPaiPropriedade.Name}.");
//            }

//            return this.PropriedadeAuxiliar.GetValue(paiPropriedade);
//        }

//        #endregion
//    }
     