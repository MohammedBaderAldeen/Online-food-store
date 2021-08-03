using Microsoft.AspNetCore.Authorization;
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
using System.Text;
using System.Threading.Tasks;

namespace spicy.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;

        public OrdersController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var cliam = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsViewModel orderDetailsVM = new OrderDetailsViewModel()
            {
                OrderHeader = await db.OrderHeaders.Include(m=>m.ApplictionUser).FirstOrDefaultAsync(m => m.UserId == cliam.Value && m.id == id),
                OrderDetails = await db.OrderDetails.Where(m => m.OrderId == id).ToListAsync()
            };

            return View(orderDetailsVM);
        }

        private int pageSize = 2;
        [Authorize]
        public async Task<IActionResult> OrderHistory(int pageNumber=1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var cliam = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //List<OrderDetailsViewModel> orderDetailsVMList = new List<OrderDetailsViewModel>();

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders=new List<OrderDetailsViewModel>()
            };

            List<OrderHeader> orderHeadersList = await db.OrderHeaders.Include(m => m.ApplictionUser).Where(m => m.UserId == cliam.Value).ToListAsync();

            foreach (var orderHeader in orderHeadersList)
            {
                OrderDetailsViewModel orderDetailsVM = new OrderDetailsViewModel()
                {
                    OrderHeader = orderHeader,
                    OrderDetails = await db.OrderDetails.Where(m => m.OrderId == orderHeader.id).ToListAsync()
                };
                orderListVM.Orders.Add(orderDetailsVM);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(o => o.OrderHeader.id).Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo()
            {
                CurrentPage=pageNumber,
                RecordPerPage=pageSize,
                TotalRecord=count,
                urlParam= "/Customer/Orders/OrderHistory?pageNumber=:"
            };
            return View(orderListVM);
        }

        public async Task<IActionResult> GetOrderDetails(int id)
        {
            OrderDetailsViewModel orderDetailsVM = new OrderDetailsViewModel()
            {
                OrderHeader = await db.OrderHeaders.Include(m => m.ApplictionUser).FirstOrDefaultAsync(m => m.id == id),
                OrderDetails=await db.OrderDetails.Where(m=>m.OrderId==id).ToListAsync()
            };
            return PartialView("_IndividualOrderDetails", orderDetailsVM);
        }

        public async Task<IActionResult> GetOrderStatus(int id)
        {
            OrderHeader orderHeader = await db.OrderHeaders.FindAsync(id);
            return PartialView("_OrderStatus", orderHeader.Status);
        }

        [Authorize(Roles =SD.ManagerUser+","+SD.KitchenUser)]
        public async Task<IActionResult> ManageOrder()
        {
            List<OrderDetailsViewModel> orderDetailsVMList = new List<OrderDetailsViewModel>();

            List<OrderHeader> orderHeadersList = await db.OrderHeaders.Where(o=>o.Status==SD.StatusSubmited||o.Status==SD.StatusIsProcess).ToListAsync();

            foreach (var orderHeader in orderHeadersList)
            {
                OrderDetailsViewModel orderDetailsVM = new OrderDetailsViewModel()
                {
                    OrderHeader = orderHeader,
                    OrderDetails = await db.OrderDetails.Where(m => m.OrderId == orderHeader.id).ToListAsync()
                };
                orderDetailsVMList.Add(orderDetailsVM);
            }

            
            return View(orderDetailsVMList.OrderBy(o=>o.OrderHeader.PickUpTime).ToList());
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.KitchenUser)]
        public async Task<IActionResult> OrderPrepare(int orderId)
        {
            var orderHeader = await db.OrderHeaders.FindAsync(orderId);
            orderHeader.Status = SD.StatusIsProcess;
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.KitchenUser)]
        public async Task<IActionResult> OrderReady(int orderId)
        {
            var orderHeader = await db.OrderHeaders.FindAsync(orderId);
            orderHeader.Status = SD.StatusReady;
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.KitchenUser)]
        public async Task<IActionResult> OrderCancel(int orderId)
        {
            var orderHeader = await db.OrderHeaders.FindAsync(orderId);
            orderHeader.Status = SD.StatusCanceled;
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }

        [Authorize(Roles = SD.ManagerUser+","+SD.FrontDeskUser)]
        public async Task<IActionResult> OrderPickup(int pageNumber = 1,String searchName=null,String searchPhone=null,String searchEmail=null)
        {

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            StringBuilder param = new StringBuilder();
            param.Append("/Customer/Orders/OrderPickup?pageNumber=:");
            param.Append("&searchName=");
            if(searchName!=null)
            {
                param.Append(searchName);
            }
            else
            {
                searchName = "";
            }

            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            else
            {
                searchPhone = "";
            }

            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            else
            {
                searchEmail = "";
            }

            List<OrderHeader> orderHeadersList = await db.OrderHeaders.Include(m => m.ApplictionUser).OrderByDescending(o => o.OrderDate)
                .Where(m => m.Status == SD.StatusReady && m.PickUpName.Contains(searchName) && m.PhoneNumber.Contains(searchPhone) && m.ApplictionUser.Email.Contains(searchEmail)).ToListAsync();

            foreach (var orderHeader in orderHeadersList)
            {
                OrderDetailsViewModel orderDetailsVM = new OrderDetailsViewModel()
                {
                    OrderHeader = orderHeader,
                    OrderDetails = await db.OrderDetails.Where(m => m.OrderId == orderHeader.id).ToListAsync()
                };
                orderListVM.Orders.Add(orderDetailsVM);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(o => o.OrderHeader.id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo()
            {
                CurrentPage = pageNumber,
                RecordPerPage = pageSize,
                TotalRecord = count,
                urlParam = param.ToString()
            };
            return View(orderListVM);
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.FrontDeskUser)]
        [HttpPost]
        public async Task<IActionResult> OrderPickup(int orderId)
        {
            var orderHeader = await db.OrderHeaders.FindAsync(orderId);
            orderHeader.Status = SD.StatusCompleted;
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(OrderPickup));
        }

    }
}
