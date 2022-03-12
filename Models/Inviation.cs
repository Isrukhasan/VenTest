using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Venturus.Models
{
   
    public class Inviation
    {
        [Key]
        public string InviationId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
        public string IsInvited { get; set; }
        public string IsAccepted { get; set; }
        public string IsPending { get; set; }
        public string Iscancelled { get; set; }



        public string IsPassReset { get; set; }
        public string InsertedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime ExpiryDate { get; set; }

         

        public bool CUSTOM { get; set; }
        public string custom1 { get; set; }
        public string custom2 { get; set; }
        public string custom3 { get; set; }
        public string custom4 { get; set; }
        
    }
}
