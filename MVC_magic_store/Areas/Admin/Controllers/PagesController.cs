using MVC_magic_store.Models.Data;
using MVC_magic_store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Threading;
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
                    slug = model.Title.Replace(" ", "-").ToLower(); // заменяет пробел на тире и присваивает краткому описанию
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

        // GET: Admin/Pages/EditPage/Id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // план:
            // объявим модель PageVM
            // получаем страницу по id
            // проверить доступна ли страница
            // если страница доступна то инициализировать страницу данными из DTO
            // возвращаем представление с моделью

            // Объявляем модель типа PageVM
            PageVM model;

            // Открываем подключение к БД
            using(DB db = new DB())
            {
                // Получаем страницу по ID
                PagesDTO dto = db.Pages.Find(id); // модели страницы присваеваем полученные данные из БД найденные по id

                // Проверка на достуупность найденной страницы
                if (dto == null)
                {
                    return Content("The page does not exist");
                }

                // Ининциализация модели страницы
                model = new PageVM(dto);
            }

            // Возвращаем в предствление модель
            return View(model);
        }

        // POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            // план
            // проверить модель на валидность
            // получить id страницы
            // объявим локальную временную переменную для slug
            // получаем страницу из БД по id
            // присваеваем название из полученной модели в DTO
            // проверить slug существует ли. если нет то присвоить его
            // проверить краткий заголовок и название на уникальность
            // присвоить остальные значения в класс DTO
            // сохраненить изменения в БД
            // оповестить пользователя о том что получилось сообщить через TempData
            // переадресовать обратно на ту страницу, которую редактировал\

            // Проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model); // просто возвращеие модели
            }

            using(DB db = new DB()) // открытие коннекта с БД. Как только закрывается этот блок то соединение с БД закрывается
            {
                // получаем id страницы
                int id = model.Id;

                // объявляем краткий заголовок и инициализируем null
                string slug = null;

                // получаем все данные страницы
                PagesDTO dto = db.Pages.Find(id);

                // присваеваем название из полученной модели в DTO
                dto.Title = model.Title;

                // Проверяем краткий заголовок
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }
                else
                {
                    slug = "home";
                }

                // Проверка slug и title на уникальность
                if (db.Pages.Where(x => x.Id != model.Id).Any(x => x.Title == model.Title)) // просмотр всех записей кроме текущей на сравнение заголовков с текущим
                {
                    // совпадение заголовоков
                    ModelState.AddModelError("", "That title already exist");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != model.Id).Any(x => x.Slug == model.Slug))
                {
                    // совпадение короткого описания
                    ModelState.AddModelError("", "That slug already exist");
                    return View(model);
                }

                // Записываем остальные значения в класс DTO
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;

                // Сохранение в БД
                db.SaveChanges();
            }

            // Устанавливаем сообщение в TempData
            TempData["SM"] = "You have edited this page.";

            // Переадресация туда откуда пользователь редактировал страницу
            return RedirectToAction("EditPage"); // переадресация на GET метод EditPage
        }

        // GET: Admin/Pages/PageDetails/Id
        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            // план:
            // объявим модель данных PageVM
            // проверяем доступна ли страница
            // присвоить модели информацию из БД
            // вернуть модель в представление

            // Объявить модель
            PageVM model;

            // открытие подключения к БД
            using(DB db = new DB())
            {
                // получение страницы из БД по id
                PagesDTO dto = db.Pages.Find(id);

                if (dto == null)
                {
                    return Content("This page does not eexist.");
                }

                // Присваиваем модели информацию из БД
                model = new PageVM(dto);
            }

            // возвращение модели
            return View(model);
        }

        // метод удаления страницы
        // GET: Admin/Pages/DeletePage/Id
        public ActionResult DeletePage(int id)
        {
            // план:
            // получаем страницу
            // удаем страницу
            // сохранение изменений в БД
            // переадресация на страницу index

            // Поддключение к БД
            using(DB db = new DB())
            {
                // Получение страницы
                PagesDTO dto = db.Pages.Find(id);

                // Удаление страницы
                db.Pages.Remove(dto);

                // Сохранение изменений
                db.SaveChanges();
            }
            // Сообщение пользователю об успешном удалении
            TempData["SM"] = "You have deleted a page.";

            // Переадресация на главную страницу
            return RedirectToAction("Index");
        }

        // метод сортировки страниц
        // POST: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
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
                PagesDTO dto;

                // Сортировка страниц
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId); // ищем элемент по id
                    dto.Sorting = count; // присваиваем текущему элементу Sorting 1

                    db.SaveChanges(); // сохраняем изменения

                    count++;
                }
            }
        }

        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            // план:
            // объявляем модель инициализируем
            // получить данные из БД DTO
            // заполнить модель данными
            // вернуть представление с моделью

            // Объявление модели
            SidebarVM model;

            // Подключение к БД
            using(DB db = new DB())
            {
                // Получение данных из DTO
                SidebarDTO dto = db.Sidebars.Find(1); // нельзя задавать жёсткие значения. Это пока 1 для примера

                // Присвоить полученные данные из БД модели
                model = new SidebarVM(dto);
            }

            // Возвращение представления с моделью
            return View(model);
        }
        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            // план:
            // получить данные из БД DTO
            // присвоить данные в тело (в свойства Body)
            // Сохранить в БД
            // Оповестить пользователя через TempData
            // переадресация на Sidebar

            // Открываем подключение к БД
            using (DB db = new DB())
            {
                // Получение данных из БД
                SidebarDTO dto = db.Sidebars.Find(1); // нельзя жестко писать

                // Присваиваем данные в модель. Из представления в dto (изменения)
                dto.Body = model.Body;

                // Сохранение в БД
                db.SaveChanges();
            }
            // Сообщение пользователю о сохранении изменений через TempData
            TempData["SM"] = "You have edited sidebar.";

            // Переаадресация на GET метод EditSidebar
            return RedirectToAction("EditSidebar");
        }
    }
}