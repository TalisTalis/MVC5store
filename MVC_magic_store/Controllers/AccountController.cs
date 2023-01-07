using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_magic_store.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount ()
        {
            return View("CreateAccount");
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
    }
}