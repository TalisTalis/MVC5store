using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_magic_store.Models.Data
{
    [Table("tblUserRoles")]
    public class UserRoleDTO
    {
        [Key,Column(Order = 0)] // так как оба поля ключевые для компилятора устанавливаем порядок выполнения
        public int UserId { get; set; }

        [Key,Column(Order = 1)] // так как оба поля ключевые для компилятора устанавливаем порядок выполнения
        public int RoleId { get; set; }

        // устанавливаем связь между таблицами
        [ForeignKey("UserId")]
        public virtual UserDTO User { get; set; }
        [ForeignKey("RoleId")]
        public virtual RoleDTO Role { get; set; }
    }
}