using ECommerce.Api.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService ordersService;
        private readonly IProductsService productsService;
        private readonly ICustomersService customersService;

        public SearchService(IOrdersService ordersService, IProductsService productsService, ICustomersService customersService)
        {
            this.ordersService = ordersService;
            this.productsService = productsService;
            this.customersService = customersService;
        }
        public async Task<(bool IsSuccess, dynamic SearchResult)> SearchAsync(int customerId)
        {
            var customerResult = await customersService.GetCustomerAsync(customerId);
            var productsResult = await productsService.GetProductsAsync();
            var ordersResults = await ordersService.GetOrdersAsync(customerId);

            if(ordersResults.IsSuccess)
            {
                foreach (var order in ordersResults.Orders)
                    foreach (var item in order.Items)
                        item.ProductName = productsResult.IsSuccess ?
                                                productsResult.Products.FirstOrDefault(rec => rec.Id == item.ProductId)?.Name :
                                                "Product information is not aviliable";

                var result = new
                {
                    Customer = customerResult.IsSuccess ?
                                customerResult.Customer :
                                new { Name = "Customer information is not aviliable" },
                    Orders = ordersResults.Orders
                };
                return (true, result);
            }
            return (false, null);
        }
    }
}
