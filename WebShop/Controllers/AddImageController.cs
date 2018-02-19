using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebShop.Data;
using WebShop.Models;

namespace WebShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AddImageController : Controller
    {
        private ApplicationDbContext _context;
        public AddImageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var x = from article in _context.Article
                    select new SelectListItem
                    {
                        Value = article.ArticleId.ToString(),
                        Text = article.Manufacturer + " " + article.Name
                    };

            /*List<SelectListItem> variabla = new List<SelectListItem>
            {
                new SelectListItem{Value="1", Text="jedan"},
                new SelectListItem{Value="2", Text="dva" }
            };*/
            ViewBag.ArticleId = x.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Image image, List<IFormFile> Data)
        {
            foreach(var item in Data)
            {
                if (item.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await item.CopyToAsync(stream);
                        image.Data = stream.ToArray();
                    }
                }
            }
            _context.Image.Add(image);
            _context.SaveChanges();
            return View();
        }
    }
}