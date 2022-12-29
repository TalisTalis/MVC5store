using MVC_magic_store.Models.ViewModels.Shop;
using MVC_magic_store.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

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

        // метод вывода товаров по категориям
        // GET: Shop/Category/name
        public ActionResult Category(string name)
        {
            // план:
            // объявить список типа лист
            // получение id категории
            // инициализация списка данными
            // получение имени категории
            // проверка на null
            // возврещение представления с моделью

            // Объявление списка типа List
            List<ProductVM> productVMList;

            // Получение id категории
            // Подключение к БД
            using (DB db = new DB())
            {
                // объявление и инициализация найденой в БД по имени категории
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();

                // получение id
                int catId = categoryDTO.Id;

                // инициализация списка данными
                productVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();

                // получение имени категории
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();

                // проверка на null
                if (productCat == null)
                {
                    // если нет еще в категории ни одного товара
                    // поиск имени категории не в товарах, а в категориях
                    var catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name).FirstOrDefault();

                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }
            }

            // возвращение модели в представление
            return View(productVMList);
        }

        // метод вывода товаров по категориям
        // GET: Shop/product-details/name
        // ссылка на метод отличается от названия метода
        // поэтому указывается в аннотации по какому названию можно найти этот метод
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            // план:
            // объявление двух моделей DTO и VM
            // инициалиация id продуктом
            // проверяем доступен ли продукт
            // инициализация модели dto данными
            // получение id
            // инициализируем модель VM данными
            // получение изображений галлереи
            // возвращаем модель в представление

            // объявление модели DTO
            ProductDTO dto;

            // объявление модели VM
            ProductVM model;

            // объявляем и инициализируем id
            int id = 0;

            // подключение к БД
            using (DB db = new DB())
            {
                // проверка доступности товара
                // если ничего не найдено
                if (!db.Products.Any(x => x.Slug.Equals(name)))
                {
                    // переадресация на метод Index
                    return RedirectToAction("Index", "Shop");
                }

                // инициализация модели DTO данными
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                // получаение id
                id = dto.Id;

                // инициализация модели VM данными
                model = new ProductVM(dto);
            }

            // Получение всех изображений из галлереи
            model.GalleryImages = Directory
                                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products" + id + "/Gallery/Thumbs"))
                                    .Select(fn => Path.GetFileName(fn));

            // возврат представления и метода
            return View("ProductDetails", model);
        }
    }
}