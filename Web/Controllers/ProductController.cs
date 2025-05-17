using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Details(int ID)
        {
            if (ID == null) return RedirectToAction("Index");
            var productsByID = _dataContext.Products.Where(p => p.CategoryID == ID).FirstOrDefault(); 
            return View(productsByID);
        }
    }
}
