using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Venturus.Data;

//[assembly: HostingStartup(typeof(Venturus.Areas.Identity.IdentityHostingStartup))]
//namespace Venturus.Areas.Identity
//{
//    public class IdentityHostingStartup : IHostingStartup
//    {
//        public void Configure(IWebHostBuilder builder)
//        {
//            builder.ConfigureServices((context, services) => {
//                services.AddDbContext<VenturusContext>(options =>
//                    options.UseSqlServer(
//                        context.Configuration.GetConnectionString("VenturusContextConnection")));

//                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//                    .AddEntityFrameworkStores<VenturusContext>();
//            });
//        }
//    }
//}