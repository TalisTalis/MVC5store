using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_magic_store.Models.Data
{
    // создание класса контекста данных для категорий товаров
    // модель DataBaseFirst
    [Table("tblCategories")]
    public class CategoryDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; } // краткое описание
        public int Sorting { get; set; }
    }
}