using Community.VisualStudio.Toolkit;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Linq;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snebur.VisualStudio
{
    public partial class MigrationWindowControl
    {
        public async Task CompilarAsync(Project projetoEntidades,
                                        Project projetoMigracao)
        {

            
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
           
            this.Log($"Limpando a solução");
            this.Log($"Compilando a solução");
            //dte.Solution.SolutionBuild.Build(true);

            this.Log($"Compilando o projeto {projetoEntidades.Name}");
            await VS.Build.BuildProjectAsync(projetoEntidades);
            this.Log($"Compilando o projeto {projetoMigracao.Name}");
            await VS.Build.BuildProjectAsync(projetoMigracao);

            System.Threading.Thread.Sleep(1500);
        }

        private async Task IniciarValidacaoAsync(Project projetoMigracao,
                                                 Project projetoEntidades,
                                                 bool isAdicionarMigracao,
                                                 bool isCompliar)
        {
            try
            {
                if(await this.AtualizandoConnectionStringEmTempoExecucaoAsync())
                {
                    return;
                }

                await this.CompilarAsync(projetoEntidades, 
                                         projetoMigracao);

                await WorkThreadUtil.SwitchToWorkerThreadAsync();


                var assemblyEntidades = AjudanteAssemblyEx.RetornarAssembly(projetoEntidades);
                var assemblyMigracao = AjudanteAssemblyEx.RetornarAssembly(projetoMigracao);

                var tiposEntidade = assemblyEntidades.GetAccessibleTypes().Where(x => TipoUtil.TipoIgualOuSubTipo(x, typeof(Entidade))).ToList();

                var tiposSemAtributoTabela = tiposEntidade.Where(x => !TipoUtil.TipoPossuiAtributo(x, typeof(TabelaAttribute), true)).ToList();
                if (tiposSemAtributoTabela.Count > 0)
                {
                    throw new Exception($"O(s) tipo(s) não possui o atributo tabela  {String.Join(", ", tiposSemAtributoTabela)} ");
                }

                var isNaoValidarNomeTabela = assemblyEntidades.GetCustomAttributes().
                                                               Where(x => x.GetType().Name == nameof(NaoValidarNomeTabelaAttribute)).
                                                               SingleOrDefault() != null;

                if (isNaoValidarNomeTabela == false)
                {
                    var tiposEntidadeNomeTabelaDiferenta = tiposEntidade.Where(x => x.Name != this.NomeTabela(x)).ToList();
                    if (tiposEntidadeNomeTabelaDiferenta.Count > 0)
                    {
                        throw new Exception($"O(s) tipo(s) possui o nome diferente do nome da tabela  {String.Join(", ", tiposEntidadeNomeTabelaDiferenta)}");
                    }
                }


                var tiposValidacaoUnicoComposta = tiposEntidade.Where(x => TipoUtil.TipoPossuiAtributo(x, typeof(ValidacaoUnicoCompostaAttribute), true)).ToList();
                foreach (var tipoEntidade in tiposValidacaoUnicoComposta)
                {
                    var atributos = TipoUtil.RetornarAtributos(tipoEntidade, typeof(ValidacaoUnicoCompostaAttribute), true);
                    foreach (var atributo in atributos)
                    {
                        var tipoEntidadeAtributo = (Type)ReflexaoUtil.RetornarValorPropriedade<ValidacaoUnicoCompostaAttribute>(x => x.TipoEntidade, atributo);

                        if (tipoEntidadeAtributo == null)
                        {
                            throw new Exception($"A {tipoEntidade.Name}, Construtor do atributo {nameof(ValidacaoUnicoCompostaAttribute)}, utilizar o construtor passando o tipo como primeiro argumento ");
                        }
                        if (tipoEntidade.Name != tipoEntidadeAtributo.Name)
                        {
                            throw new Exception($"O entidade {tipoEntidade.Name},atributo {nameof(ValidacaoUnicoCompostaAttribute)}, o tipo do construtor é diferente do tipo da entidade ");
                        }
                    }

                }
                var validacoes = new HashSet<string>();
                var alertas = new HashSet<string>();
                foreach (var tipoEntidade in tiposEntidade)
                {
                    var instancia = tipoEntidade.IsAbstract ? null : Activator.CreateInstance(tipoEntidade);
                    var nomeEntidade = tipoEntidade.Name;

                    foreach (var propriedade in tipoEntidade.GetProperties())
                    {
                        if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(IgnorarValidacaoRelacaoAttribute)))
                        {
                            this.ValidarPropriedade(tipoEntidade,
                                                    instancia,
                                                    propriedade,
                                                    validacoes,
                                                    alertas);
                        }
                    }

                    if (TipoUtil.TipoImplementaInterface(tipoEntidade, typeof(IDeletado)))
                    {
                        foreach (var propriedade in tipoEntidade.GetProperties().Where(x =>
                        PropriedadeUtil.PossuiAtributo(x, typeof(ValidacaoUnicoAttribute))))
                        {
                            if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(ValorDeletadoConcatenarGuidAttribute)))
                            {
                                throw new Exception($"A entidade {tipoEntidade.Name},  implementa a interface {nameof(IDeletado)}," +
                                                    $" a propriedade {propriedade.Name} possui  {nameof(ValidacaoUnicoAttribute)}  " +
                                                    $"  adicionar o atributo  {nameof(ValorDeletadoConcatenarGuidAttribute)}" +
                                                    $" ou  remover o atributo o {nameof(ValidacaoUnicoAttribute)} de {propriedade.Name}," +
                                                    $" e utilizar {nameof(ValidacaoUnicoCompostaAttribute)} com propriedade  a {nameof(IDeletado.DataHoraDeletado)} ");
                            }
                        }
                    }

                    var propriedadesRelacaoNn = tipoEntidade.GetProperties().Where(x => PropriedadeUtil.PossuiAtributo(x, typeof(RelacaoNnAttribute))).ToList();
                    foreach (var propriedade in propriedadesRelacaoNn)
                    {
                        if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(NaoMapearAttribute)))
                        {
                            throw new Exception($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui o atributo {nameof(NaoMapearAttribute)} ");
                        }
                    }
                }
                if (validacoes.Count > 0)
                {
                    throw new Exception(String.Join(Environment.NewLine, validacoes));
                }

                if (alertas.Count > 0)
                {
                    this.LogAlerta(String.Join(Environment.NewLine, alertas));
                }

                this.LogSucesso("Validação concluída com sucesso");

                if (isAdicionarMigracao)
                {
                    var proximaMigracao = this.RetornarProximaMigracao(assemblyMigracao);
                    this.LogSucesso($"Adicionando migração  {proximaMigracao}");

                    //dte.Solution.Properties.Item("StartupProject").Value = projetoMigracao.Name;

                    System.Threading.Thread.Sleep(2000);


                    await VS.Commands.ExecuteAsync("View.PackageManagerConsole");

                    var comando = $"add-migration {proximaMigracao} -Project {projetoMigracao.Name} -StartupProject {projetoMigracao.Name} ";
                    System.Threading.Thread.Sleep(600);
                    System.Windows.Forms.SendKeys.SendWait(comando);
                    System.Threading.Thread.Sleep(600);
                    System.Windows.Forms.SendKeys.SendWait("{ENTER}");

                }
            }
            catch (Exception ex)
            {
                var mensagem = ErroUtil.RetornarMensagem(ex);
                this.LogErro(mensagem);
            }
            finally
            {
                _ = this.DesocuparAsync();

            }
        }

        private void ValidarPropriedade(Type tipoEntidade,
                                        object instancia,
                                        PropertyInfo propriedade,
                                        HashSet<string> validacoes,
                                        HashSet<string> alertas)
        {

            if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(NaoMapearAttribute)))
            {
                if (TipoUtil.TipoIgualOuSubTipo(propriedade.PropertyType, typeof(Entidade)))
                {
                    if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(ChaveEstrangeiraAttribute)))
                    {
                        if (!propriedade.IsOverride())
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui o atributo {nameof(ChaveEstrangeiraAttribute)} ");
                        }
                    }
                    else
                    {
                        var atributoChaveEstrangeira = PropriedadeUtil.RetornarAtributo(propriedade, typeof(ChaveEstrangeiraAttribute), true);
                        var nomePropriedade = ReflexaoUtil.RetornarNomePropriedade<ChaveEstrangeiraAttribute>(x => x.NomePropriedade);
                        var propriedadeNomePropriedade = atributoChaveEstrangeira.GetType().GetProperty(nomePropriedade);
                        var nomePropriedadeChaveEstrangeira = (string)propriedadeNomePropriedade.GetValue(atributoChaveEstrangeira);

                        //var nomePropriedadeChaveEstrangeira = (string)ReflexaoUtil.RetornarValorPropriedade<ChaveEstrangeiraAttribute>(x => x.NomePropriedade, atributo);

                        var propriedadeChaveEstrangeira = tipoEntidade.GetProperty(nomePropriedadeChaveEstrangeira);
                        if (propriedadeChaveEstrangeira == null)
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}, não foi encontrada a propriedade '{nomePropriedadeChaveEstrangeira}' da chave estrangeira ");
                        }
                        else
                        {
                            if (PropriedadeUtil.PossuiAtributo(propriedade, typeof(ValidacaoRequeridoAttribute)))
                            {
                                if (ReflexaoUtil.IsTipoNullable(propriedadeChaveEstrangeira.PropertyType))
                                {
                                    validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} possui o atributo {nameof(ValidacaoRequeridoAttribute)}. Remover o Nullable? da propriedade chave estrangeira {propriedadeChaveEstrangeira.Name} ");
                                }
                            }
                            else
                            {
                                if (!ReflexaoUtil.IsTipoNullable(propriedadeChaveEstrangeira.PropertyType))
                                {
                                    validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui o atributo {nameof(ValidacaoRequeridoAttribute)}. Atricionar o Nullable? da propriedade chave estrangeira {propriedadeChaveEstrangeira.Name} ");
                                }
                            }
                        }
                    }

                    if (!propriedade.PropertyType.Name.Contains(propriedade.Name) &&
                        !propriedade.Name.Contains(propriedade.PropertyType.Name))
                    {
                        alertas.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}, o nome da propriedade é diferente do seu tipo {propriedade.PropertyType.Name} ");
                    }

                    if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoPaiAttribute)) &&
                        !PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoUmUmAttribute)))
                    {
                        if (!propriedade.IsOverride())
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}, não possui o atributo de relacao ");
                        }
                    }
                }

                if (TipoUtil.TipoIgualOuSubTipo(propriedade.PropertyType, typeof(string)))
                {
                    if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(ValidacaoTextoTamanhoAttribute)))
                    {
                        validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui o atributo {nameof(ValidacaoTextoTamanhoAttribute)} ");
                    }
                }
            }

            if (ReflexaoUtil.TipoRetornaColecao(propriedade.PropertyType) && propriedade.PropertyType.IsGenericType)
            {
                var tipoItemEntidade = propriedade.PropertyType.GetGenericArguments().First();
                if (TipoUtil.TipoIgualOuSubTipo(tipoItemEntidade, typeof(Entidade)))
                {
                    if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoNnAttribute)) &&
                        !PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoFilhosAttribute)))
                    {
                        validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui o atributo de relação, esperado '{nameof(RelacaoFilhosAttribute)}' ou {nameof(RelacaoFilhosAttribute)} ");
                    }

                    if (propriedade.SetMethod == null || !propriedade.SetMethod.IsPublic)
                    {
                        validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui está implementado o método SET, necessário para a serialização");
                    }

                    if (PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoNnAttribute)) &&
                        PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoFilhosAttribute)))
                    {
                        validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} possui ambos atributos, somente um é permitido '{nameof(RelacaoFilhosAttribute)}' ou {nameof(RelacaoFilhosAttribute)} ");
                    }

                    if (PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoNnAttribute)))
                    {
                        if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(NaoMapearAttribute)))
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name} não possui o atributo {nameof(NaoMapearAttribute)} ");
                        }

                        var atributoRelacaoNn = PropriedadeUtil.RetornarAtributo(propriedade, typeof(RelacaoNnAttribute), true);
                        var nomePropriedadeTipoEntidadeRelacao = ReflexaoUtil.RetornarNomePropriedade<RelacaoNnAttribute>(x => x.TipoEntidadeRelacao);
                        var propriedadeTipoEntidadeRelacao = atributoRelacaoNn.GetType().GetProperty(nomePropriedadeTipoEntidadeRelacao);
                        var tipoEntidadeRelacao = (Type)propriedadeTipoEntidadeRelacao.GetValue(atributoRelacaoNn);

                        if (!tipoEntidadeRelacao.GetProperties().Any(x => TipoUtil.TipoIgualOuSubTipo(tipoEntidade, x.PropertyType) || TipoUtil.TipoIgualOuSubTipo(x.PropertyType, tipoEntidade)))
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}. A relação Nn. não foi encontrada a propriedade do tipo {tipoEntidade.Name} na entidade de relação {tipoEntidadeRelacao.Name}");
                        }

                        if (!tipoEntidadeRelacao.GetProperties().Any(x => TipoUtil.TipoIgualOuSubTipo(tipoItemEntidade, x.PropertyType) || TipoUtil.TipoIgualOuSubTipo(x.PropertyType, tipoItemEntidade)))
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}. A relação Nn. não foi encontrada a propriedade  do tipo {tipoItemEntidade.Name} na entidade de relação {tipoEntidadeRelacao.Name}");
                        }

                    }

                    if (PropriedadeUtil.PossuiAtributo(propriedade, typeof(RelacaoFilhosAttribute)))
                    {
                        var propriedadesRelacao = tipoItemEntidade.GetProperties().Where(x => TipoUtil.TipoIgualOuSubTipo(tipoEntidade, x.PropertyType) || TipoUtil.TipoIgualOuSubTipo(x.PropertyType, tipoEntidade)).ToList();
                        if (propriedadesRelacao.Count == 0)
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}. A relação filhos. não foi encontrada a propriedade de relação na entidade {tipoItemEntidade.Name}");
                        }

                        var atributoRelacaoFilho = PropriedadeUtil.RetornarAtributo(propriedade, typeof(RelacaoFilhosAttribute), true);
                        var nomePropriedadeChaveEstrangeira = ReflexaoUtil.RetornarNomePropriedade<RelacaoFilhosAttribute>(x => x.NomePropriedadeChaveEstrangeira);
                        var propriedadeNomePropriedadeChaveEstrangeira = atributoRelacaoFilho.GetType().GetProperty(nomePropriedadeChaveEstrangeira);
                        var valorNomePropriedadeChaveEstrangeira = (string)propriedadeNomePropriedadeChaveEstrangeira.GetValue(atributoRelacaoFilho);

                        if (propriedadesRelacao.Count > 1)
                        {
                            if (!PropriedadeUtil.PossuiAtributo(propriedade, typeof(NaoMapearAttribute)))
                            {
                                validacoes.Add($"Adicione o atributo {nameof(NaoMapearAttribute)}  propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}. Isso é necessário existe mais de um RelacaoFilhos para a entidade {tipoItemEntidade.Name}.");
                            }


                            if (String.IsNullOrWhiteSpace(valorNomePropriedadeChaveEstrangeira?.ToString()))
                            {
                                validacoes.Add($"Adicione o  nome do propriedade chave estrangeira no atributo  {nameof(RelacaoFilhosAttribute)} na  propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name}. Isso é necessário existe mais de um RelacaoFilhos para a entidade {tipoItemEntidade.Name}.");
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(valorNomePropriedadeChaveEstrangeira))
                        {
                            var propriedadeChaveEstrangeira = tipoItemEntidade.GetProperty(valorNomePropriedadeChaveEstrangeira);
                            if (propriedadeChaveEstrangeira == null)
                            {
                                validacoes.Add($" RelacaoFilhos {propriedade.DeclaringType.Name}.{propriedade.Name}. A propriedade da relação chave estrangeira '{valorNomePropriedadeChaveEstrangeira}' não foi encontrada em {tipoItemEntidade.Name}.");
                            }
                        }

                    }
                }
            }
            if (!tipoEntidade.IsAbstract)
            {
                if (ReflexaoUtil.TipoRetornaColecao(propriedade.PropertyType) && propriedade.PropertyType.IsGenericType)
                {
                    var tipoItem = propriedade.PropertyType.GetGenericArguments().First();
                    if (TipoUtil.TipoIgualOuSubTipo(tipoItem, typeof(Entidade)))
                    {
                        if (propriedade.GetValue(instancia) == null)
                        {
                            validacoes.Add($"A propriedade {propriedade.Name} na entidade {propriedade.DeclaringType.Name},A coleção <'{tipoItem.Name}'> não foi instancia na declaração ");
                        }
                    }
                }
            }
        }
    }
}