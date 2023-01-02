using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_magic_store.Models.ViewModels.Cart
{
    public class CartVM
    {
        // необходимая информация для карзины
        // id продукта
        public int ProductId { get; set; }
        // имя продукта
        public string ProductName { get; set; }
        // количество продукта
        public int Quantity { get; set; }
        // цена продукта
        public decimal Price { get; set; }
        // общая стоимость покупки где set убран а get возвращает результат умножения количества на цену
        public decimal Total { get { return Quantity * Price; } }
        public string Image { get; set; }
    }
}