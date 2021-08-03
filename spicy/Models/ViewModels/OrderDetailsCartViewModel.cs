using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Models.ViewModels
{
    public class OrderDetailsCartViewModel
    {
        public List<ShoppingCart> ShoppingCartsList { get; set; }
        public OrderHeader OrderHeader { get; set; }

    }
}
