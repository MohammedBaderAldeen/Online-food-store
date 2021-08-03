using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Models
{
    public class MenuItem
    {
        [Key]
        public int id { get; set; }

        [Required]
        public String Name { get; set; }

        public String Description { get; set; }

        public double Price { get; set; }

        public String Image { get; set; }

        public String Spicyness { get; set; }

        public enum Espicy {NA=0,NotSpicy=1,Spicy=2,VerySpicy=3}
        
        [Display(Name ="Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Display(Name = "Sub Category")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public SubCategory SubCategory { get; set; }
    }

}
