using MVC_magic_store.Models.Data;
using MVC_magic_store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace MVC_magic_store.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // план
            // получить с БД сколько страниц имеется. Объявляем список для представления. Список сущностей PageVM
            // инициализация списка (DB)
            // возвращаем список в представление
            
            // Объявление списка представлений
            List<PageVM> pageList;

            // Определение подключения к БД. При выходе из этого метода подключение закрывается
            using (DB db = new DB())
            {
                // Наполнение Листа. Инициализация. Подключаемся к БД. Добавляем в массив. Отсортируем лямбда выражением
                // по значению по Sorting. Выбираем из БД лямбда выражением новым объектом.
                // Приводим к типу Лист. выбор из базы данных. 
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            };

            // Возвращаем список в представление
                return View(pageList);
        }

        // метод создания страниц
        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            // проверить заполнено ли всё и заполнено ли правильно
            // создать переменную для краткого описания Slug
            // нужно как-то подступиться к файлу PageDTO чтобы работать с БД
            // можно подступиться к полям и присваивать значения
            // возможно что заголовок или краткое описание не уникальны нужно убедится что они уникальны
            // сохранение модели в базу данных и сообщение пользоваателю об этом
            // переадресация на метод Index

            // Проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (DB db = new DB())
            {

                // Объявить переменную для краткого описания Slug
                string slug;

                // Инициализация класса PageDTO
                PagesDTO dto = new PagesDTO();

                // Присваеваем заголовок модели
                dto.Title = model.Title.ToUpper(); // заголовок заглавными буквами

                // Проверка есть ли краткое описание, если нет то присваеваем его
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower(); // заменяет пробел на тире и присваивает крааткому описанию
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // Убеждаемся, что заголовок и краткое описание уникальны
                if (db.Pages.Any(x => x.Title == model.Title)) // из БД выборку лямбда выражением по Title
                {
                    ModelState.AddModelError("", "That title already exist");
                    return View(model); // возвращаем данны обратно
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug)) // из БД выборку лямбда выражением по Slug
                {
                    ModelState.AddModelError("", "That slug already exist");
                    return View(model); // возвращаем данны обратно
                }

                // Присваеваем оставшиеся значения модели
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;
                dto.Sorting = 100;

                // Сохранение модели в БД
                db.Pages.Add(dto); // подключение сущности добавляем к сущностям модель dto
                db.SaveChanges(); // сохраанение в БД
            }

            // Передаем сообщение о добавлении страницы в БД через TempData
            TempData["SM"] = "You have added a new page!";

            // Переадресация пользователя на метод Index
            return RedirectToAction("Index");
        }
    }
}