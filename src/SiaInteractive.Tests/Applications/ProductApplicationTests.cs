using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using SiaInteractive.Application.Core;
using SiaInteractive.Application.Dtos.Products;
using SiaInteractive.Domain.Entities;
using SiaInteractive.Infraestructure.Interfaces;

namespace SiaInteractive.Tests.Applications
{
    [TestClass]
    public class ProductApplicationTests
    {
        private Mock<IProductRepository> _productRepository = new();
        private Mock<ICategoryRepository> _categoryRepository = new();
        private Mock<IValidator<CreateProductDto>> _createValidator = new();
        private Mock<IValidator<UpdateProductDto>> _updateValidator = new();
        private Mock<IMapper> _mapper = new();
        private Mock<ILogger<ProductApplication>> _logger = new();

        private ProductApplication CreateInstance() 
        {
            return new ProductApplication(_productRepository.Object,
                _categoryRepository.Object,
                _createValidator.Object,
                _updateValidator.Object,
                _mapper.Object,
                _logger.Object);
        } 

        [TestInitialize]
        public void Setup()
        {
            _productRepository = new Mock<IProductRepository>(MockBehavior.Strict);
            _categoryRepository = new Mock<ICategoryRepository>(MockBehavior.Strict);
            _createValidator = new Mock<IValidator<CreateProductDto>>(MockBehavior.Strict);
            _updateValidator = new Mock<IValidator<UpdateProductDto>>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _logger = new Mock<ILogger<ProductApplication>>(MockBehavior.Loose);
        }

