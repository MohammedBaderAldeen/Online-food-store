using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spicy.Data;
using spicy.Models;
using spicy.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext db;

        public CouponsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var coupons = await db.Coupons.ToListAsync();
            return View(coupons);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    var fs = files[0].OpenReadStream();
                    var ms = new MemoryStream();
                    fs.CopyTo(ms);
                    coupon.Picture = ms.ToArray();
                }
                db.Coupons.Add(coupon);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); 
            }
            return View(coupon);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await db.Coupons.FindAsync(id);

            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    var fs = files[0].OpenReadStream();
                    var ms = new MemoryStream();
                    fs.CopyTo(ms);
                    coupon.Picture = ms.ToArray();
                }
                db.Coupons.Update(coupon);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await db.Coupons.FindAsync(id);

            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Coupon coupon)
        {
                db.Coupons.Remove(coupon);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await db.Coupons.FindAsync(id);

            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }
    }
}
