using Microsoft.AspNetCore.Http;
using MVC_magic_store.Models.Data;
using MVC_magic_store.Models.ViewModels.Pages;
using MVC_magic_store.Models.ViewModels.Shop;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Helpers;
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
        [HttpGet]
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
                model.Categories = new System.Web.Mvc.SelectList(db.Categories.ToList(), "Id", "Name");
            }
            return View(model);
        }

        // Метод добавления товаров
        //POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            // план:
            // проверка модели на валидность
            // проверка имени продукта на уникальность
            // объявление переменной productId
            // инициализация модели и сохранение ее в БД на основе ProductDTO
            // оповещение пользователя об успешном сохранении через TempData
            // Работа с картинкой:
            // создать все необходимые директории по которым будут храниться картинки
            // проверка существования директорий (если нет то создаем их)
            // проверка был ли загружен файл
            // проверка расширений файла
            // объявляем переменную с именем файла
            // сохраанение имени файла в модель DTO
            // назначить пути для хранения оригинала и уменьшенной копии
            // сохранение оригинала
            // сохранение уменьшеной копии
            // переадресация на страницу добавления товаров AddProduct

            // Проверка модели на валидность
            if (!ModelState.IsValid)
            {
                // открываем подключение к БД
                using(DB db = new DB())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name"); // если не составим заново список то выпадающий список будет пуст
                    return View(model);
                }
            }

            // открываем подключение к БД
            using(DB db = new DB())
            {
                // проверка на уникальность имени продукта
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name"); // если не составим заново список то выпадающий список будет пуст
                    // добавляем ошибку
                    ModelState.AddModelError("", "That product name is taken.");
                    return View(model);
                }
            }

            // объявляем переменную ProductID
            int id;

            // подключение к БД
            using(DB db = new DB())
            {
                // инициализация модели DTO
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ","-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                
                // необходима модель категории CategoryDTO. объявление и нициализация модели
                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                // сохранение модели
                db.Products.Add(product);
                db.SaveChanges();

                // получение id только что добавленной записи
                id = product.Id;
            }

            // добавление сообщения в TempData
            TempData["SM"] = "You have added a product.";

            // Upload Image
            // Создание необходимых ссылок директорий
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads")); // в корне появится папка Images а в ней папка Uploads

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs"); // уменьшенная копия
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            // Проверка наличия директорий
            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            // Проверка был ли загружен файл
            if (file != null && file.ContentLength > 0)
            {
                // получение расширения файла
                string ext = file.ContentType.ToLower();

                // проверка расширения файла
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    // подключение к БД
                    using(DB db = new DB())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name"); // если не составим заново список то выпадающий список будет пуст
                                                                                                
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extention"); // добавляем ошибку
                        return View(model);
                    }
                }
                // Объявляем переменную с именем изображения
                string imageName = file.FileName;

                // открываем соединение с БД
                using (DB db = new DB())
                {
                    // инициализация модели
                    ProductDTO dto = db.Products.Find(id);

                    // присваивание модели имени полученого файла
                    dto.ImageName = imageName;

                    // сохранение в БД
                    db.SaveChanges();
                }

                // назначение пути для оригинала и для уменьшенной копии
                var path2 = string.Format($"{pathString2}\\{imageName}"); // оригинал
                var path3 = string.Format($"{pathString3}\\{imageName}"); // уменьшенная копия

                // сохранение оригинала
                file.SaveAs(path2);

                // создание и сохранение уменьшенной копии
                WebImage img = new WebImage(file.InputStream);
                img.Resize(60, 60).Crop(1,1);
                img.Save(path3);
            }

            // возвращаем пользователя
            return RedirectToAction("AddProduct");

        }

        // метод списка товаров
        // GET: Admin/Shop/Products
        [HttpGet]
        public ActionResult Products (int? page, int? catId)
        {
            // план: 
            // объявление списка ProductVM типа List
            // установить номер страницы
            // инициализировать список и БД
            // заполнить категории данными
            // установить выбранную категорию
            // установить постраничную навигацию
            // возвратить всё в представление

            // объявление списка
            List<ProductVM> listOfProductVM;

            // установить номер страницы
            var pageNumber = page ?? 1;

            // подключение к БД
            using(DB db = new DB())
            {
                // инициализация листа. заполнение модели данными
                listOfProductVM = db.Products.ToArray()
                                        .Where(x => catId == null || catId == 0 || x.CategoryId == catId) // в массив где категории нет или равна 0 или по id категории
                                        .Select(x => new ProductVM(x))
                                        .ToList();

                // заполняем список категорий
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // устанавливаем выбранную категорию
                ViewBag.SelectedCat = catId.ToString();
            }

            // устанавливаем постраничную навигацию
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 10);

            // возвращаем в представление через viewbag
            ViewBag.OnePageOfProducts = onePageOfProducts;

            // возвращаем представление с данными
            return View(listOfProductVM);
        }

        // метод редактирования товаров
        // GET: Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            // план:
            // объявить модель productVM
            // получить все поля товара
            // проверить доступность существование товара
            // инициализируем модель данными БД
            // создаем список категорий
            // получить все изображения из галлереи
            // вернуть модель в представление

            // Объявляем модель ProductVM
            ProductVM model;

            // подключение к БД
            using(DB db = new DB())
            {
                // получаем товар
                ProductDTO dto = db.Products.Find(id);

                // проверка доступности товара
                if (dto == null)
                {
                    return Content("That product does not exist.");
                }

                // инициализация модели данными
                model = new ProductVM(dto);

                // создаем список категорий
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // получаем все изображения из галлереи
                model.GalleryImages = Directory
                                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                .Select(fn => Path.GetFileName(fn));
            }

            // Возвращаем представление с моделью
            return View(model);
        }

        // метод редактирования товара
        // POST: Admin/Shop/EditProduct
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            // план:
            // получение id товара
            // заполнить список категориями и изображениями
            // проверка модели на валидность
            // проверка имени на уникальность
            // обновить данные в БД
            // устновить сообщение в tempdata
            // загрузка обработка изображений:
                //
            // переадресация пользователя

            // получение id
            int id = model.Id;

            //string nameImageDTO;

            // подключение к БД
            using(DB db = new DB())
            {
                // заполнение модели списком категориями
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            // получаем все изображения из галлереи
            model.GalleryImages = Directory
                            .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                            .Select(fn => Path.GetFileName(fn));

            // проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // подключение к БД
            using(DB db = new DB())
            {
                // проверка названия товара на уникальность
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name)) // в БД все товары кроме текущего и в это имя не должно быть таким же
                {
                    ModelState.AddModelError("", "That product name is taken.");
                    return View(model);
                }
            }

            // подключение к БД
            using(DB db = new DB())
            {
                // находим в БД текущий товар
                ProductDTO dto = db.Products.Find(id);

                // обновляем данные из БД данными из полученной модели
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                // обновление категории
                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                // сохранение в БД
                db.SaveChanges();
            }

            // оповещение пользователя
            TempData["SM"] = "You have edited the product.";

            // Обработка изображений:
            // план:
            // проверяем загрузку файла
            // получаем расширение файла
            // проверить расширение файла
            // установить пути для загрузки
            // удаляем существующие файлы в директориях и директории
            // сохраняем изображение
            // сохраняем оригинал и превью версии

            // Проверка на загрузку файла
            if (file != null && file.ContentLength > 0)
            {
                // Получение расширения файла
                string ext = file.ContentType.ToLower(); // получение расширения и перевод в нижний регистр

                // проверка расширения файла
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png" &&
                    ext != "image/PNG")
                {
                    // подключение к БД
                    using (DB db = new DB())
                    {
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extention"); // добавляем ошибку
                        return View(model);
                    }
                }

                // устанавливаем пути загрузки
                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads")); // в корне появится папка Images а в ней папка Uploads

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs"); // уменьшенная копия

                // удаление существующих файлов и папок
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                // два цикла обхода изображений и папок. один удаляет изображения, другой уменьшенные копии
                foreach (var origFile in di1.GetFiles())
                {
                    origFile.Delete();
                }
                foreach (var miniFile in di2.GetFiles())
                {
                    miniFile.Delete();
                }

                // Сохранить имя изображения
                string imageName = file.FileName;

                // открытие подключения к БД
                using (DB db = new DB())
                {
                    // нахождение текущего товара в БД
                    ProductDTO dto = db.Products.Find(id);

                    // присваеваение нового названия
                    dto.ImageName = imageName;

                    // сохранение в БД
                    db.SaveChanges();
                }

                // назначение пути для оригинала и для уменьшенной копии
                var path1 = string.Format($"{pathString1}\\{imageName}"); // оригинал
                var path2 = string.Format($"{pathString2}\\{imageName}"); // уменьшенная копия

                // сохранение оригинала
                file.SaveAs(path1);

                // создание и сохранение уменьшенной копии
                WebImage img = new WebImage(file.InputStream);
                img.Resize(60, 60).Crop(1, 1);
                img.Save(path2);
            }
            else if (file == null)
            {
                // устанавливаем пути загрузки
                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads")); // в корне появится папка Images а в ней папка Uploads

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs"); // уменьшенная копия

                string newFileName = model.ImageName;

                var oldOrigFileName = new DirectoryInfo(pathString1).GetFiles();
                var oldMiniFileName = new DirectoryInfo(pathString2).GetFiles();

                string oldOrigFileNameString = oldOrigFileName[0].Name;
                string oldMiniFileNameString = oldMiniFileName[0].Name;

                // назначение пути для оригинала и для уменьшенной копии
                var newpath1 = string.Format($"{pathString1}\\{newFileName}"); // оригинал
                var newpath2 = string.Format($"{pathString2}\\{newFileName}"); // уменьшенная копия

                // назначение пути для оригинала и для уменьшенной копии
                var oldpath1 = string.Format($"{pathString1}\\{oldOrigFileNameString}"); // оригинал
                var oldpath2 = string.Format($"{pathString2}\\{oldMiniFileNameString}"); // уменьшенная копия

                System.IO.File.Copy(oldpath1, newpath1, true);
                System.IO.File.Delete(oldpath1);

                System.IO.File.Copy(oldpath2, newpath2, true);
                System.IO.File.Delete(oldpath2);
            }

            // Переадресация пользователя
            return RedirectToAction("EditProduct");
        }

        // метод удаления товара
        // POST: Admin/Shop/DeleteProduct/id
        public ActionResult DeleteProduct(int id)
        {
            // план:
            // удаляем товар из БД
            // удалить директории товара
            // переадресация пользователя

            // подключение к БД
            using (DB db = new DB())
            {
                // получение данной модели
                ProductDTO dto = db.Products.Find(id);

                // удаление данной модели
                db.Products.Remove(dto);

                // сохранение изменений
                db.SaveChanges();
            }

            // удаление директорий
            // пути к изображениям
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            // проверка на доступность директорий
            if (Directory.Exists(pathString))
            {
                Directory.Delete(pathString, true);
            }

            // переадресация
            return RedirectToAction("Products");
        }

        // метод добавления изображений в галлерею
        // POST: Admin/Shop/SaveGalleryImages/id
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // план:
            // перебрать все полученные файлы
            // инициализировать файлы
            // проверка на null
            // назначаем пути к директориям
            // назначить пути самих изображений
            // сохранить оригинальные изображения и уменьшенные копии

            // Перебор полученных файлов (имен файлов)
            foreach (string fileName in Request.Files)
            {
                // инициализация файлов
                HttpPostedFileBase file = Request.Files[fileName];

                // проверка на null есть ли файлы
                if (file != null && file.ContentLength > 0)
                {
                    // назначить пути к директориям
                    var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                    // путь к Gallery оригиналам
                    string pathStringOrigGallery = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");

                    // путь к Gallery уменьшенным копиям
                    string pathStringMiniGallery = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    // пути к изображениям
                    // к оригиналам
                    var pathOrigGallery = string.Format($"{pathStringOrigGallery}\\{file.FileName}");

                    // к копиям
                    var pathMiniGallery = string.Format($"{pathStringMiniGallery}\\{file.FileName}");

                    // сохранения оригиналов и копий
                    // оригиналы
                    file.SaveAs(pathOrigGallery);

                    // копии
                    // взять из стрима файл
                    WebImage img = new WebImage(file.InputStream);
                    // уменьшение размера
                    img.Resize(60, 60).Crop(1, 1);
                    img.Save(pathMiniGallery);
                }
            }
        }

        // метод удаления изображений галлереи
        // POST: Admin/Shop/DeleteImage/id/imageName
        public void DeleteImage(int id, string imageName)
        {
            // пути 
            // оригинал
            string fullPathOrig = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);

            // копии
            string fullPathMini = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            // существуют ли там картинки
            if (System.IO.File.Exists(fullPathOrig)) // существуют ли файлы по данному пути
            {
                System.IO.File.Delete(fullPathOrig);
            }
            if (System.IO.File.Exists(fullPathMini)) // существуют ли файлы по данному пути
            {
                System.IO.File.Delete(fullPathMini);
            }

        }
    }
}