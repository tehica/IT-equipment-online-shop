using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models.ViewModels
{
    public class ProductAndCategoryViewModel
    {
        public Product Product { get; set; }

        public IEnumerable<Category> CategoryList { get; set; }

    }
}
