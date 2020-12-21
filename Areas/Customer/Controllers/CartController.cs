using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Routing;
//using Stripe;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;        
        }

        public async Task<IActionResult> Index(ShoppingCartViewModel model)
        {

            // get user id of the user who is logged in to retrieve all the shopping card that user has
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(x => x.ApplicationUserId == claim.Value);
            
            if(cart != null)
            {
                model.AllCarts = cart.ToList();
            }
            foreach(var list in model.AllCarts)
            {
                list.Product = await _db.Product.Where(p => p.Id == list.ProductId)
                                                .FirstOrDefaultAsync();
                model.Price = list.Product.Price;
                model.Quantity = list.Quantity;
                model.TotalPrice = model.TotalPrice + (model.Price * model.Quantity);
            }

            return View(model);
        }

        public async Task<IActionResult> Plus(int id)
        {
            var cart = await _db.ShoppingCart.Where(c => c.Id == id).FirstOrDefaultAsync();
            cart.Quantity += 1;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int id)
        {
            var cart = await _db.ShoppingCart.Where(c => c.Id == id).FirstOrDefaultAsync();
            if(cart.Quantity == 1)
            {
                _db.ShoppingCart.Remove(cart);
                await _db.SaveChangesAsync();
            }
            else
            {
                cart.Quantity -= 1;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Remove(int id)
        {
            var cart = await _db.ShoppingCart.Where(c => c.Id == id).FirstOrDefaultAsync();
            _db.ShoppingCart.Remove(cart);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPOST()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
