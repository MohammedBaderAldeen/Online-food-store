using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Models
{
    public class Coupon
    {
        [Key]
        public int id { get; set; }

        [Required]
        public String Name { get; set; }

        [Required]
        public String CouponType { get; set; }

        public enum EcouponType { Percent=0,Doller=1 }

        [Required]
        public double Discount { get; set; }

        [Display(Name = "Minimum Amount")]
        public double MinimumAmount { get; set; }

        public byte[] Picture { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
