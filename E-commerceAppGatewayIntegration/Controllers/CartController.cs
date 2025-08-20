using Microsoft.AspNetCore.Mvc;
using E_commerceAppGatewayIntegration.Models;
using E_commerceAppGatewayIntegration.Services;
using Newtonsoft.Json;

public class CartController : Controller
{
    private readonly ProductService _productService;

    public CartController(ProductService productService)
    {
        _productService = productService;
    }

    public IActionResult Index()
    {
        var cart = GetCartFromSession();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] ProductIdRequest request)
    {
        var products = await _productService.GetProductsAsync();
        var product = products.FirstOrDefault(p => p.Id == request.Id);

        if (product == null)
            return Json(new { success = false, message = "Product not found" });

        var cart = GetCartFromSession();
        cart.Add(new ProductEntity
        {
            Id = product.Id,
            Title = product.Title,
            Price = product.Price,
            Category = product.Category,
            Image = product.Image
        });
        SaveCartToSession(cart);

        return Json(new { success = true, count = cart.Count });
    }

    public class ProductIdRequest { public int Id { get; set; } }


    public IActionResult ClearCart()
    {
        HttpContext.Session.Remove("Cart");
        return RedirectToAction("Index");
    }

    private List<ProductEntity> GetCartFromSession()
    {
        var cartJson = HttpContext.Session.GetString("Cart");
        return cartJson == null ? new List<ProductEntity>() : JsonConvert.DeserializeObject<List<ProductEntity>>(cartJson);
    }

    private void SaveCartToSession(List<ProductEntity> cart)
    {
        HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
    }
}
