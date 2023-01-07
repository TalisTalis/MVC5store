using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVC_magic_store.Models.Data
{
    [Table("tblRoles")]
    public class RoleDTO
    {
        //[Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}