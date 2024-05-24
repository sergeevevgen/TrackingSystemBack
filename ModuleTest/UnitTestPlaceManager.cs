using Moq;
using NLog;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrackingSystem.Api.BusinessLogic.Managers;
using TrackingSystem.Api.Shared.Dto.Place;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;

namespace ModuleTest
{
    [TestFixture]
    public class UnitTestPlaceManager
    {
        private Mock<ILogger> _mockLogger;
        private Mock<IPlaceDbManager> _mockPlaceRepository;
        private IPlaceManager _placeManager;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger>();
            _mockPlaceRepository = new Mock<IPlaceDbManager>();
            _placeManager = new PlaceManager(_mockLogger.Object, _mockPlaceRepository.Object);
        }

        [Test]
        public async Task CreateOrUpdate_ShouldCreateNewPlace_WhenPlaceDoesNotExist()
        {
            // Arrange
            var PlaceDto = new PlaceDto { Name = "3-414" };
            var PlaceResponseDto = new PlaceResponseDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691"), Name = "3-414" };
            _mockPlaceRepository.Setup(s => s.GetElement(It.IsAny<PlaceDto>(), It.IsAny<CancellationToken>())).ReturnsAsync((PlaceResponseDto)null);
            _mockPlaceRepository.Setup(s => s.Insert(It.IsAny<PlaceDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(PlaceResponseDto);

            // Act
            var result = await _placeManager.CreateOrUpdate(PlaceDto);

            bool flag = false;

            if (result != null)
                flag = true;

            // Assert
            ClassicAssert.That(flag);
            ClassicAssert.AreEqual(new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691"), result.Id);
            _mockPlaceRepository.Verify(s => s.GetElement(It.IsAny<PlaceDto>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockPlaceRepository.Verify(s => s.Insert(It.IsAny<PlaceDto>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockLogger.Verify(l => l.Info(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void CreateOrUpdate_ShouldThrowException_WhenPlaceWithSameNameExists()
        {
            // Arrange
            var PlaceDto = new PlaceDto { Name = "3-414" };
            var existingPlace = new PlaceResponseDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691"), Name = "3-414" };
            _mockPlaceRepository.Setup(s => s.GetElement(It.IsAny<PlaceDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingPlace);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _placeManager.CreateOrUpdate(PlaceDto));
            ClassicAssert.AreEqual("Уже есть помещение с таким названием", ex.Message);
            _mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Delete_ShouldReturnTrue_WhenPlaceIsDeleted()
        {
            // Arrange
            var PlaceDto = new PlaceDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691") };
            var PlaceResponseDto = new PlaceResponseDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691"), Name = "3-414" };
            _mockPlaceRepository.Setup(s => s.GetElement(It.IsAny<PlaceDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(PlaceResponseDto);
            _mockPlaceRepository.Setup(s => s.Delete(It.IsAny<PlaceDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _placeManager.Delete(PlaceDto);

            // Assert
            ClassicAssert.IsTrue(result);
            _mockPlaceRepository.Verify(s => s.GetElement(It.IsAny<PlaceDto>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockPlaceRepository.Verify(s => s.Delete(It.IsAny<PlaceDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Delete_ShouldThrowException_WhenPlaceNotFound()
        {
            // Arrange
            var PlaceDto = new PlaceDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691") };
            _mockPlaceRepository.Setup(s => s.GetElement(It.IsAny<PlaceDto>(), It.IsAny<CancellationToken>())).ReturnsAsync((PlaceResponseDto)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _placeManager.Delete(PlaceDto));
            ClassicAssert.AreEqual("Элемент с идентификатором 3cabc7f5-5ccf-4e9a-914d-80edb0687691 не найден", ex.Message);
        }

        [Test]
        public async Task Read_ShouldReturnPlace_WhenPlaceExists()
        {
            // Arrange
            var PlaceDto = new PlaceDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691") };
            var PlaceResponseDto = new PlaceResponseDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691"), Name = "3-414" };
            _mockPlaceRepository.Setup(s => s.GetElement(It.IsAny<PlaceDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(PlaceResponseDto);

            // Act
            var result = await _placeManager.Read(PlaceDto);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNull(result.ErrorMessage);
            ClassicAssert.AreEqual(PlaceResponseDto, result.Data);
        }

        [Test]
        public async Task Read_ShouldReturnError_WhenExceptionThrown()
        {
            // Arrange
            var PlaceDto = new PlaceDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691") };
            _mockPlaceRepository.Setup(s => s.GetElement(It.IsAny<PlaceDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _placeManager.Read(PlaceDto);

            // Assert
            ClassicAssert.IsNull(result.Data);
            ClassicAssert.AreEqual("Some error", result.ErrorMessage);
        }
    }
}
