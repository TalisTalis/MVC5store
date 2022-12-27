using MVC_magic_store.Models.Data;
using MVC_magic_store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_magic_store.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "") // по умолчанию page равна пусто строке
        {
            // план:
            // получить(установить) краткий заголовок то есть slug
            // объявляем модель и данные класс DTO
            // проверка доступна ли текущая страница
            // получить контекст данных DTO страницы
            // установка заголовка страницы (title)
            // проверка есть ли для этой страницы боковая панель
            // заполнение можеи данными
            // возвращаем представление с моделью

            // получаем slug
            if (page == "")
            {
                page = "home";
            }

            // обявление модели
            PageVM model;

            // объявление класса DTO
            PagesDTO dto;

            // подключение к БД
            using (DB db = new DB())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page))) // ищем в БД всё и если не найдено ни одного slug переадресация на главную страницу
                {
                    return RedirectToAction("Index", new {page = ""});
                }
            }

            // получение DTO страницы
            using (DB db = new DB())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault(); // первое совпадение slug и page
            }

            // устанавливаем заголовок страницы
            ViewBag.PageTitle = dto.Title;

            // проверка есть ли боковая панель
            if (dto.HasSideBar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            // заполнить модель данными
            model = new PageVM(dto);

            // вернуть в представление
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            // план:
            // инициализация листа VM
            // получение всех страниц кроме home
            // возвращение частичного представления с листом данных

            // объявление листа
            List<PageVM> pageVMList;

            // подключение к БД
            using (DB db = new DB())
            {
                // получение всех страниц кроме home
                // инициализация листа
                pageVMList = db.Pages
                                .ToArray() // в массив
                                .OrderBy(x => x.Sorting) // отсортированный по полю sorting
                                .Where(x => x.Slug != "home") // все кроме записи в поле slug home
                                .Select(x => new PageVM(x)) // в модель
                                .ToList(); // преобразовываем модель к типу List
            }

            // возвращение частичного представления
            return PartialView("_PagesMenuPartial", pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            // план:
            // объявить модель
            // инициализация модели данными
            // возвращение модели в частичное представление

            // объявление модели sidebar
            SidebarVM model;

            // инициализация модели данными
            // подключение к БД
            using (DB db = new DB())
            {
                // загрузка в dto данные из БД
                SidebarDTO dto = db.Sidebars.Find(1);

                // сохранение данных в модель
                model = new SidebarVM(dto);
            }

            // возвращение модели в частичное представление
            return PartialView("_SidebarPartial", model);
        }
    }
}