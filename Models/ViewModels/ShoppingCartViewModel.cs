using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<ShoppingCart> AllCarts { get; set; }

        public double Price { get; set; }
        public int Quantity { get; set; }

        public double TotalPrice { get; set; }

    }
}
