using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Web.Models;
using System.Security.Claims;
using Web.Repository;

namespace Web.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly DataContext _dataContext;
        public ReviewController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult Review(int productId, int orderId)
        {
            var model = new ReviewModel
            {
                ProductID = productId,
                OrderID = orderId
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(ReviewModel model)
        {
            if (ModelState.IsValid)
            {
                model.CustomerEmail = User.FindFirstValue(ClaimTypes.Email);
                model.CreatedDate = DateTime.Now;
                _dataContext.Add(model);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cảm ơn bạn đã đánh giá sản phẩm!";
                return RedirectToAction("OrderDetails", "Checkout", new { id = model.OrderID });
            }
            return View(model);
        }
    }
}