        [TestMethod]
        public async Task GetAsync_WhenProductExists_ReturnsSuccess()
        {
            // Arrange
            var instance = CreateInstance();

            var product = new Product { ProductID = 10, Name = "P1" };
            var dto = new ProductDto { Id = 10, Name = "P1" };

            _productRepository.Setup(r => r.GetAsync(10)).ReturnsAsync(product);
            _mapper.Setup(m => m.Map<ProductDto>(product)).Returns(dto);

            // Act
            var res = await instance.GetAsync(10);

            // Assert
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual("Product retrieved successfully.", res.Message);
            Assert.IsNotNull(res.Data);
            Assert.AreEqual(10, res.Data!.Id);

            _productRepository.Verify(r => r.GetAsync(10), Times.Once);
            _mapper.Verify(m => m.Map<ProductDto>(product), Times.Once);

            _productRepository.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetAsync_WhenProductNotFound_ReturnsNotFoundMessage()
        {
            // Arrange
            var instance = CreateInstance();

            _productRepository.Setup(r => r.GetAsync(10)).ReturnsAsync((Product?)null);
            _mapper.Setup(m => m.Map<ProductDto>(null)).Returns((ProductDto?)null);

            // Act
            var res = await instance.GetAsync(10);

            // Assert
            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual("Product not found.", res.Message);
            Assert.IsNull(res.Data);

            _productRepository.Verify(r => r.GetAsync(10), Times.Once);
            _mapper.Verify(m => m.Map<ProductDto>(null), Times.Once);

            _productRepository.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetAllAsync_WhenRepoReturnsList_ReturnsSuccess()
        {
            // Arrange
            var instance = CreateInstance();

            var products = new[] { new Product { ProductID = 1, Name = "A" } };
            var dtos = new[] { new ProductDto { Id = 1, Name = "A" } };

            _productRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
            _mapper.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(dtos);

            // Act
            var res = await instance.GetAllAsync();

            // Assert
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual("Products retrieved successfully.", res.Message);
            Assert.IsNotNull(res.Data);

            _productRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mapper.Verify(m => m.Map<IEnumerable<ProductDto>>(products), Times.Once);

            _productRepository.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetAllAsync_WhenRepoReturnsNull_ReturnsNoProductsFound()
        {
            // Arrange
            var instance = CreateInstance();

            _productRepository.Setup(r => r.GetAllAsync()).ReturnsAsync((IEnumerable<Product>?)null);
            _mapper.Setup(m => m.Map<IEnumerable<ProductDto>>(null)).Returns((IEnumerable<ProductDto>?)null);

            // Act
            var res = await instance.GetAllAsync();

            // Assert
            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual("No products found.", res.Message);
            Assert.IsNull(res.Data);

            _productRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mapper.Verify(m => m.Map<IEnumerable<ProductDto>>(null), Times.Once);

            _productRepository.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetAllWithPaginationAsync_WhenRepoReturnsList_ReturnsPaginationFields()
        {
            // Arrange
            var instance = CreateInstance();

            var products = new[] { new Product { ProductID = 1 }, new Product { ProductID = 2 } };
            var dtos = new[] { new ProductDto { Id = 1 }, new ProductDto { Id = 2 } };

            _productRepository.Setup(r => r.Count()).ReturnsAsync(5);
            _productRepository.Setup(r => r.GetAllWithPaginationAsync(2, 2)).ReturnsAsync(products);
            _mapper.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(dtos);

            // Act
            var res = await instance.GetAllWithPaginationAsync(2, 2);

            // Assert
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual("Products retrieved successfully.", res.Message);
            Assert.AreEqual(2, res.PageNumber);
            Assert.AreEqual(5, res.TotalRecords);
            Assert.AreEqual(3, res.TotalPages); // ceil(5/2)=3
            Assert.IsNotNull(res.Data);

            _productRepository.Verify(r => r.Count(), Times.Once);
            _productRepository.Verify(r => r.GetAllWithPaginationAsync(2, 2), Times.Once);
            _mapper.Verify(m => m.Map<IEnumerable<ProductDto>>(products), Times.Once);

            _productRepository.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetAllWithPaginationAsync_WhenRepoReturnsNull_ReturnsNoProductsFound()
        {
            // Arrange
            var instance = CreateInstance();

            _productRepository.Setup(r => r.Count()).ReturnsAsync(5);
            _productRepository.Setup(r => r.GetAllWithPaginationAsync(1, 10)).ReturnsAsync((IEnumerable<Product>?)null);
            _mapper.Setup(m => m.Map<IEnumerable<ProductDto>>(null)).Returns((IEnumerable<ProductDto>?)null);

            // Act
            var res = await instance.GetAllWithPaginationAsync(1, 10);

            // Assert
            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual("No products found.", res.Message);
            Assert.IsNull(res.Data);

            _productRepository.Verify(r => r.Count(), Times.Once);
            _productRepository.Verify(r => r.GetAllWithPaginationAsync(1, 10), Times.Once);
            _mapper.Verify(m => m.Map<IEnumerable<ProductDto>>(null), Times.Once);

            _productRepository.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task InsertAsync_WhenValidationFails_ReturnsValidationErrors_AndDoesNotInsert()
        {
            // Arrange
            var instance = CreateInstance();

            var dto = new CreateProductDto
            {
                Name = "",
                CategoryIds = [1, 2]
            };

            var failures = new[]
            {
                new ValidationFailure("Name", "Name is required")
            };

            _createValidator.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(failures));

            // Act
            var res = await instance.InsertAsync(dto);

            // Assert
            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual("Validation errors occurred.", res.Message);
            Assert.IsNotNull(res.ValidationErrors);

            _productRepository.Verify(r => r.InsertAsync(It.IsAny<Product>()), Times.Never);
            _categoryRepository.Verify(r => r.GetTrackingAsync(It.IsAny<List<int>>()), Times.Never);

            _createValidator.Verify(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
            _createValidator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task InsertAsync_WhenValid_InsertsProductWithCategories()
        {
            // Arrange
            var instance = CreateInstance();

            var dto = new CreateProductDto
            {
                Name = "Product",
                CategoryIds = [10, 20]
            };

            _createValidator
                .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var mappedProduct = new Product
            {
                Name = "Product",
                Categories = new List<Category>()
            };

            _mapper.Setup(m => m.Map<Product>(dto)).Returns(mappedProduct);

            var categories = new List<Category>
            {
                new Category { CategoryID = 10, Name = "C10" },
                new Category { CategoryID = 20, Name = "C20" }
            };

            _categoryRepository.Setup(r => r.GetTrackingAsync(dto.CategoryIds!)).ReturnsAsync(categories);
            _productRepository.Setup(r => r.InsertAsync(mappedProduct)).ReturnsAsync(true);

            // Act
            var res = await instance.InsertAsync(dto);

            // Assert
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual("Product inserted successfully.", res.Message);
            Assert.IsTrue(res.Data);
            Assert.AreEqual(2, mappedProduct.Categories.Count);

            _createValidator.Verify(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
            _mapper.Verify(m => m.Map<Product>(dto), Times.Once);
            _categoryRepository.Verify(r => r.GetTrackingAsync(dto.CategoryIds!), Times.Once);
            _productRepository.Verify(r => r.InsertAsync(mappedProduct), Times.Once);

            _productRepository.VerifyNoOtherCalls();
            _categoryRepository.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
            _createValidator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task UpdateAsync_WhenValidationFails_ReturnsValidationErrors_AndDoesNotUpdate()
        {
            // Arrange
            var instance = CreateInstance();

            var dto = new UpdateProductDto
            {
                Id = 5,
                Name = "",
                CategoryIds = [1]
            };

            var failures = new[]
            {
                new ValidationFailure("Name", "Name is required")
            };

            _updateValidator.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(failures));

            // Act
            var res = await instance.UpdateAsync(dto);

            // Assert
            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual("Validation errors occurred.", res.Message);

            _productRepository.Verify(r => r.GetTrackingAsync(It.IsAny<int>()), Times.Never);
            _productRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);

            _updateValidator.Verify(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
            _updateValidator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task UpdateAsync_WhenValid_UpdatesAndSyncsCategories()
        {
            // Arrange
            var instance = CreateInstance();

            var savedProduct = new Product
            {
                ProductID = 5,
                Name = "Old",
                Categories = [ new Category { CategoryID = 1, Name = "C1" },
                    new Category { CategoryID = 2, Name = "C2" }
                ]
            };

            var dto = new UpdateProductDto
            {
                Id = 5,
                Name = "New",
                CategoryIds = [2,3]
            };

            var updatedProduct = new Product
            {
                ProductID = savedProduct.ProductID,
                Name = dto.Name,
                Categories = savedProduct.Categories
            };

            _updateValidator
                .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _productRepository.Setup(r => r.GetTrackingAsync(5)).ReturnsAsync(savedProduct);

            _mapper.Setup(m => m.Map(dto, savedProduct)).Returns(updatedProduct);

            var requestedCategories = new List<Category>
            {
                new Category { CategoryID = 2, Name = "C2" },
                new Category { CategoryID = 3, Name = "C3" }
            };

            _categoryRepository.Setup(r => r.GetTrackingAsync(dto.CategoryIds!)).ReturnsAsync(requestedCategories);
            _productRepository.Setup(r => r.UpdateAsync(updatedProduct)).ReturnsAsync(true);

            // Act
            var res = await instance.UpdateAsync(dto);

            // Assert
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual("Product updated successfully.", res.Message);
            Assert.IsTrue(res.Data);
            Assert.AreEqual("New", updatedProduct.Name);

            _updateValidator.Verify(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
            _productRepository.Verify(r => r.GetTrackingAsync(5), Times.Once);
            _mapper.Verify(m => m.Map(dto, savedProduct), Times.Once);
            _categoryRepository.Verify(r => r.GetTrackingAsync(dto.CategoryIds!), Times.Once);
            _productRepository.Verify(r => r.UpdateAsync(updatedProduct), Times.Once);

            _productRepository.VerifyNoOtherCalls();
            _categoryRepository.VerifyNoOtherCalls();
            _mapper.VerifyNoOtherCalls();
            _updateValidator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task DeleteAsync_WhenRepoReturnsTrue_ReturnsSuccess_AndLogs()
        {
            // Arrange
            var instance = CreateInstance();

            _productRepository.Setup(r => r.DeleteAsync(7)).ReturnsAsync(true);

            // Act
            var res = await instance.DeleteAsync(7);

            // Assert
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual("Product deleted successfully.", res.Message);
            Assert.IsTrue(res.Data);

            _productRepository.Verify(r => r.DeleteAsync(7), Times.Once);

            _logger.VerifyLog(LogLevel.Information, Times.Once());
        }

        [TestMethod]
        public async Task DeleteAsync_WhenRepoReturnsFalse_ReturnsFail_AndDoesNotLogInfo()
        {
            // Arrange
            var instance = CreateInstance();

            _productRepository.Setup(r => r.DeleteAsync(7)).ReturnsAsync(false);

            // Act
            var res = await instance.DeleteAsync(7);

            // Assert
            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual("Failed to delete product.", res.Message);
            Assert.IsFalse(res.Data);

            _productRepository.Verify(r => r.DeleteAsync(7), Times.Once);

            _logger.VerifyLog(LogLevel.Information, Times.Never());
        }
    }

    internal static class LoggerMoqExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level, Times times)
        {
            logger.Verify(x => x.Log(level, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), 
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), times);
        }
    }
}
