using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Venturus.Data;
using Venturus.Models;
using Venturus.ViewModel;
using WebApplication.Repo;

namespace Venturus.Controllers
{
    public class AdministrationController : Controller
    {
       
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public INotyfService _notifyService { get; }        

        public AdministrationController (RoleManager<IdentityRole> roleManager ,UserManager<ApplicationUser> userManager, INotyfService notifyService, ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.roleManager = roleManager;
            _userManager = userManager;
            _notifyService = notifyService;
            _httpContextAccessor = httpContextAccessor;
            _context = applicationDbContext;
        }
        //Change the role of a general user

        [AllowAnonymous]
        //Update Role according to user
      
        public async Task<IActionResult> ChangeRolesForEveryUser(IFormCollection collection)
        {
            string NewRole = collection["NewRole"];
            try
            {                
                if (!String.IsNullOrEmpty(NewRole))
                {
                    
                    string PreviousRole = collection["previousRole"];
                    var UID = collection["aspUserId"].ToString();
                    string n = UID.ToString();

                    //We are getting the data from table but this was coming with white space
                    //So we had to use trim method

                    ApplicationUser applicationUser = await _userManager.FindByIdAsync(UID.Trim());


                    if (!String.IsNullOrEmpty(UID) && !String.IsNullOrEmpty(NewRole) &&
                    !String.IsNullOrEmpty(PreviousRole) && PreviousRole != NewRole)
                    {
                        await _userManager.RemoveFromRoleAsync(applicationUser, PreviousRole);

                        await _userManager.AddToRoleAsync(applicationUser, NewRole);

                        await _userManager.UpdateAsync(applicationUser);


                    }
                    else
                    {
                        _notifyService.Warning("Previous Role & New Role can't be same ");
                    }
                    
                }
                else
                {
                    //get the All User data from Db by SP and
                    var empRecords = _context.DataBindings.FromSqlRaw("EXECUTE dbo.GetAllUserDetailsWithRoleDetails ").ToList();
                    //Get roles
                    var aspRoless = roleManager.Roles;
                    ViewBag.aspRoles = aspRoless;
                    ViewBag.empRecord = empRecords;
                    return View();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  

            }
            var aspRoles = roleManager.Roles;
            ViewBag.aspRoles = aspRoles;
            var empRecord = _context.DataBindings.FromSqlRaw("EXECUTE dbo.GetAllUserDetailsWithRoleDetails ").ToList();
            ViewBag.empRecord = empRecord;
            return View();
            //return RedirectToAction("ChangeRolesForEveryUser", "Administration");
        }

         
        public async Task<IActionResult> RoleSeriesWithIdForUpdate()
        {
            return View("RoleSeriesForUpdate");
        }

        
        public IActionResult GetUsersWithRole()
        {
            return View();
        }



        //Update user view
        public async Task<IActionResult> SingleUserView()
        {
            var myuser = _userManager.GetUserId(User);

            ApplicationUser applicationUser = await _userManager.FindByIdAsync(myuser);
            string Phone = applicationUser.Phone;
            string Address=applicationUser.Address;
            DateTime Birthday = applicationUser.Birthday;
            ViewBag.dept = applicationUser.Department;
            ViewBag.phone = Phone;
            ViewBag.address = Address;
            ViewBag.birthday=Birthday;
            ViewBag.email = applicationUser.NormalizedEmail;
            ViewBag.employeeId=applicationUser.EmployeeId;

            return View();
        }
        public async Task<IActionResult> updateUserView(IFormCollection collection)
        {
            var myuser = _userManager.GetUserId(User);

            ApplicationUser applicationUser = await _userManager.FindByIdAsync(myuser);

            string upDept = collection["dept"].ToString();
            applicationUser.Department = upDept;
            await _userManager.UpdateAsync(applicationUser);
            _notifyService.Success($"updated user{@upDept}", 5);
            return View("SingleUserView");
        }


        // var aspRoles = roleManager.Roles;
        //Asp User lists
        public IActionResult GetAspUser()
        {
            var aspUsers = _userManager.Users;

            return View(aspUsers);
        }
        
        //Invited User List
       /* [Authorize(Roles = "SYSTEM ADMINISTRATOR")]*/
        public IActionResult InvitedUser()
        {
            var invitedPeople = _context.Inviations.Where(i => i.InviationId != null).ToList().OrderBy(i=>i.RequestedDate);

            var accpetedInvite = invitedPeople.Where(i => i.IsPassReset == "1").OrderByDescending(i => i.InsertedDate).ToList();

            var pendingInvite = invitedPeople.Where(i => i.IsPassReset == "0").OrderByDescending(i => i.InsertedDate).ToList();
            var cancelledInvite = invitedPeople.Where(i => i.custom4 == "4" && i.IsPassReset == "0").OrderByDescending(i => i.InsertedDate).ToList();

            decimal totallCancelled = cancelledInvite.Count();
            decimal totalInvited = invitedPeople.Count();
            decimal totalAccepted = accpetedInvite.Count();
            decimal totallpendingInvitation = totalInvited - (totalAccepted+totallCancelled);

            ViewBag.cancelledPeoples = cancelledInvite;
            ViewBag.totallCancelled = totallCancelled;
            ViewBag.invitedPeoples = invitedPeople;
            ViewBag.accpetedInvite = accpetedInvite;
            ViewBag.pendingInvite = pendingInvite;
            ViewBag.totalInvited = totalInvited;
            ViewBag.totalAccepted = totalAccepted;
            ViewBag.totallpendingInvitation = totallpendingInvitation;
            return View();
        }

        [HttpGet]
        public IActionResult CreateRole()
        {            
            return View("CreateRoleView");
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if(ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                //if the role already exist
                var existingRoles = roleManager.Roles; 
                foreach (var item in existingRoles)
                {
                    if(identityRole.Name.ToString()==item.Name.ToString() )
                    {
                         _notifyService.Error("Error in making role");
                        return View("CreateRoleView");
                    }
                    
                }
                
                    IdentityResult result = await roleManager.CreateAsync(identityRole);
                    if (result.Succeeded)
                    {
                        _notifyService.Success("Role has been created");

                    }
                    foreach (IdentityError errors in result.Errors)
                    {
                        ModelState.AddModelError("", errors.Description);
                    }               

            }

            return RedirectToAction ("ListRoles", model);
        }

        //Get all the roles

        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(EdituserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"user with the id ={model.Id} does not exist";
                return View("Error");
            }
            else
            {
                user.Department=model.Department;
                var result = await _userManager.UpdateAsync(user);

                foreach (var errors in result.Errors)
                {
                    ModelState.AddModelError("", errors.Description);
                }
            }
            return View(model);
        }

