using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using E_commerceAppGatewayIntegration.Models;

namespace E_commerceAppGatewayIntegration.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductEntity>> GetProductsAsync()
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductEntity>>(
                "https://fakestoreapi.com/products");

            return products ?? new List<ProductEntity>();
        }
    }
}
