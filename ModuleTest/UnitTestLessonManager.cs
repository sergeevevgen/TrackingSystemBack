using Moq;
using NLog;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrackingSystem.Api.BusinessLogic.Managers;
using TrackingSystem.Api.Shared.Dto.Lesson;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;

namespace ModuleTest
{
    [TestFixture]
    public class UnitTestLessonManager
    {
        private Mock<ILogger> _mockLogger;
        private Mock<ILessonDbManager> _mockLessonRepository;
        private ILessonManager _lessonManager;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger>();
            _mockLessonRepository = new Mock<ILessonDbManager>();
            _lessonManager = new LessonManager(_mockLogger.Object, _mockLessonRepository.Object);
        }

        [Test]
        public async Task CreateOrUpdate_ShouldCreateNewLesson_WhenLessonDoesNotExist()
        {
            // Arrange
            var lessonDto = new LessonDto { Name = "Math" };
            var lessonResponseDto = new LessonResponseDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691"), Name = "Math" };
            _mockLessonRepository.Setup(s => s.GetElement(It.IsAny<LessonDto>(), It.IsAny<CancellationToken>())).ReturnsAsync((LessonResponseDto)null);
            _mockLessonRepository.Setup(s => s.Insert(It.IsAny<LessonDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(lessonResponseDto);

            // Act
            var result = await _lessonManager.CreateOrUpdate(lessonDto);
            
            bool flag = false;
            
            if (result != null)
                flag = true;

            // Assert
            ClassicAssert.That(flag);
            ClassicAssert.AreEqual(new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691"), result.Id);
            _mockLessonRepository.Verify(s => s.GetElement(It.IsAny<LessonDto>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockLessonRepository.Verify(s => s.Insert(It.IsAny<LessonDto>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockLogger.Verify(l => l.Info(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void CreateOrUpdate_ShouldThrowException_WhenLessonWithSameNameExists()
        {
            // Arrange
            var lessonDto = new LessonDto { Name = "Math" };
            var existingLesson = new LessonResponseDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691"), Name = "Math" };
            _mockLessonRepository.Setup(s => s.GetElement(It.IsAny<LessonDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingLesson);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _lessonManager.CreateOrUpdate(lessonDto));
            ClassicAssert.AreEqual("”же есть тип зан€ти€ с таким названием", ex.Message);
            _mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Delete_ShouldReturnTrue_WhenLessonIsDeleted()
        {
            // Arrange
            var lessonDto = new LessonDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691") };
            var lessonResponseDto = new LessonResponseDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691"), Name = "Math" };
            _mockLessonRepository.Setup(s => s.GetElement(It.IsAny<LessonDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(lessonResponseDto);
            _mockLessonRepository.Setup(s => s.Delete(It.IsAny<LessonDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _lessonManager.Delete(lessonDto);

            // Assert
            ClassicAssert.IsTrue(result);
            _mockLessonRepository.Verify(s => s.GetElement(It.IsAny<LessonDto>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockLessonRepository.Verify(s => s.Delete(It.IsAny<LessonDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Delete_ShouldThrowException_WhenLessonNotFound()
        {
            // Arrange
            var lessonDto = new LessonDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691") };
            _mockLessonRepository.Setup(s => s.GetElement(It.IsAny<LessonDto>(), It.IsAny<CancellationToken>())).ReturnsAsync((LessonResponseDto)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _lessonManager.Delete(lessonDto));
            ClassicAssert.AreEqual("Ёлемент с идентификатором 3cabc7f5-5ccf-4e9a-914d-80edb0687691 не найден", ex.Message);
        }

        [Test]
        public async Task Read_ShouldReturnLesson_WhenLessonExists()
        {
            // Arrange
            var lessonDto = new LessonDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691") };
            var lessonResponseDto = new LessonResponseDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691"), Name = "Math" };
            _mockLessonRepository.Setup(s => s.GetElement(It.IsAny<LessonDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(lessonResponseDto);

            // Act
            var result = await _lessonManager.Read(lessonDto);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNull(result.ErrorMessage);
            ClassicAssert.AreEqual(lessonResponseDto, result.Data);
        }

        [Test]
        public async Task Read_ShouldReturnError_WhenExceptionThrown()
        {
            // Arrange
            var lessonDto = new LessonDto { Id = new Guid("3cabc7f5-5ccf-4e9a-914d-80edb0687691") };
            _mockLessonRepository.Setup(s => s.GetElement(It.IsAny<LessonDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _lessonManager.Read(lessonDto);

            // Assert
            ClassicAssert.IsNull(result.Data);
            ClassicAssert.AreEqual("Some error", result.ErrorMessage);
        }
    }
}