using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web.Models;
using Web.Models.ViewModels;
using Web.Repository;

namespace Web.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        public CheckoutController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = orderCode;
                orderItem.CustomerName = userEmail;
                orderItem.Status = 1;
                orderItem.CreatedDate = DateTime.Now;
                _dataContext.Add(orderItem);
                await _dataContext.SaveChangesAsync();
                List<CartItemModel> CartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var cart in CartItems)
                {
                    var orderDetail = new OrderDetails();
                    orderDetail.CustomerName = userEmail;
                    orderDetail.OrderCode = orderCode;
                    orderDetail.ProductID = cart.ProductID;
                    orderDetail.Quantity = cart.Quantity;
                    orderDetail.Price = cart.Price;
                    _dataContext.Add(orderDetail);
                    await _dataContext.SaveChangesAsync();
                }
                HttpContext.Session.Remove("Cart");
                TempData["success"] = "Đặt hàng thành công, vui lòng chờ duyệt đơn hàng";
                return RedirectToAction("Index", "Cart");
            }
            return View();
        }
        public async Task<IActionResult> MyOrders()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
                return RedirectToAction("Login", "Account");

            var orders = await _dataContext.Orders
                .Where(o => o.CustomerName == userEmail)
                .OrderByDescending(o => o.CreatedDate).ToListAsync();

            return View(orders);
        }
        public async Task<IActionResult> OrderDetails(int id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.Id == id && o.CustomerName == userEmail);
            if (order == null) return NotFound();

            var details = await _dataContext.OrderDetails
                .Where(d => d.OrderCode == order.OrderCode)
                .Include(d => d.Product)
                .ToListAsync();

            ViewBag.Order = order;
            return View(details);
        }
    }
}
