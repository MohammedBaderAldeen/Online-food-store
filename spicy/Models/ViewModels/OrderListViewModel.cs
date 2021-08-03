using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Models.ViewModels
{
    public class OrderListViewModel
    {
        public List<OrderDetailsViewModel> Orders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
