using Dataset.Sample1;
using NSubstitute;

namespace DeepSeekR10528UnitTests
{
    public class DepartmentServiceTests
    {
        private readonly IDepartmentRepository _repositorySubstitute;
        private readonly DepartmentService _sut;

        public DepartmentServiceTests()
        {
            _repositorySubstitute = Substitute.For<IDepartmentRepository>();
            _sut = new DepartmentService(_repositorySubstitute);
        }

        [Fact]
        public async Task GetAsync_WhenIdExists_ReturnsMappedModel()
        {
            // Arrange
            var testId = 1;
            var mockEntity = new DepartmentEntity { Id = testId, Name = "Test Department", Description = "Test Description" };
            _repositorySubstitute.GetAsync(testId).Returns(Task.FromResult(mockEntity));

            // Act
            var result = await _sut.GetAsync(testId);

            // Assert
            Assert.Equal(mockEntity.Id, result.Id);
            Assert.Equal(mockEntity.Name, result.Name);
            Assert.Equal(mockEntity.Description, result.Description);
        }

        [Fact]
        public async Task GetAsync_WhenRepositoryReturnsNull_ThrowsNullReferenceException()
        {
            // Arrange
            var testId = 1;
            _repositorySubstitute.GetAsync(testId).Returns(Task.FromResult<DepartmentEntity>(null));

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _sut.GetAsync(testId));
        }

        [Fact]
        public async Task ListAsync_WhenRepositoryReturnsEmptyList_ReturnsEmptyCollection()
        {
            // Arrange
            var emptyList = new List<DepartmentEntity>();
            _repositorySubstitute.ListAsync().Returns(Task.FromResult<ICollection<DepartmentEntity>>(emptyList));

            // Act
            var result = await _sut.ListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ListAsync_WhenRepositoryReturnsNonEmptyList_ReturnsMappedCollection()
        {
            // Arrange
            var entities = new List<DepartmentEntity>
            {
                new DepartmentEntity { Id = 1, Name = "Dept1", Description = "Desc1" },
                new DepartmentEntity { Id = 2, Name = "Dept2", Description = "Desc2" }
            };
            _repositorySubstitute.ListAsync().Returns(Task.FromResult<ICollection<DepartmentEntity>>(entities));

            // Act
            var result = await _sut.ListAsync();

            // Assert
            Assert.Equal(entities.Count, result.Count);
            for (var i = 0; i < entities.Count; i++)
            {
                Assert.Equal(entities[i].Id, result.ElementAt(i).Id);
                Assert.Equal(entities[i].Name, result.ElementAt(i).Name);
                Assert.Equal(entities[i].Description, result.ElementAt(i).Description);
            }
        }
    }
}
