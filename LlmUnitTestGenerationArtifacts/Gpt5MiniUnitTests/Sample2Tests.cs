using Dataset.Sample2;
using NSubstitute;

namespace Gpt5MiniUnitTests
{
    public class EmployeeServiceTests
    {
        // Arrange helpers
        private static EmployeeEntity CreateEntity(int id = 1)
        {
            return new EmployeeEntity
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                MiddleName = "M",
                BirthDate = new DateTime(1980, 1, 1),
                Country = "USA",
                City = "NewYork",
                Address = "123 Street",
                Phone = "+12345678901",
                HireDate = DateTime.Now.AddDays(-1),
                Salary = 1000m,
                DepartmentId = 1,
                Department = "Dept",
                PositionId = 1,
                Position = "Pos"
            };
        }

        private static EmployeeModel CreateValidModel(int id = 0)
        {
            return new EmployeeModel
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                MiddleName = "M",
                BirthDate = new DateTime(1980, 1, 1),
                Country = "USA",
                City = "NewYork",
                Address = "123 Street",
                Phone = "+12345678901",
                HireDate = DateTime.Now.AddDays(-1),
                Salary = 1000m,
                DepartmentId = 1,
                Department = "Dept",
                PositionId = 1,
                Position = "Pos"
            };
        }

        [Fact]
        public async Task GetAsync_WhenEntityExists_ReturnsMappedModel()
        {
            // Arrange
            var repo = Substitute.For<IEmployeeRepository>();
            var entity = CreateEntity(5);
            repo.GetAsync(5).Returns(Task.FromResult(entity));
            var service = new EmployeeService(repo);

            // Act
            var result = await service.GetAsync(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.FirstName, result.FirstName);
            Assert.Equal(entity.LastName, result.LastName);
            Assert.Equal(entity.MiddleName, result.MiddleName);
            Assert.Equal(entity.BirthDate, result.BirthDate);
            Assert.Equal(entity.Country, result.Country);
            Assert.Equal(entity.City, result.City);
            Assert.Equal(entity.Address, result.Address);
            Assert.Equal(entity.Phone, result.Phone);
            Assert.Equal(entity.HireDate, result.HireDate);
            Assert.Equal(entity.Salary, result.Salary);
            Assert.Equal(entity.DepartmentId, result.DepartmentId);
            Assert.Equal(entity.Department, result.Department);
            Assert.Equal(entity.PositionId, result.PositionId);
            Assert.Equal(entity.Position, result.Position);
        }

        [Fact]
        public async Task ListAsync_WhenCalled_ReturnsMappedModels()
        {
            // Arrange
            var repo = Substitute.For<IEmployeeRepository>();
            var entities = new List<EmployeeEntity>
            {
                CreateEntity(1),
                CreateEntity(2)
            };
            repo.ListAsync().Returns(Task.FromResult((ICollection<EmployeeEntity>)entities));
            var service = new EmployeeService(repo);

            // Act
            var result = await service.ListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, m => m.Id == 1);
            Assert.Contains(result, m => m.Id == 2);
        }

        [Fact]
        public async Task ListFilteredAsync_NoFilters_CallsListAsync()
        {
            // Arrange
            var repo = Substitute.For<IEmployeeRepository>();
            var entities = new List<EmployeeEntity> { CreateEntity(1) };
            repo.ListAsync().Returns(Task.FromResult((ICollection<EmployeeEntity>)entities));
            var service = new EmployeeService(repo);

            // Act
            var result = await service.ListFilteredAsync(
                name: null,
                country: null,
                city: null,
                minSalary: 0,
                maxSalary: 0,
                departmentId: 0,
                positionId: 0);

            // Assert
            Assert.Single(result);
            await repo.Received(1).ListAsync();
            await repo.DidNotReceive().ListFilteredAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<decimal>(),
                Arg.Any<int>(),
                Arg.Any<int>());
        }

        [Fact]
        public async Task ListFilteredAsync_WithFilters_CallsRepositoryFiltered()
        {
            // Arrange
            var repo = Substitute.For<IEmployeeRepository>();
            var entities = new List<EmployeeEntity> { CreateEntity(2) };
            repo.ListFilteredAsync("n", "c", "t", 1m, 2m, 3, 4)
                .Returns(Task.FromResult((ICollection<EmployeeEntity>)entities));
            var service = new EmployeeService(repo);

            // Act
            var result = await service.ListFilteredAsync("n", "c", "t", 1m, 2m, 3, 4);

            // Assert
            Assert.Single(result);
            await repo.Received(1).ListFilteredAsync("n", "c", "t", 1m, 2m, 3, 4);
            await repo.DidNotReceive().ListAsync();
        }

        [Fact]
        public async Task AddAsync_ValidModel_TrimsAndNormalizesAndCallsRepository()
        {
            // Arrange
            var repo = Substitute.For<IEmployeeRepository>();
            repo.AddAsync(Arg.Any<EmployeeEntity>()).Returns(42);
            var service = new EmployeeService(repo);

            var model = CreateValidModel();
            model.FirstName = " John ";
            model.LastName = " Doe ";
            model.MiddleName = " M ";
            model.Country = " "; // should become null
            model.City = "   ";  // should become null
            model.Phone = "12345678901"; // should get '+' prefix
            model.Address = " 123 Street "; // trimmed

            // Act
            var result = await service.AddAsync(model);

            // Assert
            Assert.Equal(42, result);
            await repo.Received(1).AddAsync(Arg.Is<EmployeeEntity>(e =>
                e.FirstName == "John" &&
                e.LastName == "Doe" &&
                e.MiddleName == "M" &&
                e.Country == null &&
                e.City == null &&
                e.Address == "123 Street" &&
                e.Phone == "+12345678901" &&
                e.Salary == model.Salary &&
                e.DepartmentId == model.DepartmentId &&
                e.PositionId == model.PositionId));
        }

        [Fact]
        public async Task AddAsync_NullModel_ThrowsArgumentNullException()
        {
            // Arrange
            var repo = Substitute.For<IEmployeeRepository>();
            var service = new EmployeeService(repo);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(null!));
            await repo.DidNotReceive().AddAsync(Arg.Any<EmployeeEntity>());
        }

        [Fact]
        public async Task AddAsync_WhiteSpaceFirstName_ThrowsArgumentExceptionWithParamNameFirstName()
        {
            // Arrange
            var repo = Substitute.For<IEmployeeRepository>();
            var service = new EmployeeService(repo);
            var model = CreateValidModel();
            model.FirstName = "   ";

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(model));

            // Assert
            Assert.Equal("FirstName", ex.ParamName);
            await repo.DidNotReceive().AddAsync(Arg.Any<EmployeeEntity>());
        }

        [Fact]
        public async Task AddAsync_AddressTooLong_ThrowsArgumentExceptionWithParamNameCity()
        {
            // Arrange
            var repo = Substitute.For<IEmployeeRepository>();
            var service = new EmployeeService(repo);
            var model = CreateValidModel();
            model.Address = new string('a', 51);

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(model));

            // Assert: production code incorrectly uses nameof(employeeModel.City) for this error
            Assert.Equal("City", ex.ParamName);
            await repo.DidNotReceive().AddAsync(Arg.Any<EmployeeEntity>());
        }

        [Fact]
        public async Task UpdateAsync_InvalidId_ThrowsArgumentExceptionWithParamNameId()
        {
            // Arrange
            var repo = Substitute.For<IEmployeeRepository>();
            var service = new EmployeeService(repo);
            var model = CreateValidModel(id: 0);

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(model));

            // Assert
            Assert.Equal("Id", ex.ParamName);
            await repo.DidNotReceive().UpdateAsync(Arg.Any<EmployeeEntity>());
        }

        [Fact]
        public async Task UpdateAsync_ValidModel_CallsRepositoryUpdateAndReturnsResult()
        {
            // Arrange
            var repo = Substitute.For<IEmployeeRepository>();
            repo.UpdateAsync(Arg.Any<EmployeeEntity>()).Returns(7);
            var service = new EmployeeService(repo);
            var model = CreateValidModel(id: 5);

            // Act
            var result = await service.UpdateAsync(model);

            // Assert
            Assert.Equal(7, result);
            await repo.Received(1).UpdateAsync(Arg.Is<EmployeeEntity>(e => e.Id == 5 && e.FirstName == model.FirstName));
        }

        [Fact]
        public async Task UpdateAsync_NullModel_ThrowsArgumentNullException()
        {
            // Arrange
            var repo = Substitute.For<IEmployeeRepository>();
            var service = new EmployeeService(repo);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(null!));
            await repo.DidNotReceive().UpdateAsync(Arg.Any<EmployeeEntity>());
        }

        [Fact]
        public async Task DeleteAsync_ForwardsToRepositoryAndReturnsResult()
        {
            // Arrange
            var repo = Substitute.For<IEmployeeRepository>();
            repo.DeleteAsync(10).Returns(1);
            var service = new EmployeeService(repo);

            // Act
            var result = await service.DeleteAsync(10);

            // Assert
            Assert.Equal(1, result);
            await repo.Received(1).DeleteAsync(10);
        }
    }
}
