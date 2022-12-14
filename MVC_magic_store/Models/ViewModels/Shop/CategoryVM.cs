using MVC_magic_store.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_magic_store.Models.ViewModels.Shop
{
    public class CategoryVM
    {
        public CategoryVM() { } // конструктор класса без параметров по умолчанию

        public CategoryVM(CategoryDTO row) // конструктор с параметром модели DTO
        { 
            Id = row.Id;
            Name = row.Name;
            Slug= row.Slug;
            Sorting = row.Sorting;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; } // краткое описание
        public int Sorting { get; set; }
    }
}