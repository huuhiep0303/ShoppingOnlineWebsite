using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Web.Repository;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }   
        public async Task<IActionResult> Search(string searchText)
        {
            var products = await _dataContext.Products
                .Where(p => p.Name.Contains(searchText) || p.Category.Name.Contains(searchText) || p.Brand.Name.Contains(searchText))
                .ToListAsync();

            ViewBag.Keyword = searchText;
            return View(products); 
        }
        public async Task<IActionResult> Details(int ID)
        {
            if (ID == null) return RedirectToAction("Index");
            var productsByID = _dataContext.Products.Where(p => p.CategoryID == ID).FirstOrDefault(); 
            return View(productsByID);
        }

    }
}
