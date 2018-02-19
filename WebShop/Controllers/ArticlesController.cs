using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.Models;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;

namespace WebShop.Controllers
{
    
    [Authorize(Roles = "Admin")]
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        

        public ArticlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Articles
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, string currentGroup,string currentFilter, string choseGroup, string searchString, int? page)
        {
            
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (choseGroup != null)
            {
                page = 1;
            }
            else
            {
                choseGroup = currentGroup;
            }
            ViewData["CurrentGroup"] = choseGroup;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var articles = from s in _context.Article
                         select s;
            //var articles = (IQueryable<Article>)_context.Article;

            if (!String.IsNullOrEmpty(searchString))
            {
                articles = articles.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()) 
                                    || s.Manufacturer.ToUpper().Contains(searchString.ToUpper()));
            }
            
            switch (sortOrder)
            {
                case "name_desc":
                    articles = articles.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    articles = articles.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    articles = articles.OrderByDescending(s => s.Price);
                    break;
                default:
                    articles = articles.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 3;

            var groups = from g in _context.Group
                         select g;

            var x = from g in groups
                    select new SelectListItem
                    {
                        Value = g.GroupId.ToString(),
                        Text =  g.Name
                    };
            ViewBag.GroupId = x.ToList();
            //var articleGroup
            articles = from article in articles
                       join g in groups on
                       article.GroupId equals g.GroupId
                       select new Article
                       {
                           ArticleId = article.ArticleId,
                           Manufacturer = article.Manufacturer,
                           Name = article.Name,
                           Code = article.Code,
                           Price = article.Price,
                           GroupId = article.GroupId,
                           Group = g
                       };

            var images = from image in _context.Image
                         select image;
            var articlesWithImages = from article in articles
                                     join image in images on article.ArticleId equals image.Article.ArticleId into gj
                                     select new Article
                                     {
                                         ArticleId = article.ArticleId,
                                         Manufacturer = article.Manufacturer,
                                         Name = article.Name,
                                         Code = article.Code,
                                         Price = article.Price,
                                         GroupId = article.GroupId,
                                         Group = article.Group,
                                         Images = gj.ToList()
                                     };


            

            if (ViewData["CurrentGroup"] != null)
            {
                articlesWithImages = articlesWithImages.Where(t => t.GroupId == Int32.Parse(ViewData["CurrentGroup"].ToString()));
            }
            
            return View(await PaginatedList<Article>.CreateAsync(articlesWithImages.AsNoTracking(), page ?? 1, pageSize));
            //return View(await articles.AsNoTracking().ToListAsync());
        }

        // GET: Articles/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .SingleOrDefaultAsync(m => m.ArticleId == id);
            var group = await _context.Group.SingleOrDefaultAsync(m => m.GroupId == article.GroupId);
            article.Group = group;
            var images =  _context.Image.Where(i => i.ArticleId == article.ArticleId).ToList();
            article.Images = images;
            

            if (article == null)
            {
                return NotFound();
            }
            foreach(Image img in article.Images)
            {
                ConsoleApplication.WriteDefaultValues("wwwroot/img-article/"+img.ImageId.ToString() + ".jpg", img.Data);    
            }

            var x = from g in _context.Group
                    select new SelectListItem
                    {
                        Value = g.GroupId.ToString(),
                        Text = g.Name
                    };
            ViewBag.GroupId = x.ToList();

            return View(article);
        }

        // GET: Articles/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var x = from g in _context.Group
                    select new SelectListItem
                    {
                        Value = g.GroupId.ToString(),
                        Text = g.Name
                    };
            ViewBag.GroupId = x.ToList();
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,Manufacturer,Name,Code,GroupId,Price")] Article article)
        {
            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(article);
        }

        // GET: Articles/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.SingleOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,Name,Code,GroupId,Price")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
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
            return View(article);
        }
        [Authorize(Roles = "Admin")]
        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .SingleOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.SingleOrDefaultAsync(m => m.ArticleId == id);
            _context.Article.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.ArticleId == id);
        }


    }
}
