using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Models
{
    public class SubCategory
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Display(Name ="Sub Category Name")]
        public String Name { get; set; }

        [Required]
        [Display(Name ="Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
