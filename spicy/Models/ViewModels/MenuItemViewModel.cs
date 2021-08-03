using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Models.ViewModels
{
    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }

        public IEnumerable<Category> categoriesList { get; set; }

        public IEnumerable<SubCategory> subCategoriesList { get; set; }
    }
}
