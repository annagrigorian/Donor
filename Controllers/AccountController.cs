using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DonorTestWithIndividual.Data;
using DonorTestWithIndividual.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonorTestWithIndividual.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<RegisteredPerson> _userManager;
        private readonly SignInManager<RegisteredPerson> _signInManager;
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public List<BloodGroup> AreChecked { get; set; }

        List<BloodGroup> bloodGroup = new List<BloodGroup>();
        
        public AccountController(RoleManager<IdentityRole> roleManager,
                                UserManager<RegisteredPerson> userManager, 
                                SignInManager<RegisteredPerson> signInManager,
                                ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateUserView()
        {
            return View();
        }

        public IActionResult Success()
        {
            return RedirectToAction("Index", "Account");
        }

        //public IActionResult LoginAccount()
        //{
        //    return RedirectToAction("Index", "Account");
        //}

        public IActionResult Failed()
        {
            return RedirectToAction("Failed", "Account");
        }

        public List<BloodGroup> GetBloodGroups()
        {
            bloodGroup.Add(BloodGroup.AB_Minus);
            bloodGroup.Add(BloodGroup.AB_Plus);
            bloodGroup.Add(BloodGroup.O_Minus);
            bloodGroup.Add(BloodGroup.O_Plus);
            bloodGroup.Add(BloodGroup.A_Minus);
            bloodGroup.Add(BloodGroup.A_Plus);
            bloodGroup.Add(BloodGroup.B_Minus);
            bloodGroup.Add(BloodGroup.B_Plus);

            return bloodGroup;
        }

        public ActionResult HospitalIndex(RegisteredPerson loginModel)
        {
            ViewBag.Hospital = loginModel;
            ViewBag.BloodGroups = GetBloodGroups();
            return View();
        }

        public ActionResult GetHospitalUsers()
        {
            var hospitals = _context.Users.Select(user => user).Where(user => user.Role == Roles.HospitalUser).ToList();
            ViewBag.Hospitals = hospitals;
            return View();
        }

        public ActionResult GetCommonUsers()
        {
            var users = _context.Users.Select(user => user).Where(user => user.Role == Roles.CommonUser).ToList();
            ViewBag.CommonUsers = users;        
            return View();
        }


        public ActionResult SendEmail()
        {
            ViewBag.Selected = AreChecked;
            return Content("Emails were sent succesfully");
        }

        public async Task<IActionResult> DeleteHospital(CreateUserModel createUserModel)
        {
            var user = await _userManager.FindByEmailAsync(createUserModel.Email);

            _context.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("GetHospitalUsers", "Account");
        }

        
        public ActionResult EditHospital(CreateUserModel createUserModel)
        {
            return View();
        }

        [HttpPut]
        public ActionResult Save(CreateUserModel createUserModel)
        {
            var hospital = _userManager.FindByEmailAsync(createUserModel.Email);

            _context.Entry(hospital).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("GetHospitalUsers", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync(CreateUserModel createUserModel)
        {
            if (ModelState.IsValid)
            {
                var user = new RegisteredPerson
                {
                    FirstName = createUserModel.UserName,
                    UserName = createUserModel.Email,
                    Email = createUserModel.Email,
                    Password = createUserModel.Password,
                    Role = Roles.HospitalUser
                };

                var result = await _userManager.CreateAsync(user, user.Password);
                await _signInManager.SignInAsync(user, false);

                if (result.Succeeded)
                {
                    if (!(await _roleManager.RoleExistsAsync("HospitalUser")))
                    {
                        var roleresult = _roleManager.CreateAsync(new IdentityRole { Name = "HospitalUser" }).Result;

                        if (roleresult.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, "HospitalUser");                          
                        }
                    }
                    return RedirectToAction("Success", "Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Content("Failed to create new hospital");
        }
    }
}