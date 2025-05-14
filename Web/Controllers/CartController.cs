using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.ViewModels;
using Web.Repositoty;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext context)
        {
            _dataContext = context;
        }

        public IActionResult Index()
        {
            List<CartItemModel> CartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new()
            {
                CartItems = CartItems,
                GrandTotal = CartItems.Sum(x => x.Quantity * x.Price)
            };
            return View(cartVM);
        }
        public ActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }
        public async Task<IActionResult> Add(int ID)
        {
            ProductModel product = await _dataContext.Products.FindAsync(ID);
            List<CartItemModel> Cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItems = Cart.Where(c => c.ProductID == ID).FirstOrDefault();
            if (cartItems == null)
            {
                Cart.Add(new CartItemModel(product));
            } else
            {
                cartItems.Quantity += 1;
            }
            HttpContext.Session.SetJson("Cart", Cart);

            TempData["success"] = "Added Item to cart successfully!";
            return Redirect(Request.Headers["Referer"].ToString());
        }
        public async Task<IActionResult> Increase(int ID)
        {
            List<CartItemModel> Cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = Cart.Where(c => c.ProductID == ID).FirstOrDefault();
            if (cartItem.Quantity >= 1)
            {
                ++cartItem.Quantity;
            }
            else
            {
                Cart.RemoveAll(p => p.ProductID == ID);
            }
            if (Cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", Cart);
            }
            TempData["success"] = "Increased Item quantity to cart successfully!";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Decrease(int ID)
        {
            List<CartItemModel> Cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = Cart.Where(c => c.ProductID == ID).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            } else
            {
                Cart.RemoveAll(p => p.ProductID == ID);
            }
            if (Cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            } else
            {
                HttpContext.Session.SetJson("Cart", Cart);
            }
            TempData["success"] = "Decreased Item quantity to cart successfully!";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(int ID)
        {
            List<CartItemModel> Cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            Cart.RemoveAll(p => p.ProductID == ID);
            if (Cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            } else
            {
                HttpContext.Session.SetJson("Cart", Cart);
            }
            TempData["success"] = "Remove Item of cart successfully!";
            return RedirectToAction("Index");
        }
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Clear all Item of cart successfully!";
            return RedirectToAction("Index");
        }
    }
}
