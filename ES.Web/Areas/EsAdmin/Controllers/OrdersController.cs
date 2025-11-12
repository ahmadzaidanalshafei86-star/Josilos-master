using ES.Web.Areas.EsAdmin.Repositories;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class OrdersController : Controller
    {
        private readonly OrdersRepository _ordersRepository;

        public OrdersController(OrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _ordersRepository.GetPendingOrdersAsync();
            return View(orders);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetOrder(int id)
        //{
        //    var order = await _ordersRepository.GetOrderByIdAsync(id);

        //    if (order == null)
        //        return NotFound();

        //    return Json(new
        //    {
        //        id = order.Id,
        //        name = order.CustomerFullName,
        //        mobilePhone = order.MobilePhone,
        //        address = order.StreetAddress,
        //        paymentMethod = order.PaymentMethod,
        //        estimatedTotal = order.EstimatedTotal,
        //        isDelivered = order.IsDelivered,
        //        isCancelled = order.IsCancelled,
        //        createdAt = order.CreatedAt,
        //        items = order.Items.Select(i => new
        //        {
        //            productTitle = i.ProductTitle,
        //            customizationName = i.CustomizationName,
        //            quantity = i.Quantity,
        //            selectedAttributes = i.SelectedAttributes.Select(a => new { a.Value }).ToList(),
        //        }).ToList()
        //    });
        //}

        public async Task<IActionResult> CompletedAndCanceledOrders()
        {
            var orders = await _ordersRepository.GetCompletedAndCanceledOrdersAsync();
            return View("Index", orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredOrders(string mobile = "", string sort = "newest", string status = "pending")
        {
            var orders = await _ordersRepository.GetFilteredOrdersAsync(mobile, sort, status);
            return Json(orders);
        }

        [HttpPost]
        [Route("Orders/CompleteOrder/{orderId}")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var success = await _ordersRepository.CompleteOrderAsync(orderId);
            if (success)
            {
                return Ok();
            }
            return StatusCode(500, "Error completing order");
        }

        [HttpPost]
        [Route("Orders/CancelOrder/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var success = await _ordersRepository.CancelOrderAsync(orderId);
            if (success)
            {
                return Ok();
            }
            return StatusCode(500, "Error canceling order");
        }
    }
}

