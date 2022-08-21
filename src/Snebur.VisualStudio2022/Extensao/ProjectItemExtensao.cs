using System;

namespace EnvDTE
{
    public static class ProjectItemExtensao
    {
        public static ProjectItem RetornarProjectItemPai(this ProjectItem projectItem)
        {
            var encontrarPai = new EncontrarPai(projectItem);
            return encontrarPai.RetornarProjectItemPai();
        }

        private class EncontrarPai
        {
            private ProjectItem ProjectItem;

            public Project Projecto { get; }

            public EncontrarPai(ProjectItem projectItem)
            {
                this.ProjectItem = projectItem;
                this.Projecto = projectItem.ContainingProject;
            }

            public ProjectItem RetornarProjectItemPai()
            {
                foreach (ProjectItem item in this.Projecto.ProjectItems)
                {
                    var projectItem = this.VarrerProjectItens(item);
                    if (projectItem != null)
                    {
                        return projectItem;
                    }

                }
                throw new Exception("O project item pai não foi encontrado ");
            }

            private ProjectItem VarrerProjectItens(ProjectItem projectItem)
            {
                foreach (ProjectItem item in projectItem.ProjectItems)
                {
                    var nome = item.Name;
                    if (item == this.ProjectItem)
                    {
                        return projectItem;
                    }
                    var projectItemFilho = this.VarrerProjectItens(item);
                    if (projectItemFilho != null)
                    {
                        return projectItemFilho;
                    }
                }
                return null;

            }
        }
    }
}
