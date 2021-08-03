﻿using Microsoft.AspNetCore.Mvc;
using spicy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace spicy.ViewComponents
{
    public class UserNameViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext db;

        public UserNameViewComponent(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userFromDb = await db.ApplictionUsers.FindAsync(claim.Value);
            return View(userFromDb);
        }
    }
}
