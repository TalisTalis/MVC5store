using MVC_magic_store.Models.ViewModels.Account;
using MVC_magic_store.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_magic_store.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // метод создания аккаунта
        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount ()
        {
            return View("CreateAccount");
        }

        // метод создания аккаунта
        // POST: account/create-account
        [ActionName("create-account")]
        public ActionResult CreateAccount (UserVM model)
        {
            // план:
            // проверить модель на валидность
            // проверить соответствие пароля
            // проверить логин на уникальность
            // создание экземпляра класса UserDTO
            // добавить все данные в экземпляр класса
            // сохранить данные
            // добавить роль пользователю
            // сообщить пользователю об успешном завершении через tempdata
            // переадресация пользователя

            // Проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            // Проверка соответствия пароля
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                // добавление ошибки
                ModelState.AddModelError("", "Password do not match!");

                // возвращение представления с моделью
                return View("CreateAccount", model);
            }

            // подключение к БД
            using (DB db = new DB())
            {
                // проверка имени пользователя на уникальность
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    // добавление ошибки
                    ModelState.AddModelError("", $"Username {model.Username} is taken!");

                    // станавливаем пустое полее Login
                    model.Username = "";

                    // возвращение представления с моделью
                    return View("CreateAccount", model);
                }

                // создание экземпляра класса UserDTO и инициализация
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    Username = model.Username,
                    Password = model.Password
                };

                // добавление данных в модель в БД
                db.Users.Add(userDTO);

                // Сохранение данных
                db.SaveChanges();

                // добавление роли пользователю
                // объявление и инициализация переменной для хранения id пользователя 
                int id = userDTO.Id;

                // создание экземпляра класса userroledto и инициализация данными
                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2 // хардкодно установка роли нового пользователя
                };

                // Добавление записи в таблицу ролей
                db.UserRoles.Add(userRoleDTO);

                // сохранение изменений в БД
                db.SaveChanges();
            }

            // оповестить пользователя через TempData
            TempData["SM"] = "You are now registered and can login.";

            // переадресация пользователя на Login
            return RedirectToAction("Login");
        }

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login ()
        {
            // план:
            // подтвердить что пользователь не авторизован
            // возвращение представления

            // подтверждение что не авторизован
            // объявление и инициализация переменной именем текущего пользователя
            string userName = User.Identity.Name;

            // проверка если не пустая
            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("user-profile");
            }

            // возвращаем представление без модели
            return View();
        }

        // метод Login
        // POST: Account/Login
        [HttpPost]
        public ActionResult Login (LoginUserVM model)
        {
            // план:
            // проверка модели на валидность
            // проверка пользователя на валидность

            // проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // проверка пользователя на валидность
            // объявление и инициализация флага валидности
            bool isValid = false;

            // подключение к БД
            using (DB db = new DB())
            {
                // проверка елси в БД есть такой пользователь с таким паролем
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }

                // если пользователя такого нет
                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }
                else
                {
                    // сохранение к куки модель
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);

                    // переадресация по адресу по умолчанию в webconfig
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }
        }

        // метод Logout
        // GET: account/logout
        [HttpGet]
        public ActionResult Logout ()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login");
        }

        // метод обработки частичного представления usernavpartialvm
        public ActionResult UserNavPartial()
        {
            // план:
            // получение еимени пользователя
            // объявление модели
            // получение пользователя
            // заполнение модели данными из контекста DTO
            // возвращение частичного представления с моделью

            // Получение имени пользователя
            // объявление локальной переменной и инициализация 
            string userName = User.Identity.Name;

            // объявление модели VM
            UserNavPartialVM model;

            // подключение к БД
            using (DB db = new DB())
            {
                // получение пользователя из БД
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                // заполнение модели полученными из БД данными
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }

            // возвращение частичного представления с моделью
            return PartialView(model);
        }
    }
}