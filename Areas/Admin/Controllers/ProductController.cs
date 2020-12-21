using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Models.ViewModels;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db, 
                                 IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _db.Product.Include(x => x.Category).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ProductAndCategoryViewModel model = new ProductAndCategoryViewModel()
            {
                Product = new Models.Product(),
                CategoryList = await _db.Category.ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductAndCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string imageName = "noimage.png";

            if (model.Product.ImageUpload != null)
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                imageName = Guid.NewGuid().ToString() + "_" + model.Product.ImageUpload.FileName;
                string filePath = Path.Combine(uploadsDir, imageName);
                FileStream fs = new FileStream(filePath, FileMode.Create);
                await model.Product.ImageUpload.CopyToAsync(fs);
                fs.Close();
            }

            model.Product.Image = imageName;

            _db.Add(model.Product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int id)
        {
            Product product = await _db.Product.Include(c => c.Category)
                                               .FirstOrDefaultAsync(c => c.Id == id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _db.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ProductAndCategoryViewModel model = new ProductAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                Product = product
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.Product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");

                    if (!string.Equals(model.Product.Image, "noimage.png"))
                    {
                        // delete old image
                        string oldImagePath = Path.Combine(uploadsDir, model.Product.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string imageName = Guid.NewGuid().ToString() + "_" + model.Product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await model.Product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    model.Product.Image = imageName;
                }


                _db.Product.Update(model.Product);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                // check if the product image has not 'noimage.jpg'
                if (!string.Equals(product.Image, "noimage.png"))
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");

                    string oldImagePath = Path.Combine(uploadsDir, product.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        // removing old image
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _db.Product.Remove(product);
                await _db.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Index));
        }
    }
}