using AutoMapper;
using ECommerce.Api.Products.Db;
using ECommerce.Api.Products.Profiles;
using ECommerce.Api.Products.Providers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ECommerce.Api.Products.Tests
{
    public class ProductsServiceTests
    {       
        private ProductsDbContext dbContext;
        private Mapper mapper;
        public ProductsServiceTests()
        {
            if (dbContext == null)
            {
                var options = new DbContextOptionsBuilder<ProductsDbContext>().UseInMemoryDatabase("TestProducts").Options;
                dbContext = new ProductsDbContext(options);
                CreateProducts(dbContext);

                var productsProfile = new ProductProfile();
                var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productsProfile));
                mapper = new Mapper(configuration);
            }
        }

        private void CreateProducts(ProductsDbContext dbContext)
        {
            for (int i = 1; i < 10; i++)
            {
                dbContext.Products.Add(new Product()
                {
                    Id = i,
                    Name = Guid.NewGuid().ToString(),
                    Inventory = i + 10,
                    Price = (decimal)(i * 3.13)
                });
            }
            dbContext.SaveChanges();
        }


        [Fact]
        public async Task GetProductReturnsProductUsingValidId()
        {            
            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var result = await productsProvider.GetProductAsync(1);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Product);
            Assert.True(result.Product.Id == 1);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public async Task GetProductReturnsProductUsingInvalidId()
        {
            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var result = await productsProvider.GetProductAsync(-1);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Product);           
            Assert.NotNull(result.ErrorMessage);
        }

        [Fact]
        public async Task GetProductsResultsAllProducts()
        {

            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var result = await productsProvider.GetProductsAsync();

            Assert.True(result.IsSuccess);
            Assert.True(result.Products.Any());
            Assert.Null(result.ErrorMessage);
        }

    }
}
