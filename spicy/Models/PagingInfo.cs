using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Models
{
    public class PagingInfo
    {
        public int TotalRecord { get; set; }
        public int RecordPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int ToltalPages => (int)Math.Ceiling((Decimal)TotalRecord / RecordPerPage);
        public String urlParam { get; set; }
    }
}
