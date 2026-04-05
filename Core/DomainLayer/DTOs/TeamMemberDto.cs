using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs
{
    public class TeamMemberDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Role { get; set; }
        public string? Position { get; set; }
        public string? PhotoUrl { get; set; }
    }
    
}
