using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Venturus.Models;
using Venturus.ViewModel;

namespace Venturus.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Inviation> Inviations { get; set; }
        //public virtual DbSet<StoredProcedure>? RetriveData { get; }

    //public DbSet<Common> Common { get; set; }
    // public DbSet<ApplicationUser> ApplicationUser { get; set; }
}
}
//Add-Migration intial06032022 -context ApplicationDbContext
//update-database -context ApplicationDbContext