        //Cancel Pending Invitaion
        public async Task <IActionResult> CancelInvitaion(string id)
        {
            var cancel = _context.Inviations.Where(w => w.InviationId == id).FirstOrDefault();
            cancel.Iscancelled = "1";
            cancel.IsPassReset = "0";
            cancel.IsAccepted = "0";
            cancel.IsPending = "0";
            cancel.custom2 = "Invitation cancelled manually";

            cancel.custom4 = "4";
            _context.Update(cancel);
            _context.SaveChanges();
            return RedirectToAction("InvitedUser");
        }

        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.Error = "Role id not found";
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id, RoleName = role.Name

            };
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.UsersList.Add(user.UserName);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag("Role not found found"  );
                return View("ListRoles");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                     return RedirectToAction("ListRoles");
                }
                foreach (var errors in result.Errors)
                {
                    ModelState.AddModelError("", errors.Description);
                }
            }

            return View("ListRoles");
        }


       //Delete a role from asp .net roles
     
       public async Task<IActionResult> DeleteRole(string roleId)
        {
            var roleName = await roleManager.FindByIdAsync(roleId);
            if (!String.IsNullOrEmpty(roleName.ToString())){
                
                var result = await roleManager.DeleteAsync(roleName);
                _notifyService.Success(roleName+" Role removed from system");
            }
            else
            {
                _notifyService.Warning("Role not found on database");
               

            }
            return RedirectToAction("ListRoles");
        }

        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            {
                ViewBag.roleId = roleId;
            }
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag("Role not found found");
                return View("");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)

            {
                var userRoleInviewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleInviewModel.IsSelected = true;
                }
                else
                {
                    userRoleInviewModel.IsSelected = false;
                }
                model.Add(userRoleInviewModel);

            }

            return View(model);

        }
       
        
        
}
}
