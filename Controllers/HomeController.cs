using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Venturus.Models;
using WebApplication.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using PasswordGenerator;
using Venturus.Data;
using Venturus.ViewModel;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Venturus.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly MailSenderRepository mailrepo;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<IdentityRole> roleManager;
        public INotyfService _notifyService { get; }
        public HomeController(ILogger<HomeController> logger,
                              IConfiguration configuration,
                              UserManager<ApplicationUser> userManager,
                              ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager, INotyfService notifyService)
        {
            _notifyService = notifyService;
            this.roleManager = roleManager;
            _context = applicationDbContext;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            mailrepo = new MailSenderRepository(_configuration);
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task <string> LoggedInUser()
        {
            var myuser = _userManager.GetUserId(User);
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(myuser);
            string LIU = applicationUser.Email;
            return LIU;

        }


        public async Task<IActionResult> test()
        {

            var myuser = _userManager.GetUserId(User);

            ApplicationUser applicationUser = await _userManager.FindByIdAsync("26a11ebb-5cbd-435d-9021-e0720d17ca58");

            applicationUser.Custom2 = "FTest";
            await _userManager.UpdateAsync(applicationUser);
            return View();
        }



        [Authorize(Roles = "System Administrator")]
        public IActionResult Invite()
        {
            var aspRoles = roleManager.Roles;
            ViewBag.aspRoles=aspRoles;
            if (aspRoles != null)
            {
                return View();

            }
            else
            {
                return View("Error");
            }
            
        }       

        public async Task<RedirectToActionResult> SentInvite(IFormCollection collection)
        {
            var aspRoles = roleManager.Roles;
            ViewBag.aspRoles = aspRoles;
            var dateAndTime = DateTime.Now;
            var today = dateAndTime.Date;
            
            var expiray = today.AddDays(2);
            string host = _httpContextAccessor.HttpContext.Request.Host.Value;
            
            string email = collection["Iemail"].ToString();
            string invitedRole = collection["IRole"].ToString();
            string username = email;
            string isPassconfirmed = "0";
            DateTime insertedTime = today;
            string emailBody = "Dear User, </br>Please use following credentials to login.</br>";
            //Get LoggedIn user Email
            var myuser = _userManager.GetUserId(User);
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(myuser);
            string LIU = applicationUser.Email;

            //Checking if the email already in DB
            var UserCheck = _context.Inviations.Where(w => w.Email == email).FirstOrDefault();           

            if (email != null && UserCheck==null )
            {

                var pwd = new Password(10);
                var password = pwd.Next();

                Guid G = Guid.NewGuid();
                
                emailBody = emailBody+"</br><b>User Name: </b>" + email +" </br>" + "<b>Pasword: </b>" + password + "</br>";
                emailBody = emailBody+" "  + host+ "/Home/GetUserReg?token=" + G;
                try
                {   
                    var Invitations = new Inviation();
                    Invitations.InviationId = G.ToString();
                    Invitations.UserName = username;
                    Invitations.Email = email;
                    Invitations.Password = password;

                    Invitations.IsPassReset = "0";
                    Invitations.RequestedDate = today;
                    Invitations.InsertedBy = LIU;
                    Invitations.ExpiryDate = expiray;
                    Invitations.custom1 = invitedRole;


                    Invitations.IsInvited = "1";
                    Invitations.IsPending = "1";
                    Invitations.IsAccepted = "0";
                    Invitations.Iscancelled = "0";
                    Invitations.custom2 = "Invitation Sent";

                    _context.Add(Invitations);

                    _context.SaveChanges();
                    await mailrepo.Send(email, "New User", emailBody);
                    //await mailrepo.Send(email, "New User", emailBody);
                    _notifyService.Information("Invitation email sent to "+ email);
                    
                    RedirectToAction("Invite","Home");

                }
                catch(Exception ex)
                {
                    ex.Message.ToString(); 
                }               

            }
            else 
            {
                _notifyService.Warning(email + "is already in database.User exists");
               return RedirectToAction("Invite", "Home");
            }

            return RedirectToAction( "InvitedUser", "Administration");

        }
        public IActionResult GetUserReg(string token)
        {
            
            ViewBag.token = token;
            return View("UserReg");
        }
        public async Task<IActionResult> UserRegistrations(IFormCollection collection)

        {
            string token = collection["token"].ToString();
            
            string oldpass = collection["oldPassword"].ToString();
            string newpass = collection["newPassword"].ToString();


           // string assignedRole = collection["roles"].ToString();

            

            if (!String.IsNullOrEmpty(token))
            {
                var checkStatus = _context.Inviations.Where(i => i.InviationId == token && i.ExpiryDate >= DateTime.Today).FirstOrDefault();

                string email = checkStatus.Email;
                string assignedRole = checkStatus.custom1;

                //Generate 3 Digit Random Employee ID

                Random rand = new Random();
                const int maxValue = 999;
                string number = rand.Next(maxValue + 1).ToString("D3");
                var dateAndTime = DateTime.Now;
                var date = dateAndTime.Date;

                var empId = "VE-"+number + date;
                
                if (checkStatus.IsPassReset=="0" && oldpass==checkStatus.Password && oldpass!=newpass)
                {
                    
                    var user = new ApplicationUser { UserName =email, Email = email,EmployeeId=empId };
                    var result = await _userManager.CreateAsync(user, newpass);

                    ApplicationUser applicationUser = await _userManager.FindByEmailAsync(email);
                   

                    if (result.Succeeded&& applicationUser != null)
                    { 
                        //Invitation Table 
                        var updatestatus = _context.Inviations.Where(i => i.InviationId == token).FirstOrDefault();

                        updatestatus.IsPassReset = "1"; updatestatus.custom3 = empId;

                        updatestatus.IsInvited = "1";
                        updatestatus.IsPending = "0";
                        updatestatus.IsAccepted = "1";
                        updatestatus.Iscancelled = "0";
                        updatestatus.custom2 = "Invitation Accepted";                        

                        _context.SaveChanges();


                        //Asp Net Users table

                        applicationUser.Custom1 = token;
                        applicationUser.EmailConfirmed = true;
                        applicationUser.EmployeeId = empId;

                        //invited user -> inserted to ASP.NET USER table and then add an specific role by the System Admin  

                        //For adding the user to role 

                        var r = roleManager.Roles.Where(w => w.Name == assignedRole).FirstOrDefault();
                        var getuserId = applicationUser.Id;
                        var getRoleId = r.Id;
                      
                         await _userManager.AddToRoleAsync(applicationUser, assignedRole);
                        
                        await _userManager.UpdateAsync(applicationUser);

                        //await _userManager.AddToRoleAsync(applicationUser,assignedRole);


                    }
                    else
                    {
                        return View("Error");
                    }
                }
                else
                {
                    return View("Error");
                }
                
            }
            return RedirectToAction("Index", "Home");
            //string host = _httpContextAccessor.HttpContext.Request.Host.Value;
            //string URL = host + "/Identity/Account/Login";
            //return Redirect(URL.ToString());
           
        }

        //[Authorize]
        public IActionResult Register()
        {
            var url = Request.Scheme + "://" + Request.Host.Value;
            string URL = url + "/Identity/Account/Register";
            return Redirect(URL.ToString());
        }


        public async Task <IActionResult> Index()
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync("9f44e59e-55c9-4529-b7ca-ce013d7a5c52");
            return View();
        }

      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
