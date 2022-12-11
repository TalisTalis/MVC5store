using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_magic_store.Models.Data
{
    [Table("tblSidebar")]
    public class SidebarDTO
    {
        // реализовать автоматические свойства с гетером и сетером по типу полей из БД
        [Key]
        public int Id { get; set; }

        public string Body { get; set; }
    }
}