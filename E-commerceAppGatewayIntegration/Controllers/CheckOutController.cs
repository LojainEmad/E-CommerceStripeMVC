
using E_commerceAppGatewayIntegration.Models;
using E_commerceAppGatewayIntegration.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Session = Stripe.Checkout.Session;
using SessionService = Stripe.Checkout.SessionService;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using Newtonsoft.Json;

namespace E_commerceAppGatewayIntegration.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly ProductService _productService;

        public CheckOutController(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var apiProducts = await _productService.GetProductsAsync();

            var productList = apiProducts.Select(p => new ProductEntity
            {
                Id = p.Id,               
                Title = p.Title,
                Price = p.Price,
                Category = p.Category,
                Image = p.Image,
                Description= p.Description,
            }).ToList();

            return View(productList);
        }

        public IActionResult OrderConfirmation()
        {
            var service = new SessionService();
            Session session = service.Get(TempData["Session"].ToString());
            if (session.PaymentStatus == "paid")
            {
                var transaction = session.PaymentIntentId?.ToString();
                return View("Success");
            }
            return View("Login");
        }

        public IActionResult Success() => View();
        public IActionResult Login() => View();
        [HttpPost]
        public IActionResult CheckOut()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            var productList = cartJson == null
                ? new List<ProductEntity>()
                : JsonConvert.DeserializeObject<List<ProductEntity>>(cartJson);

            var domain = "http://localhost:5003/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + "CheckOut/OrderConfirmation",
                CancelUrl = domain + "CheckOut/Login",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in productList)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Title
                        }
                    },
                    Quantity = 1
                };
                options.LineItems.Add(sessionListItem);
            }

            var service = new SessionService();
            var session = service.Create(options);

            TempData["Session"] = session.Id;
            return Redirect(session.Url);
        }


        public IActionResult CancelledTransaction()
        {
            return View();
        }

    }
}
