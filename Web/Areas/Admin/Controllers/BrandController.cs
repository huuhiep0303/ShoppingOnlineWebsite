using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Author")]
    [Route("Admin/Brand")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        [HttpGet("Index")]
        [Route("Index")]
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<BrandModel> brand = _dataContext.Brands.ToList(); // 33 datas
            const int pageSize = 10;

            if (pg < 1) //page < 1;
            {
                pg = 1; //page == 1
            }
            int recsCount = brand.Count(); //33 items;
            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize; //(3 - 1) * 10; 

            //category.Skip(20).Take(10).ToList()

            var data = brand.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            return View(data);
        }
        [HttpGet("Create")]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(id);
            return View(brand);
        }
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Brand has already had in Database!");
                    return View(brand);
                }
                _dataContext.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Insert successful!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model has error.";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMsg = string.Join("\n", errors);
                return BadRequest(errorMsg);
            }
            return View(brand);
        }
        [HttpPost("Edit/{edit}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                slug.Name = brand.Name;
                slug.Description = brand.Description;
                slug.Slug = brand.Slug;
                //_dataContext.Update(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Update successful";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model has error.";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    } 
                }
                string errorMsg = string.Join("\n", errors);
                return BadRequest(errorMsg);
            }
            return View(brand);
        }
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _dataContext.Brands.FindAsync(id);
            if (brand == null)
                return NotFound();

            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Delete successful!";
            return RedirectToAction("Index");
        }
    }
}
