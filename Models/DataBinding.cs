using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Venturus.Models
{
    public class DataBinding: DbContext
    {
        [Key]
        public string Id { get; set; }
        public string  UserID  { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public String RoleId { get; set; }
    }
}
