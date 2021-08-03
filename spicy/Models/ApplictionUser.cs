using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Models
{
    public class ApplictionUser:IdentityUser
    {
        public String Name { get; set; }
        public String StreetAddress { get; set; }
        public String PostalCode { get; set; }
        public String City { get; set; }
        public String State { get; set; }
    }
}
