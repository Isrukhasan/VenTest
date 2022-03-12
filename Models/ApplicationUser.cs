using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venturus.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string EmployeeId { get; set; }
        public string Department { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime Birthday { get; set; }
        public string Remarks { get; set; }
        public string ImagePath { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
    }
}
