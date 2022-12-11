using MVC_magic_store.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_magic_store.Models.ViewModels.Pages
{
    public class SidebarVM
    {
        // создать два конструктора: один без параметров по молчанию и с параметром
        public SidebarVM() 
        { 

        }

        public SidebarVM (SidebarDTO row)
        {
            Id = row.Id;
            Body = row.Body;
        }

        // копируем свойства из sidebarDTO
        public int Id { get; set; }

        public string Body { get; set; }
    }
}