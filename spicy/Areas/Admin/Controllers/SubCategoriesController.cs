using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using spicy.Data;
using spicy.Models;
using spicy.Models.ViewModels;
using spicy.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        [TempData]
        public String StatusMessage { get; set; }

        public SubCategoriesController(ApplicationDbContext db)
        {
            this.db = db;
        }
       
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var subCategories = await db.SubCategory.Include(m => m.Category).ToListAsync();
            return View(subCategories);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoriesList = await db.Categories.ToListAsync(),
                subCategory = new Models.SubCategory(),
                //distinct  mn 2gl mn3 tkrar
                //subCategoriesList=await db.SubCategory.OrderBy(m=>m.Name).Select(m=>m.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesExistSubCategory = await db.SubCategory.Include(m => m.Category).Where(m => m.Category.id == model.subCategory.CategoryId && m.Name == model.subCategory.Name).ToListAsync();

                if (doesExistSubCategory.Count() > 0)
                {
                    StatusMessage = "Error : this is Sub Category Exists under " + doesExistSubCategory.FirstOrDefault().Category.Name + " Ctaegory";
                }
                else
                {
                    db.SubCategory.Add(model.subCategory);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoriesList = await db.Categories.ToListAsync(),
                subCategory = model.subCategory,
                statusMessage = StatusMessage
            };
            return View(modelVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubCategories(int id)
        {
            List<SubCategory> subCategoies = new List<SubCategory>();
            subCategoies = await db.SubCategory.Where(m => m.CategoryId == id).ToListAsync();
            return Json(new SelectList(subCategoies, "id", "Name"));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await db.SubCategory.FindAsync(id);

            if(subCategory==null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoriesList = await db.Categories.ToListAsync(),
                subCategory = subCategory,
                //distinct  mn 2gl mn3 tkrar
                //subCategoriesList=await db.SubCategory.OrderBy(m=>m.Name).Select(m=>m.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesExistSubCategory = await db.SubCategory.Include(m => m.Category).Where(m => m.Category.id == model.subCategory.CategoryId && m.Name == model.subCategory.Name && m.id != model.subCategory.id).ToListAsync();

                if (doesExistSubCategory.Count() > 0)
                {
                    StatusMessage = "Error : this is Sub Category Exists under " + doesExistSubCategory.FirstOrDefault().Category.Name + " Ctaegory";
                }
                else
                {
                    db.SubCategory.Update(model.subCategory);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoriesList = await db.Categories.ToListAsync(),
                subCategory = model.subCategory,
                statusMessage = StatusMessage
            };
            return View(modelVM);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = db.SubCategory.Include(m=>m.Category).Where(m=>m.id==id).SingleOrDefault();

            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> Delete(SubCategory subCategory)
        {
            db.SubCategory.Remove(subCategory);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = db.SubCategory.Include(m => m.Category).Where(m => m.id == id).SingleOrDefault();

            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }
    }
}
