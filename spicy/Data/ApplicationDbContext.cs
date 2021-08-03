using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using spicy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace spicy.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<SubCategory> SubCategory { get; set; }

        public DbSet<MenuItem> menuItems { get; set; }

        public DbSet<Coupon> Coupons { get; set; }

        public DbSet<ApplictionUser> ApplictionUsers { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
