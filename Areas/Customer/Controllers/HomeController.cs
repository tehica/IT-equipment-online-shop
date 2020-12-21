using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Models.ViewModels;

namespace OnlineShop.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ApplicationDbContext db,
                              ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchProduct { get; set; }


        public async Task<IActionResult> Index()
        {
            if(SearchProduct == null)
            {
                ProductIndexViewModel vm = new ProductIndexViewModel()
                {
                    // get aLL products from db
                    Product = await _db.Product
                                   .Include(c => c.Category)
                                   .ToListAsync(),
                    // get all categories from db
                    Category = await _db.Category.ToListAsync()
                };
                return View(vm);
            }

            ProductIndexViewModel SearchVM = new ProductIndexViewModel()
            {
                // get aLL products from db
                Product = await _db.Product
                                   .Include(c => c.Category)
                                   .Where(p => p.Name.Contains(SearchProduct))
                                   .ToListAsync(),
                // get all categories from db
                Category = await _db.Category.ToListAsync()
            };
            return View(SearchVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            Product productFromDb = await _db.Product.Where(p => p.Id == id)
                                                     .FirstOrDefaultAsync();
            ShoppingCart cartObj = new ShoppingCart
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            return View(cartObj);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if (ModelState.IsValid)
            {
                // get loggedin user's id
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                CartObject.ApplicationUserId = userId;

                ShoppingCart cartFromDb = await _db.ShoppingCart
                                                   .Where(x => x.ApplicationUserId == CartObject.ApplicationUserId
                                                   && x.ProductId == CartObject.ProductId)
                                                   .FirstOrDefaultAsync();
                if(cartFromDb == null)
                {
                    await _db.ShoppingCart.AddAsync(CartObject);
                }
                else
                {
                    cartFromDb.Quantity = cartFromDb.Quantity + 1;
                }
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(CartObject);
        }

    }
}
