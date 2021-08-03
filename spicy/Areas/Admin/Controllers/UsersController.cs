using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spicy.Data;
using spicy.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace spicy.Areas.Admin.Controllers
{
    [Authorize(Roles =SD.ManagerUser)]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;

        public UsersController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            String UserId = claim.Value;
            return View(await db.ApplictionUsers.Where(m=>m.Id != UserId).ToListAsync());
        }

        public async Task<IActionResult> LockUnLock(String  id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await db.ApplictionUsers.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if(user.LockoutEnd==null||user.LockoutEnd<DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddHours(6);
            }
            else
            {
                user.LockoutEnd = DateTime.Now;
            }
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
