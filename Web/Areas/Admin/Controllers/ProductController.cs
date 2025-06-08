using Azure.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Author")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<ProductModel> product = _dataContext.Products.Include(p => p.Category).Include(p => p.Brand).ToList(); // 33 datas
            const int pageSize = 10;
                
            if (pg < 1) //page < 1;
            {
                pg = 1; //page == 1
            }
            int recsCount = product.Count(); //33 items;
            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize; //(3 - 1) * 10; 

            //category.Skip(20).Take(10).ToList()

            var data = product.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            return View(data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryID);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandID);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(product);
                }
                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fileStream);
                    }
                    product.Image = imageName;
                }
                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm sản phẩm thành công";
                return RedirectToAction("Index");
            } else
            {
                TempData["error"] = "Model has error.";
                List<string> errors = new List<string>();
                foreach(var value in ModelState.Values)
                {
                    foreach(var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMsg = string.Join("\n", errors);
                return BadRequest(errorMsg);
            }
            return View(product);   
        } 
        public async Task<IActionResult> Edit(int id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryID);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandID);
            
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryID);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandID);

            var existed_product = _dataContext.Products.Find(product.Id);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                
                if (product.ImageUpload != null)
                {
                    // upload new image
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    // delele old image
                    string oldFilePath = Path.Combine(uploadsDir, existed_product.Image);
                    try
                    {
                        if (System.IO.File.Exists(oldFilePath))
                        {  
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "Lỗi xóa ảnh sản phẩm");
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fileStream);
                    }
                    existed_product.Image = imageName;
                }
                // update properties
                existed_product.Name = product.Name;
                existed_product.Description = product.Description;
                existed_product.Price = product.Price;
                existed_product.CategoryID = product.CategoryID;
                existed_product.BrandID = product.BrandID;

                _dataContext.Update(existed_product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật sản phẩm thành công";
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
            return View(product);
        }
        public async Task<IActionResult> Delete(int id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(id);
            if (!string.Equals(product.Image, "noimage.jpg"))
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                string imagePath = Path.Combine(uploadsDir, product.Image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa sản phẩm thành công";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddNumberProduct(int id, int quantity)
        {
            ProductModel product = await _dataContext.Products.FindAsync(id);
            if (product == null)
            {
                return BadRequest("nahhh! No exist!");
            }
            else
            {
                product.StockQuantity += quantity;
                TempData["Success"] = "Add success!";
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Decrease(int id, int quantity)
        {
            ProductModel product = await _dataContext.Products.FindAsync(id);
            if (product == null)
            {
                return BadRequest("Product isn't exist!");
            }
            else
            {
                product.StockQuantity -= quantity;
                TempData["Success"] = "Decrease success!";
            }
            return RedirectToAction("Index");
        }
    }
}
