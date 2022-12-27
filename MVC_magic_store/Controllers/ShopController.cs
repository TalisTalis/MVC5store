using MVC_magic_store.Models.ViewModels.Shop;
using MVC_magic_store.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_magic_store.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages"); // переадресация на уже существующий метод индекс в контроллере pages
        }

        // метод для частичного представления списка категорий
        public ActionResult CategoryMenuPartial()
        {
            // план:
            // объявить модель типа лист CateegoryVM
            // инициализация модели данными из бд
            // возвращение частичного представления с моделью

            // объявление модели
            List<CategoryVM> categoryVMList;

            // инициализация модеи данными
            // подключение к БД
            using (DB db = new DB())
            {
                categoryVMList = db.Categories.ToArray()
                                            .OrderBy(x => x.Sorting)
                                            .Select(x => new CategoryVM(x))
                                            .ToList();
            }

            return PartialView("_CategoryMenuPartial", categoryVMList);
        }
    }
}