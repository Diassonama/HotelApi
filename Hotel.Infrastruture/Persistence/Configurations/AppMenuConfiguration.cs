using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class AppMenuConfiguration : IEntityTypeConfiguration<AppMenu>
    {
        public void Configure(EntityTypeBuilder<AppMenu> builder)
        {
/*
            builder.HasData(
new AppMenu(1,"hotel",  "account_box", "Home", "home"),
new AppMenu(2,"account_box", "account_box", "Profile", "profile"),
    new AppMenu(3,"view_agenda", "view_agenda", "Rack", "Rack"),
    new AppMenu(4,"account_circle", "supervisor_account", "Inquilino", "inquilino"),
    new AppMenu(5,"chrome_reader_mode", "chrome_reader_mode", "Produtos/Serviços", "produtos"),
    new AppMenu(6,"bookmark", "bookmark", "Tela", "tela"),
    new AppMenu(7,"assignment", "assignment", "Contratos", "contrato"),
    new AppMenu(8,"account_balance", "account_balance", "Caixa", "caixa"),
    new AppMenu(9,"hotel", "hotel", "Apartamentos", "apartamento"),
    new AppMenu(10,"bookmark", "bookmark", "Disciplina", "disciplina"),
    new AppMenu(11,"location_city", "location_city", "Edíficios", "edificio"),
    new AppMenu(12,"menu", "menu", "Menu", "menu"),
    new AppMenu(13,"widgets", "widgets", "Param", "param"),
    new AppMenu(14,"perm_identity", "perm_identity", "Perfil", "perfil"),
    new AppMenu(15,"face", "user", "Utilizador", "utilizador"),
    new AppMenu(16,"lock", "key", "Alterar Senha", "alterasenha"),
    new AppMenu(17,"list", "list", "Gestão de Permissões", "permissoes"),
    new AppMenu(18,"bookmark", "bookmark", "Venda", "venda"),
    new AppMenu(19,"bookmark", "bookmark", "Tipos de Documento", "tipoDoc"),
    new AppMenu(20,"bookmark", "bookmark", "Cursos", "curso"),
    new AppMenu(21,"bookmark", "bookmark", "Documentos Secretária", "documentoSecretaria"),
    new AppMenu(22,"account_circle", "account_circle", "Funcionários", "funcionario"),
    new AppMenu(23,"account_circle", "account_circle", "Lançamento de Notas", "nota")
    */
/*
                     new AppMenu { Id = 1, PreIcon = "hotel", PostIcon = "account_box", Nome = "Home", Path = "home" },
                    new AppMenu { Id = 2, PreIcon = "account_box", PostIcon = "account_box", Nome = "Profile", Path = "profile" },
                    new AppMenu { Id = 3, PreIcon = "view_agenda", PostIcon = "view_agenda", Nome = "Rack", Path = "Rack" },
                    new AppMenu { Id = 4, PreIcon = "account_circle", PostIcon = "supervisor_account", Nome = "Inquilino", Path = "inquilino" },
                    new AppMenu { Id = 5, PreIcon = "chrome_reader_mode", PostIcon = "chrome_reader_mode", Nome = "Produtos/Serviços", Path = "produtos" },
                    new AppMenu { Id = 6, PreIcon = "bookmark", PostIcon = "bookmark", Nome = "Tela", Path = "tela" },
                    new AppMenu { Id = 7, PreIcon = "assignment", PostIcon = "assignment", Nome = "Contratos", Path = "contrato" },
                    new AppMenu { Id = 8, PreIcon = "account_balance", PostIcon = "account_balance", Nome = "Caixa", Path = "caixa" },
                    new AppMenu { Id = 9, PreIcon = "hotel", PostIcon = "hotel", Nome = "Apartamentos", Path = "apartamento" },
                    new AppMenu { Id = 10, PreIcon = "bookmark", PostIcon = "bookmark", Nome = "Disciplina", Path = "disciplina" },
                    new AppMenu { Id = 11, PreIcon = "location_city", PostIcon = "location_city", Nome = "Edíficios", Path = "edificio" },
                    new AppMenu { Id = 12, PreIcon = "menu", PostIcon = "menu", Nome = "Menu", Path = "menu" },
                    new AppMenu { Id = 13, PreIcon = "widgets", PostIcon = "widgets", Nome = "Param", Path = "param" },
                    new AppMenu { Id = 14, PreIcon = "perm_identity", PostIcon = "perm_identity", Nome = "Perfil", Path = "perfil" },
                    new AppMenu { Id = 15, PreIcon = "face", PostIcon = "user", Nome = "Utilizador", Path = "utilizador" },
                    new AppMenu { Id = 16, PreIcon = "lock", PostIcon = "key", Nome = "Alterar Senha", Path = "alterasenha" },
                    new AppMenu { Id = 17, PreIcon = "list", PostIcon = "list", Nome = "Gestão de Permissões", Path = "permissoes" },
                    new AppMenu { Id = 18, PreIcon = "bookmark", PostIcon = "bookmark", Nome = "Venda", Path = "venda" },
                    new AppMenu { Id = 19, PreIcon = "bookmark", PostIcon = "bookmark", Nome = "Tipos de Documento", Path = "tipoDoc" },
                    new AppMenu { Id = 20, PreIcon = "bookmark", PostIcon = "bookmark", Nome = "Cursos", Path = "curso" },
                    new AppMenu { Id = 21, PreIcon = "bookmark", PostIcon = "bookmark", Nome = "Documentos Secretária", Path = "documentoSecretaria" },
                    new AppMenu { Id = 22, PreIcon = "account_circle", PostIcon = "account_circle", Nome = "Funcionários", Path = "funcionario" },
                    new AppMenu { Id = 23, PreIcon = "account_circle", PostIcon = "account_circle", Nome = "Lançamento de Notas", Path = "nota" }
 
            )
  */          
        }
    }
}