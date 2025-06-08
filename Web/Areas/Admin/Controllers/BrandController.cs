using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/Brand")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        [Route("Index")]
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<BrandModel> brand = _dataContext.Brands.ToList(); // 33 datas
            const int pageSize = 5;

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
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(id);
            return View(brand);
        }
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã có trong database");
                    return View(brand);
                }
                _dataContext.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm thương hiệu thành công";
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
        [Route("Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                // Kiểm tra slug trùng nhưng phải loại trừ chính brand đang sửa
                var slug = await _dataContext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug && p.Id != brand.Id);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã có trong database");
                    return View(brand);
                }
                // Lấy entity từ DB rồi cập nhật từng trường
                var existedBrand = await _dataContext.Brands.FindAsync(brand.Id);
                if (existedBrand == null)
                {
                    return NotFound();
                }
                existedBrand.Name = brand.Name;
                existedBrand.Description = brand.Description;
                existedBrand.Slug = brand.Slug;
                existedBrand.Status = brand.Status;
                _dataContext.Update(existedBrand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật thương hiệu thành công";
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
            // return View(brand); // Không cần dòng này nữa
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _dataContext.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            // Kiểm tra xem có sản phẩm nào dùng brand này không
            bool hasProduct = await _dataContext.Products.AnyAsync(p => p.BrandID == id);
            if (hasProduct)
            {
                TempData["error"] = "Không thể xóa thương hiệu vì đang có sản phẩm sử dụng!";
                return RedirectToAction("Index");
            }
            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa thương hiệu thành công";
            return RedirectToAction("Index");
        }
    }
}
