using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Student : User
    {
        public string GraduationProjectDatails { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string Skills { get; set; } = null!;

    }
}
