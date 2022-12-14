using MVC_magic_store.Models.Data;
using MVC_magic_store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_magic_store.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            // план:
            // объявляем модель типа List
            // инициализируем модель данными
            // возвращаем List в представление

            List<CategoryVM> categoryVMList;

            // подключение к БД
            using(DB db = new DB())
            {
                // Инициализируем модель данных
                categoryVMList = db.Categories // подключение к сущностям
                                    .ToArray() // в массив
                                    .OrderBy(x => x.Sorting) // отсортированный лямбда выражением по Sorting
                                    .Select(x => new CategoryVM(x)) // создать выборку лямбда выражением
                                    .ToList(); // приводит к типу List
            }
            // Возвращаем в представление
            return View(categoryVMList);
        }
    }
}