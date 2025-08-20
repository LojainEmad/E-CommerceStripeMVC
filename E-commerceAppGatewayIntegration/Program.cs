using Stripe;
using E_commerceAppGatewayIntegration.Services;
using ProductService = E_commerceAppGatewayIntegration.Services.ProductService;
//using ProductService = Stripe.ProductService;

namespace E_commerceAppGatewayIntegration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add MVC
            builder.Services.AddControllersWithViews();

            // Correctly register ProductService with HttpClient support.
            // This single line handles both the service and the HttpClient setup.
            builder.Services.AddHttpClient<ProductService>();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();

            // Read from appsettings.json or environment variable
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();
            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}