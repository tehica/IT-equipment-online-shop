using Microsoft.AspNetCore.Http;
using OnlineShop.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Price must be above ${1}")]
        public double Price { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        // ForeignKey
        [Display(Name = "Category")]
        [Range(1, int.MaxValue, ErrorMessage = "Choose product category.")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [NotMapped]
        [FileImgExtension]
        public IFormFile ImageUpload { get; set; }
    }
}
