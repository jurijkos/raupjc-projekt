using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public int ArticleId { get; set; }
        public string AspUserId { get; set; }
        public Article Article { get; set; }
        
    }
}
