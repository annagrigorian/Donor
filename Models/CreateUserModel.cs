using DonorTestWithIndividual.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DonorTestWithIndividual.Models
{
    public class CreateUserModel
    {        
       // [Required]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "Enter Email Address")]
        //[EmailAddress(ErrorMessage = "Invalid Email address.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public Roles Role { get; set; }
        
        [Required(ErrorMessage = "Enter password")]
        [DataType(DataType.Password,ErrorMessage = "Invalid Password.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Enter password once again to confirm.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; }
    }
}
