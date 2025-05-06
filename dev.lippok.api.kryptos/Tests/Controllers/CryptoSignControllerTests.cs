using System.Text;
using dev.lippok.api.kryptos.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace dev.lippok.api.kryptos.Tests.Controllers
{
    public class CryptoSignControllerTests
    {
        private readonly Mock<ILogger<CryptoSignController>> _mockLogger;
        private readonly CryptoSignController _controller;

        public CryptoSignControllerTests()
        {
            _mockLogger = new Mock<ILogger<CryptoSignController>>();
            _controller = new CryptoSignController(_mockLogger.Object);
        }

        [Fact]
        public void SignString_ValidInput_ReturnsSignature()
        {
            // Arrange
            var input = "Test string";

            // Act
            var result = _controller.SignString(input) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var response = result.Value as dynamic;
            Assert.NotNull(response.Signature);
        }

        [Fact]
        public void SignString_NullOrEmptyInput_ReturnsBadRequest()
        {
            // Act
            var result = _controller.SignString(null) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Input string cannot be null or empty.", result.Value);
        }

        [Fact]
        public void GetPublicKey_ReturnsPublicKey()
        {
            // Act
            var result = _controller.GetPublicKey() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var response = result.Value as dynamic;
            Assert.NotNull(response.PublicKey);
        }

        [Fact]
        public void VerifySignature_ValidSignature_ReturnsTrue()
        {
            // Arrange
            var input = "Test string";
            var data = Encoding.UTF8.GetBytes(input);
            var signature = _controller.SignString(input) as OkObjectResult;
            var base64Signature = (signature.Value as dynamic).Signature;

            var request = new VerifyRequest
            {
                Input = input,
                Signature = base64Signature
            };

            // Act
            var result = _controller.VerifySignature(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var response = result.Value as dynamic;
            Assert.True(response.IsValid);
        }

        [Fact]
        public void VerifySignature_InvalidSignature_ReturnsFalse()
        {
            // Arrange
            var request = new VerifyRequest
            {
                Input = "Test string",
                Signature = Convert.ToBase64String(new byte[] { 0x00, 0x01, 0x02 })
            };

            // Act
            var result = _controller.VerifySignature(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var response = result.Value as dynamic;
            Assert.False(response.IsValid);
        }

        [Fact]
        public void VerifySignature_NullOrEmptyInput_ReturnsBadRequest()
        {
            // Arrange
            var request = new VerifyRequest
            {
                Input = null,
                Signature = null
            };

            // Act
            var result = _controller.VerifySignature(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Input string and signature cannot be null or empty.", result.Value);
        }
    }
}