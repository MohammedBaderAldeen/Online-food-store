using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spicy.Data;
using spicy.Models;
using spicy.Models.ViewModels;
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
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemsController(ApplicationDbContext db,IWebHostEnvironment webHostEnvironment)
        {
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                MenuItem = new MenuItem(),
                categoriesList = db.Categories.ToList()
            };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var menuItems = await db.menuItems.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuItems);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost()
        {
            if(ModelState.IsValid)
            {
                String ImagePath = @"\img\default-food.png";
                var files = HttpContext.Request.Form.Files;
                if(files.Count > 0)
                {
                    String webRootPath = webHostEnvironment.WebRootPath;
                    String ImageName = DateTime.Now.ToFileTime().ToString() + Path.GetExtension(files[0].FileName);
                    FileStream fileStream = new FileStream(Path.Combine(webRootPath, "img", ImageName), FileMode.Create);
                    files[0].CopyTo(fileStream);

                    ImagePath = @"\img\" + ImageName;
                }

                MenuItemVM.MenuItem.Image = ImagePath;
                db.menuItems.Add(MenuItemVM.MenuItem);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(MenuItemVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var menuItem = db.menuItems.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefault(m=>m.id==id);

            if(menuItem==null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = menuItem;
            MenuItemVM.subCategoriesList = await db.SubCategory.Where(m => m.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost()
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    String webRootPath = webHostEnvironment.WebRootPath;
                    String ImageName = DateTime.Now.ToFileTime().ToString() + Path.GetExtension(files[0].FileName);
                    FileStream fileStream = new FileStream(Path.Combine(webRootPath, "img", ImageName), FileMode.Create);
                    files[0].CopyTo(fileStream);

                    String ImagePath = @"\img\" + ImageName;
                    MenuItemVM.MenuItem.Image = ImagePath;
                }

                
                db.menuItems.Update(MenuItemVM.MenuItem);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(MenuItemVM);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = db.menuItems.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefault(m => m.id == id);

            if (menuItem == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = menuItem;
            MenuItemVM.subCategoriesList = await db.SubCategory.Where(m => m.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = db.menuItems.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefault(m => m.id == id);

            if (menuItem == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = menuItem;
            MenuItemVM.subCategoriesList = await db.SubCategory.Where(m => m.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost()
        {
                db.menuItems.Remove(MenuItemVM.MenuItem);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }
    }
}
