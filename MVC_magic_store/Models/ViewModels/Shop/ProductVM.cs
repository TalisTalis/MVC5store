using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;
using MVC_magic_store.Models.Data;

namespace MVC_magic_store.Models.ViewModels.Shop
{
    public class ProductVM
    {
        public ProductVM(){} // конструктор по умолчанию без параметров

        public ProductVM(ProductDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Description = row.Description;
            Price = row.Price;
            CategoryName = row.CategoryName;
            CategoryId = row.CategoryId;
            ImageName = row.ImageName;
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } // обязательное поле
        public string Slug { get; set; }
        [Required]
        public string Description { get; set; } // обязательное поле
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        [Required]
        [DisplayName("Category")]  // как будет отображаться. Не CategoryId, а Category
        public int CategoryId { get; set; }
        public string ImageName { get; set; }

        // получение данных по внешнему ключу
        public IEnumerable<System.Web.Mvc.SelectListItem> Categories { get; set; } // перебор полученных данных
        public IEnumerable<String> GalleryImages { get; set; }
    }
}