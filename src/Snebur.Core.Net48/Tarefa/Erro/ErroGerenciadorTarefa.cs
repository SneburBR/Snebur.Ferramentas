using System;
using System.Collections.Generic;

namespace Snebur.Tarefa
{
    public class ErroGerenciadorTarefa<TTarefa> : Erro where TTarefa : ITarefa
    {
        public GerenciadorTarefa<TTarefa> GerenciadorTarefas { get; set; }
        public List<TTarefa> Tarefas { get; set; }

        public ErroGerenciadorTarefa(List<TTarefa> tarefas)
        {
            this.Tarefas = tarefas;
        }
    }
}
