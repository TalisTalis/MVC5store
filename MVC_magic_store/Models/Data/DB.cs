using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVC_magic_store.Models.Data
{
    public class DB : DbContext
    {                   
        // проложить мостик от базы данных к модели к сущности
        public DbSet<PagesDTO> Pages { get; set; } // сбор информации из pagesDTO

        // связать модель sidebarDTO с БД
        public DbSet<SidebarDTO> Sidebars { get; set; } // сбор информации из sidebarDTO

        // подключение таблицы Categories
        public DbSet<CategoryDTO> Categories { get; set; }

        // подключение таблицы Products
        public DbSet<ProductDTO> Products { get; set; }

        // подключение таблицы Users
        public DbSet<UserDTO> Users { get; set; }

        // подключение таблицы Roles
        public DbSet<RoleDTO> Roles { get; set; }
    }
}