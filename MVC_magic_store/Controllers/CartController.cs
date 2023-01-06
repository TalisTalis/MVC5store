using MVC_magic_store.Models.ViewModels.Cart;
using MVC_magic_store.Models.Data;
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

                // сохранение в модель
                model.Quantity = qty;
                model.Price = price;
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

        public ActionResult AddToCartPartial(int id)
        {
            // план:
            // объявить список типа cartvm
            // объявить модель cartvm
            // получить товар по id
            // проверка находится ли такой товар в корзине или нет
            // если нет в корзине то добавить в корзину
            // если есть то добавить еще
            // поличить общее количество товаров
            // получить общее цен
            // добавить данные в модель
            // сохраняем состояние корзины в сессию
            // возвращаем частичное представление с моделью

            // объявение лист параметризированный типом CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // обявление модели CartVM
            CartVM model = new CartVM();

            // открываем подключение к БД
            using (DB db = new DB())
            {
                // получение товара
                ProductDTO product = db.Products.Find(id); // поиск БД по id

                // проверка находится ли товар уже в корзине
                // объявление и инициализация переменной данными из БД записью товара с переданным id
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // если товара нет то добавляем новый товар в корзину
                if (productInCart == null)
                {
                    // добавляем товар и присваиваем значения
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    // если товар уже есть в корзине
                    productInCart.Quantity++;
                }

                // получение общее количество товара, итоговую цену
                // переменная общего количества товара
                int qty = 0;
                // переменная общей цены
                decimal price = 0m;

                // проводим по карзине и прибавляем в общее количество и в общую цену цены товаров в корзине
                foreach (var item in cart)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                // добавляем полученные данные в модель
                model.Quantity = qty;

                model.Price = price;

                // сохранение состояния корзины в сессию корзины
                Session["cart"] = cart;

                // Возвращение частичного представления с моделью
                return PartialView("_AddToCartPartial", model);
            }
        }
    }
}