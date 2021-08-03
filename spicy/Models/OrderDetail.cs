﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Models
{
    public class OrderDetail
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual OrderHeader OrderHeader { get; set; }
        public int MenuItemId { get; set; }
        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }
        public int Count { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public double Price { get; set; }

    }
}
