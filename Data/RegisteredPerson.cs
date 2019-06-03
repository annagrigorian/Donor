using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DonorTestWithIndividual.Data
{
    public class RegisteredPerson : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; }
    }   
}
