using MVC_magic_store.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace MVC_magic_store.Models.ViewModels.Pages
{
    /// <summary>
    /// Получает от DTO все данные и в конструукторе присваивает значения моделе которая пойдет на представление
    /// </summary>
    public class PageVM
    {
        public PageVM() { } // конструктор класса по умолчанию
        public PageVM(PagesDTO row) { // конструктор класса который будет присваивать моделе значения из pagesDTO 
            Id = row.Id;
            Title= row.Title;
            Slug= row.Slug;
            Body= row.Body;
            Sorting= row.Sorting;
            HasSideBar= row.HasSideBar;
        } 
        public int Id { get; set; }
        [Required] // обязательное для заполнения поле
        [StringLength(50,MinimumLength = 3)] // максимальное и минимальное количество символов
        public string Title { get; set; }
        public string Slug { get; set; }
        [Required]
        [StringLength(int.MaxValue,MinimumLength = 3)]
        public string Body { get; set; }
        public int Sorting { get; set; }
        public bool HasSideBar { get; set; }
    }
}