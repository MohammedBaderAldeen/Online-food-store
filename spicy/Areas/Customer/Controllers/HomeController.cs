using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spicy.Data;
using spicy.Models;
using spicy.Models.ViewModels;
using spicy.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace spicy.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;

        public HomeController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                List<ShoppingCart> shoppingCarts = await db.ShoppingCarts.Where(m => m.ApplicationUserId == claim.Value).ToListAsync();
                HttpContext.Session.SetInt32(SD.ShoppingCartCount, shoppingCarts.Count);
            }

            IndexViewModel indexVM = new IndexViewModel()
            {
                Categories =await db.Categories.ToListAsync(),
                Coupons= await db.Coupons.Where(m=>m.IsActive == true).ToListAsync(),
                MenuItems=await db.menuItems.Include(m=>m.Category).Include(m=>m.SubCategory).ToListAsync()
            };
            return View(indexVM);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int itemId)
        {
            var menuItem = await db.menuItems.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.id == itemId).FirstOrDefaultAsync();

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                MenuItem =menuItem,
                MenuItemId=menuItem.id
            };

            return View(shoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                //shoppingCart.id = 0;   way to solve problem id shoppingCart equal id MenuItem
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var cliam = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                shoppingCart.ApplicationUserId = cliam.Value;

                var shoppingCartFromDB = await db.ShoppingCarts.Where(m => m.ApplicationUserId == shoppingCart.ApplicationUserId && m.MenuItemId == shoppingCart.MenuItemId).FirstOrDefaultAsync();

                if(shoppingCartFromDB == null)
                {
                    db.ShoppingCarts.Add(shoppingCart);
                }
                else
                {
                    shoppingCartFromDB.Count += shoppingCart.Count;
                }
                await db.SaveChangesAsync();

                var count = db.ShoppingCarts.Where(m => m.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ShoppingCartCount, count);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menuItem = await db.menuItems.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.id == shoppingCart.MenuItemId).FirstOrDefaultAsync();

                ShoppingCart shoppingCartObj = new ShoppingCart()
                {
                    MenuItem = menuItem,
                    MenuItemId = menuItem.id
                };

                return View(shoppingCart);
            }

        }
    }
}
