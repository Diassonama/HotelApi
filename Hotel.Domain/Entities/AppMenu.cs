using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;
using Hotel.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Domain.Entities
{
    public class AppMenu : BaseDomainEntity
    {
        public string PreIcon { get; private set; }
        public string PostIcon { get; private set; }
        public string Nome { get; private set; }
        public string Path { get; private set; }
        public ICollection<MenuRole> MenuRole { get; private set; } = new List<MenuRole>();

        // Construtor para inicialização
        private AppMenu() { }

        public AppMenu(string preIcon, string postIcon, string nome, string path)
        {
            Validate(preIcon, postIcon, nome, path);

            PreIcon = preIcon;
            PostIcon = postIcon;
            Nome = nome;
            Path = path;
        }

        // Método para adicionar MenuRole
        public void AddMenuRole(MenuRole menuRole)
        {
            if (menuRole == null)
                throw new ArgumentNullException(nameof(menuRole));

            MenuRole.Add(menuRole);
        }

        // Método para remover MenuRole
        public void RemoveMenuRole(MenuRole menuRole)
        {
            if (menuRole == null)
                throw new ArgumentNullException(nameof(menuRole));

            MenuRole.Remove(menuRole);
        }

        // Método para atualizar propriedades do AppMenu
        /* public AppMenu(int id, string preIcon, string postIcon, string nome, string path)
        {
            Validate(preIcon, postIcon, nome, path);
            Id = Id;
            PreIcon = preIcon;
            PostIcon = postIcon;
            Nome = nome;
            Path = path;
        } */
        public void Update(int id) //, string preIcon, string postIcon, string nome, string path)
        {
           // Validate(preIcon, postIcon, nome, path);
            Id = id;
           /*  PreIcon = preIcon;
            PostIcon = postIcon;
            Nome = nome;
            Path = path; */
        }
        public void UpdateIcons(string preIcon, string postIcon)
        {
            PreIcon = preIcon;
            PostIcon = postIcon;
        }
        // Método de validação
        private void Validate(string preIcon, string postIcon, string nome, string path)
        {
            if (string.IsNullOrWhiteSpace(preIcon))
                throw new ArgumentException("PreIcon é obrigatório.", nameof(preIcon));

            if (string.IsNullOrWhiteSpace(postIcon))
                throw new ArgumentException("PostIcon é obrigatório.", nameof(postIcon));

            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório.", nameof(nome));

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path é obrigatório.", nameof(path));
        }
    }

}