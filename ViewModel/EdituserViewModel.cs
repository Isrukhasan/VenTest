using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Venturus.ViewModel
{
    public class EdituserViewModel
    {
        [Key]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Department { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime Birthday { get; set; }
        public string Remarks { get; set; }
        public string ImagePath { get; set; }

        public IFormFile ProfileImage { get; set; }
    }
}
