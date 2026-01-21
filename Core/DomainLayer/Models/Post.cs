using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
