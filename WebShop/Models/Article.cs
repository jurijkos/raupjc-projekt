using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public double Price { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public List<Image> Images { get; set; }
        public List<Item> Items { get; set; }
       
    }
}
