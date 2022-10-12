using Bakery.Controllers;
using Bakery.Core.Interfaces;
using Bakery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using Xunit;

namespace Bakery.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task Catalog_ReturnsAViewResult_WithAListOfProducts()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(repo => repo.ListAsync())
                .ReturnsAsync(GetTestProducts());
            var controller = new HomeController(mockRepo.Object, null, null);

            // Act
            var result = await controller.CatalogAsync();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(
                viewResult.ViewData.Model);
            Assert.Equal(3, model.Count());
        }

        private List<Product> GetTestProducts()
        {
            return new List<Product>()
            {
                    new Product
                    {
                        Name = "Французская булочка",
                        Price = 50
                    },
                    new Product
                    {
                        Name = "Тортик кремовый",
                        Price = 240
                    },
                    new Product
                    {
                        Name = "Тортик йогуртовый",
                        Price = 310
                    },
            };
        }

        [Fact]
        public async Task AccessDenied_ReturnsAViewResult_WithCorrectMessage()
        {
            // Arrange
            var controller = new HomeController(null, null, null);

            // Act
            var result = controller.AccessDenied();

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("Access Denied", contentResult.Content);
        }

        [Fact]
        public async Task AddProduct_Get_ReturnsAViewResult()
        {
            // Arrange
            var controller = new HomeController(null, null, null);

            // Act
            var result = controller.AddProduct();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AddProduct_Post_RedirectsToCatalog()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(repo => repo.ListAsync())
                .ReturnsAsync(GetTestProducts());
            var controller = new HomeController(mockRepo.Object, null, null);

            var newName = "Новый";
            var newPrice = 310;
            var newProduct = new Product
            {
                Name = newName,
                Price = newPrice
            };
            mockRepo.Setup(repo => repo.AddAsync(newProduct))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await controller.AddProductAsync(newProduct);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Catalog", redirectToActionResult.ActionName);
            mockRepo.Verify();
            //var products = await mockRepo.Object.ListAsync();

            //Assert.Equal(4, products.Count);
            //Assert.Contains(products, p => p.Name == "Новый" && p.Price == newPrice);
        }

        //[Fact]
        //public void IndexViewResultNotNull()
        //{
        //    // Arrange
        //    HomeController controller = new HomeController();
        //    // Act
        //    ViewResult result = controller.Index() as ViewResult;
        //    // Assert
        //    Assert.NotNull(result);
        //}

        //[Fact]
        //public void IndexViewNameEqualIndex()
        //{
        //    // Arrange
        //    HomeController controller = new HomeController();
        //    // Act
        //    ViewResult result = controller.Index() as ViewResult;
        //    // Assert
        //    Assert.Equal("Index", result?.ViewName);
        //}
    }
}
