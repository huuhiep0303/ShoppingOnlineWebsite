using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Author")]
    [Route("Admin/Order")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext context)
        {
            _dataContext = context;
        }
        [Route("Index")]
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<OrderModel> order = _dataContext.Orders.ToList(); // 33 datas
            const int pageSize = 10;

            if (pg < 1) //page < 1;
            {
                pg = 1; //page ==1
            }
            int recsCount = order.Count(); //33 items;
            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize; //(3 - 1) * 10; 

            //category.Skip(20).Take(10).ToList()

            var data = order.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            return View(data);
        }
        [HttpGet]
        [Route("ViewOrder/{orderCode}")]
        public async Task<IActionResult> ViewOrder(string orderCode)
        {
            var detailsOrder = await _dataContext.OrderDetails.Include(od => od.Product).Include(od => od.Order).Where(od => od.OrderCode == orderCode).ToListAsync();
            return View(detailsOrder);
        }
        [HttpPost]
        [Route("UpdateOrder")]

        public async Task<IActionResult> UpdateOrder(string orderCode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
            if (order == null)
            {
                return NotFound();
            }
            if (order.Status != (OrderStatus)status && status == 0)
            {
                var orderDetails = await _dataContext.OrderDetails
                    .Where(od => od.OrderCode == orderCode)
                    .ToListAsync();

                foreach (var detail in orderDetails)
                {
                    var product = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == detail.ProductID);
                    if (product == null)
                    {
                        return BadRequest(new { success = false, message = $"Không tìm thấy sản phẩm với ID {detail.ProductID}" });
                    }

                    if (product.StockQuantity < detail.Quantity)
                    {
                        return BadRequest(new { success = false, message = $"Sản phẩm {product.Name} không đủ hàng tồn." });
                    }

                    product.StockQuantity -= detail.Quantity;
                }
            }
            order.Status = (OrderStatus)status;
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Order updated successfully" });
            }
            catch (Exception e)
            {
                return StatusCode(500, "Updated error");
            }
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _dataContext.Orders.FindAsync(id);
            _dataContext.Orders.Remove(order);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
