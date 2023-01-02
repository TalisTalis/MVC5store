using MVC_magic_store.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_magic_store.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            // план:
            // объявить список товаров
            // проверка не пуста ли корзина
            // если не пуста складываем сумму и передать через viewbag
            // вернуть список в представление

            // объявляем список типа cartVM
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>(); // есил ссесия будет пусти то по умолчанию создаст новый экземпляр листа

            // проверка пуста ли корзина
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty";
                return View();
            }

            // складываем сумму и записываем во viewbag
            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // возвращаем представление с листом
            return View(cart);
        }

        // частичное представление корзины
        public ActionResult CartPartial()
        {
            // план:
            // объявить модель CartVM
            // переменная для общего количества. объявляем ее
            // объявить переменную цены
            // проверить сессию корзины имеются ли данные
            // если в корзине чтото есть то получить количество и цену
            // если пустая корзинаа то инициализировать стандартными значениями по нулям
            // вернуть частичное представление с моделью

            // Объявление CartVM и создаем экземпляр этой модели
            CartVM model = new CartVM();

            // объявить переменную общего количества и инициализировать нулем
            int qty = 0;

            // объявить переменную цены и инициализировать нулем
            decimal price = 0m;

            // проверка сессии корзины
            if (Session["cart"] != null)
            {
                // Получение общего количества товара и цену
                var list = (List<CartVM>)Session["cart"]; // явно указать что сессия должна быть типом List с параметром CartVM

                // циклом пройти по списку и добавить значения
                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }
            }
            else
            {
                // устанавливаем значения в модели на 0
                model.Quantity = 0;
                model.Price = 0m;
            }

            // возвращаем частичное представление
            return PartialView("_CartPartial", model);
        }
    }
}