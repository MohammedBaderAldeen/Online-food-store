using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spicy.Data;
using spicy.Models;
using spicy.Models.ViewModels;
using spicy.Utility;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace spicy.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext db;

        public CartsController(ApplicationDbContext db)
        {
            this.db = db;
        }
        [BindProperty]
        public OrderDetailsCartViewModel OrderDetailsCartVM { get; set; }

        public IActionResult Index()
        {
            OrderDetailsCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new Models.OrderHeader()
            };
            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingCarts = db.ShoppingCarts.Where(m => m.ApplicationUserId == claim.Value);

            if(shoppingCarts !=null)
            {
                OrderDetailsCartVM.ShoppingCartsList = shoppingCarts.ToList();
            }

            foreach (var item in OrderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = db.menuItems.FirstOrDefault(m => m.id == item.MenuItemId);
                OrderDetailsCartVM.OrderHeader.OrderTotal += item.MenuItem.Price * item.Count;
                OrderDetailsCartVM.OrderHeader.OrderTotal = Math.Round(OrderDetailsCartVM.OrderHeader.OrderTotal, 2);
                item.MenuItem.Description = SD.ConvertToRawHtml(item.MenuItem.Description);

                if(item.MenuItem.Description.Length>75)
                {
                    item.MenuItem.Description = item.MenuItem.Description.Substring(0, 74);
                }
            }

            OrderDetailsCartVM.OrderHeader.OrderTotalOrginal = OrderDetailsCartVM.OrderHeader.OrderTotal;

            if(HttpContext.Session.GetString(SD.ssCouponCode)!= null)
            {
                OrderDetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);

                var couponFormDB = db.Coupons.Where(m => m.Name.ToLower() == OrderDetailsCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefault();

                OrderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(couponFormDB, OrderDetailsCartVM.OrderHeader.OrderTotalOrginal);
            }

            return View(OrderDetailsCartVM);
        }

        public IActionResult Summary()
        {
            OrderDetailsCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new Models.OrderHeader()
            };
            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var appUser = db.ApplictionUsers.Find(claim.Value);

            OrderDetailsCartVM.OrderHeader.PickUpName = appUser.Name;
            OrderDetailsCartVM.OrderHeader.PhoneNumber = appUser.PhoneNumber;
            OrderDetailsCartVM.OrderHeader.PickUpTime = DateTime.Now;

            var shoppingCarts = db.ShoppingCarts.Where(m => m.ApplicationUserId == claim.Value);

            if (shoppingCarts != null)
            {
                OrderDetailsCartVM.ShoppingCartsList = shoppingCarts.ToList();
            }

            foreach (var item in OrderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = db.menuItems.FirstOrDefault(m => m.id == item.MenuItemId);
                OrderDetailsCartVM.OrderHeader.OrderTotal += item.MenuItem.Price * item.Count;
                OrderDetailsCartVM.OrderHeader.OrderTotal = Math.Round(OrderDetailsCartVM.OrderHeader.OrderTotal, 2);
            }

            OrderDetailsCartVM.OrderHeader.OrderTotalOrginal = OrderDetailsCartVM.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                OrderDetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);

                var couponFormDB = db.Coupons.Where(m => m.Name.ToLower() == OrderDetailsCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefault();

                OrderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(couponFormDB, OrderDetailsCartVM.OrderHeader.OrderTotalOrginal);
            }

            return View(OrderDetailsCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(String stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


           OrderDetailsCartVM.ShoppingCartsList = await db.ShoppingCarts.Where(m => m.ApplicationUserId == claim.Value).ToListAsync();

            OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.OrderDate = DateTime.Now;
            OrderDetailsCartVM.OrderHeader.UserId = claim.Value;
            OrderDetailsCartVM.OrderHeader.Status = SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.PickUpTime = Convert.ToDateTime(OrderDetailsCartVM.OrderHeader.PickUpDate.ToShortDateString() + " "
                + OrderDetailsCartVM.OrderHeader.PickUpTime.ToShortTimeString());
            OrderDetailsCartVM.OrderHeader.OrderTotalOrginal = 0;

            db.OrderHeaders.Add(OrderDetailsCartVM.OrderHeader);
            await db.SaveChangesAsync();

            foreach (var item in OrderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = db.menuItems.FirstOrDefault(m => m.id == item.MenuItemId);

                OrderDetail orderDetail = new OrderDetail()
                {
                    MenuItemId=item.MenuItemId,
                    OrderId=OrderDetailsCartVM.OrderHeader.id,
                    Description=item.MenuItem.Description,
                    Name=item.MenuItem.Name,
                    Price=item.MenuItem.Price,
                    Count=item.Count
                };

                OrderDetailsCartVM.OrderHeader.OrderTotalOrginal += item.MenuItem.Price * item.Count;
                db.OrderDetails.Add(orderDetail);   
            }


            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                OrderDetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);

                var couponFormDB = db.Coupons.Where(m => m.Name.ToLower() == OrderDetailsCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefault();

                OrderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(couponFormDB, OrderDetailsCartVM.OrderHeader.OrderTotalOrginal);
            }
            else
            {
                OrderDetailsCartVM.OrderHeader.OrderTotal = Math.Round(OrderDetailsCartVM.OrderHeader.OrderTotalOrginal,2);
            }

            OrderDetailsCartVM.OrderHeader.CouponCodeDiscount = OrderDetailsCartVM.OrderHeader.OrderTotalOrginal - OrderDetailsCartVM.OrderHeader.OrderTotal;

            db.ShoppingCarts.RemoveRange(OrderDetailsCartVM.ShoppingCartsList);
            HttpContext.Session.SetInt32(SD.ShoppingCartCount, 0);
            await db.SaveChangesAsync();

            var option = new Stripe.ChargeCreateOptions
            {
                Amount=Convert.ToInt32(OrderDetailsCartVM.OrderHeader.OrderTotal*100),
                Currency="usd",
                Description="Order ID: "+OrderDetailsCartVM.OrderHeader.id,
                Source=stripeToken
            };

            var service = new ChargeService();
            Charge charge = service.Create(option);
            if (charge.BalanceTransactionId == null)
            {
                OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            else
            {
                OrderDetailsCartVM.OrderHeader.TrasactionId = charge.BalanceTransactionId;
            }
            if (charge.Status.ToLower() == "succeeded")
            {
                OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                OrderDetailsCartVM.OrderHeader.Status = SD.StatusSubmited;
            }
            else
            {
                OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }

            await db.SaveChangesAsync();

            //return RedirectToAction("Index","Home");
            return RedirectToAction("Confirm", "Orders", new { id = OrderDetailsCartVM.OrderHeader.id });
        }

        public IActionResult ApplyCoupon()
        {
            if (OrderDetailsCartVM.OrderHeader.CouponCode == null)
            {
                OrderDetailsCartVM.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, OrderDetailsCartVM.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString(SD.ssCouponCode, String.Empty);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var shoppingCart = await db.ShoppingCarts.FindAsync(cartId);

            shoppingCart.Count += 1;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var shoppingCart = await db.ShoppingCarts.FindAsync(cartId);

            if (shoppingCart.Count > 1)
            {
                shoppingCart.Count -= 1;
                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var shoppingCart = await db.ShoppingCarts.FindAsync(cartId);

            db.ShoppingCarts.Remove(shoppingCart);

            await db.SaveChangesAsync();

            var count = db.ShoppingCarts.Where(m => m.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ShoppingCartCount, count);

            return RedirectToAction(nameof(Index));
        }
    }
}
