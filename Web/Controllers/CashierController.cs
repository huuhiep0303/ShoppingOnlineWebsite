using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Repository;

namespace Web.Controllers
{
    public class CashierController : Controller
    {
        private readonly DataContext _dataContext;
        public CashierController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> CheckOrderByOrderId(int orderId)
        {
            
        }
        public async Task<IActionResult> CheckOrderByCustomerId(int customerId)
        {

        }
        public async Task<IActionResult> InsertOrderById(int customerId, OrderDetails)
    }
}
