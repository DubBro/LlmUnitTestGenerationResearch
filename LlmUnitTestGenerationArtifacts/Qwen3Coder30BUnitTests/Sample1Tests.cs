using Dataset.Sample1;
using NSubstitute;

namespace Qwen3Coder30BUnitTests
{
    public class DepartmentServiceTests
    {
        [Fact]
        public async Task GetAsync_ValidId_ReturnsDepartmentModel()
        {
            // Arrange
            var repository = Substitute.For<IDepartmentRepository>();
            var entity = new DepartmentEntity { Id = 1, Name = "HR", Description = "Human Resources" };
            repository.GetAsync(1).Returns(entity);

            var service = new DepartmentService(repository);

            // Act
            var result = await service.GetAsync(1);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal("HR", result.Name);
            Assert.Equal("Human Resources", result.Description);
        }

        [Fact]
        public async Task ListAsync_Called_ReturnsCollectionOfDepartmentModels()
        {
            // Arrange
            var repository = Substitute.For<IDepartmentRepository>();
            var entities = new List<DepartmentEntity>
            {
                new DepartmentEntity { Id = 1, Name = "HR", Description = "Human Resources" },
                new DepartmentEntity { Id = 2, Name = "IT", Description = "Information Technology" }
            };
            repository.ListAsync().Returns(entities);

            var service = new DepartmentService(repository);

            // Act
            var result = await service.ListAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result.ElementAt(0).Id);
            Assert.Equal("HR", result.ElementAt(0).Name);
            Assert.Equal("Human Resources", result.ElementAt(0).Description);
            Assert.Equal(2, result.ElementAt(1).Id);
            Assert.Equal("IT", result.ElementAt(1).Name);
            Assert.Equal("Information Technology", result.ElementAt(1).Description);
        }
    }
}
