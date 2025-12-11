using Dataset.Sample1;
using NSubstitute;

namespace Gpt5MiniUnitTests
{
    public class DepartmentServiceTests
    {
        [Fact]
        public async Task GetAsync_ExistingId_ReturnsMappedDepartmentModel()
        {
            // Arrange
            var repository = Substitute.For<IDepartmentRepository>();
            var entity = new DepartmentEntity
            {
                Id = 5,
                Name = "HR",
                Description = "Human Resources"
            };
            repository.GetAsync(Arg.Is(5)).Returns(Task.FromResult(entity));
            var service = new DepartmentService(repository);

            // Act
            var result = await service.GetAsync(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.Description, result.Description);
        }

        [Fact]
        public async Task GetAsync_RepositoryReturnsNull_ThrowsNullReferenceException()
        {
            // Arrange
            var repository = Substitute.For<IDepartmentRepository>();
            repository.GetAsync(Arg.Any<int>()).Returns(Task.FromResult<DepartmentEntity?>(null));
            var service = new DepartmentService(repository);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetAsync(1));
        }

        [Fact]
        public async Task ListAsync_WhenCalled_ReturnsMappedCollection()
        {
            // Arrange
            var repository = Substitute.For<IDepartmentRepository>();
            var entities = new List<DepartmentEntity>
            {
                new DepartmentEntity { Id = 1, Name = "A", Description = "Desc A" },
                new DepartmentEntity { Id = 2, Name = "B", Description = null }
            };
            repository.ListAsync().Returns(Task.FromResult<ICollection<DepartmentEntity>>(entities));
            var service = new DepartmentService(repository);

            // Act
            var result = await service.ListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            var list = result.ToList();
            Assert.Equal(entities[0].Id, list[0].Id);
            Assert.Equal(entities[0].Name, list[0].Name);
            Assert.Equal(entities[0].Description, list[0].Description);
            Assert.Equal(entities[1].Id, list[1].Id);
            Assert.Equal(entities[1].Name, list[1].Name);
            Assert.Equal(entities[1].Description, list[1].Description);
        }

        [Fact]
        public async Task ListAsync_RepositoryReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var repository = Substitute.For<IDepartmentRepository>();
            repository.ListAsync().Returns(Task.FromResult<ICollection<DepartmentEntity>>(new List<DepartmentEntity>()));
            var service = new DepartmentService(repository);

            // Act
            var result = await service.ListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ListAsync_RepositoryReturnsListContainingNull_ThrowsNullReferenceException()
        {
            // Arrange
            var repository = Substitute.For<IDepartmentRepository>();
            var entities = new List<DepartmentEntity?> { new DepartmentEntity { Id = 1, Name = "X", Description = "d" }, null! };
            repository.ListAsync().Returns(Task.FromResult<ICollection<DepartmentEntity>>(entities.Cast<DepartmentEntity>().ToList()));
            var service = new DepartmentService(repository);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.ListAsync());
        }
    }
}
