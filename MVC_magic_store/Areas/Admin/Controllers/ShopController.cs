using MVC_magic_store.Models.Data;
using MVC_magic_store.Models.ViewModels.Pages;
using MVC_magic_store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;

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

        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            // план:
            // объявить строковую переменную id
            // проверить имя категории на уникальность
            // инициализировать модель dto
            // добавить данные в модель
            // сохранить в БД
            // получить id чтобы вернуть в представление
            // возвращение id в представление

            string id;

            // подключение к БД
            using(DB db = new DB())
            {
                // проверка на уникаальность названия категории
                if (db.Categories.Any(x => x.Name == catName))
                {
                    return "titletaken"; // если название такое уже есть то возвращает кодовое слово
                }

                // инициализация модели DTO
                CategoryDTO dto = new CategoryDTO();

                // Добавление данные в модель
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                // Сохранение
                db.Categories.Add(dto);
                db.SaveChanges();

                // Получение id того что сохрании
                id = dto.Id.ToString();
            }

            // возвращение в представление
            return id;
        }
        
        // метод сортировки страниц
        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            // план:
            // создаем счетчик count 
            // инициализируем модель данных
            // устанавливаем сортировку ддля каждой страницы (в БД есть метод Sorting)

            // Открываем подключение к БД
            using (DB db = new DB())
            {
                int count = 1; // home по умолчанию равен 0, поэтому отсчёт начинается с 1

                // инициализируем модель данных
                CategoryDTO dto;

                // Сортировка страниц
                foreach (var categoryId in id)
                {
                    dto = db.Categories.Find(categoryId); // ищем элемент по id
                    dto.Sorting = count; // присваиваем текущему элементу Sorting 1

                    db.SaveChanges(); // сохраняем изменения

                    count++;
                }
            }
        }

        // метод удаления категории
        // POST: Admin/Shop/DeleteCategory
        public string DeleteCategory(int id)
        {
            // план:
            // получаем страницу
            // удаем страницу
            // сохранение изменений в БД
            // переадресация на страницу index

            // Поддключение к БД
            using (DB db = new DB())
            {
                // Получение страницы
                CategoryDTO dto = db.Categories.Find(id);

                // Удаление страницы
                db.Categories.Remove(dto);

                // Сохранение изменений
                db.SaveChanges();
            }

            // Переадресация на страницу Categories
            return id.ToString();
        }

        // метод переименовывания категории
        // POST: Admin/Categories/RenameCategory/Id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            // план:
            // проверить имя категории на уникальность
            // получить все данные из БД модель DTO
            // редактирование модели DTO
            // сохранение изменений
            // возвращение результата string

            using (DB db = new DB())
            {
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                CategoryDTO dto = db.Categories.Find(id);
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();
                db.SaveChanges();
            }
            return "succes";
        }

        // Метод добавления товаров
        // GET: Admin/Shop/AddProduct
        public ActionResult AddProduct()
        {
            // план:
            // объявить модель данных
            // добавить в модель список категорий из БД
            // возвращаем модель в представление

            // Обявление модели
            ProductVM model = new ProductVM();

            // Открываем соединение с БД
            using (DB db = new DB())
            {
                // добавление  модель список категорий
                model.Categories = new System.Web.Mvc.SelectList(db.Categories.ToList(), "id", "Name");
            }
            return View(model);
        }
    }
}