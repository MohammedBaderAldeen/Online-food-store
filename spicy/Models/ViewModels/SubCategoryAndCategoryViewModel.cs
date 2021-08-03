using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Models.ViewModels
{
    public class SubCategoryAndCategoryViewModel
    {
        public IEnumerable<Category> CategoriesList { get; set; }

        public SubCategory subCategory { get; set; }

        public String statusMessage { get; set; }
    }
}
