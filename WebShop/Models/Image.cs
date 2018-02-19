using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public class Image
    {
        public int ImageId { get; set; }
        public string Description { get; set; }
        public byte[] Data { get; set; }
        public int PositionNumber { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
