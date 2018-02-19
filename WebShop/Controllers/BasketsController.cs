using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.Models;

namespace WebShop.Controllers
{
    [Authorize]
    public class BasketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public BasketsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            var items = from item in _context.Item
                        where item.AspUserId == _userManager.GetUserId(HttpContext.User)
                        select item;
            items = from item in items
                    join article in _context.Article on
                    item.ArticleId equals article.ArticleId
                    select new Item
                    {
                        ArticleId = item.ArticleId,
                        AspUserId = item.AspUserId,
                        ItemId = item.ItemId,
                        Quantity = item.Quantity,
                        Article = article
                    };
            double totalPrice = 0.0;
            foreach (var item in items)
            {
                totalPrice = totalPrice + item.Article.Price * item.Quantity;
            }
            ViewBag.totalPrice = totalPrice;
            return View(items);
        }

        public IActionResult Payment()
        {
            return View();
        }
        
        [HttpPost,ActionName("Pay")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentConfirmed()
        {
            var items = from item in _context.Item
                        where item.AspUserId == _userManager.GetUserId(HttpContext.User)
                        select item;
            foreach (var item in items)
            {
                _context.Item.Remove(item);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }


        // GET: Baskets/AddArticle/5
        public async Task<IActionResult> AddArticle(int? articleId)
        {
            if (articleId == null)
            {
                return NotFound();
            }

            var articleInOrder = await _context.Item.SingleOrDefaultAsync(m => m.ArticleId == articleId && m.AspUserId == _userManager.GetUserId(HttpContext.User));
            if (articleInOrder == null)
            {
                articleInOrder = new Item();
                articleInOrder.ArticleId = (int)articleId;
                articleInOrder.AspUserId = _userManager.GetUserId(HttpContext.User);
                articleInOrder.Quantity = 1;

            }
            return View(articleInOrder);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddArticle([Bind("ItemId","ArticleId","AspUserId","Quantity")] Item item)
        {
            

            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ArticleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        private bool ItemExists(int id)
        {
            return _context.Article.Any(e => e.ArticleId == id);
        }

    }
}