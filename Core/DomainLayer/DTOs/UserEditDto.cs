using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs
{
    public class UserEditDto
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? University { get; set; }
        public string? Bio { get; set; }
        public IFormFile? Picture { get; set; }
    }
}
