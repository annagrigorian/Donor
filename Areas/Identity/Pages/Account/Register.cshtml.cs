using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DonorTestWithIndividual.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DonorTestWithIndividual.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<RegisteredPerson> _signInManager;
        private readonly UserManager<RegisteredPerson> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        //private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private List<BloodGroup> bloodGroups = new List<BloodGroup>();

        public RegisterModel(
            //IServiceCollection collection,
            UserManager<RegisteredPerson> userManager,
            SignInManager<RegisteredPerson> signInManager,
            ILogger<RegisterModel> logger,
           // IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
           // _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Blood Group")]
            public BloodGroup BloodGroup { get; set; }

            public Roles Role { get; set; }

            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }
        }

        public SelectList SelectBloodGroup()
        {            
            bloodGroups.Add(BloodGroup.AB_Minus);
            bloodGroups.Add(BloodGroup.AB_Plus);
            bloodGroups.Add(BloodGroup.O_Minus);
            bloodGroups.Add(BloodGroup.O_Plus);
            bloodGroups.Add(BloodGroup.A_Minus);
            bloodGroups.Add(BloodGroup.A_Plus);
            bloodGroups.Add(BloodGroup.B_Minus);
            bloodGroups.Add(BloodGroup.B_Plus);

            var models = bloodGroups.Select(group => new BloodGroupDropdownModel
            {
                Name = group.ToString(),
                Id = (int)group
            });

            var selectedBloodGroups = new SelectList(models, "Id", "Name");

            return selectedBloodGroups;
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            
            ViewData["BloodGroup"] = SelectBloodGroup();            
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new RegisteredPerson
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    EmailConfirmed = true,
                    BloodGroup = Input.BloodGroup,
                    UserName = Input.Email,
                    Email = Input.Email,
                    Role = Roles.CommonUser
                };                
                
                var result = _userManager.CreateAsync(user).Result;

                if (result.Succeeded)
                    {                  
                    if (!(await _roleManager.RoleExistsAsync("CommonUser")))
                        {
                        var roleResult = _roleManager.CreateAsync(new IdentityRole { Name = "CommonUser" }).Result;
                        }
                    
                        await _userManager.AddToRoleAsync(user, "CommonUser");                
                    } 
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index", "Home");
        }
    }
}
