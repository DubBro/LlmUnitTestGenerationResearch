using Dataset.Sample2;
using NSubstitute;

namespace DeepSeekR10528UnitTests;

public class EmployeeServiceTests
{
    private readonly IEmployeeRepository _employeeRepositorySubstitute;
    private readonly EmployeeService _employeeService;

    public EmployeeServiceTests()
    {
        _employeeRepositorySubstitute = Substitute.For<IEmployeeRepository>();
        _employeeService = new EmployeeService(_employeeRepositorySubstitute);
    }

    [Fact]
    public async Task GetAsync_ExistingId_ReturnsMappedEmployeeModel()
    {
        // Arrange
        var id = 1;
        var employeeEntity = new EmployeeEntity
        {
            Id = id,
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "M",
            BirthDate = new DateTime(1980, 1, 1),
            Country = "USA",
            City = "New York",
            Address = "123 Main St",
            Phone = "+1234567890",
            HireDate = new DateTime(2020, 1, 1),
            Salary = 50000,
            DepartmentId = 1,
            Department = "IT",
            PositionId = 1,
            Position = "Developer"
        };
        _employeeRepositorySubstitute.GetAsync(id).Returns(employeeEntity);

        // Act
        var result = await _employeeService.GetAsync(id);

        // Assert
        Assert.Equal(employeeEntity.Id, result.Id);
        Assert.Equal(employeeEntity.FirstName, result.FirstName);
        Assert.Equal(employeeEntity.LastName, result.LastName);
        Assert.Equal(employeeEntity.MiddleName, result.MiddleName);
        Assert.Equal(employeeEntity.BirthDate, result.BirthDate);
        Assert.Equal(employeeEntity.Country, result.Country);
        Assert.Equal(employeeEntity.City, result.City);
        Assert.Equal(employeeEntity.Address, result.Address);
        Assert.Equal(employeeEntity.Phone, result.Phone);
        Assert.Equal(employeeEntity.HireDate, result.HireDate);
        Assert.Equal(employeeEntity.Salary, result.Salary);
        Assert.Equal(employeeEntity.DepartmentId, result.DepartmentId);
        Assert.Equal(employeeEntity.Department, result.Department);
        Assert.Equal(employeeEntity.PositionId, result.PositionId);
        Assert.Equal(employeeEntity.Position, result.Position);
    }

    [Fact]
    public async Task GetAsync_RepositoryReturnsNull_ThrowsNullReferenceException()
    {
        // Arrange
        var id = 1;
        _employeeRepositorySubstitute.GetAsync(id).Returns((EmployeeEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _employeeService.GetAsync(id));
    }

    [Fact]
    public async Task ListAsync_WhenCalled_ReturnsMappedEmployeeModels()
    {
        // Arrange
        var employeeEntities = new List<EmployeeEntity>
        {
            new EmployeeEntity { Id = 1, FirstName = "John" },
            new EmployeeEntity { Id = 2, FirstName = "Jane" }
        };
        _employeeRepositorySubstitute.ListAsync().Returns(employeeEntities);

        // Act
        var result = await _employeeService.ListAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("John", result.First().FirstName);
        Assert.Equal("Jane", result.Last().FirstName);
    }

    [Fact]
    public async Task ListFilteredAsync_AllFiltersDefault_CallsListAsync()
    {
        // Arrange
        var employeeEntities = new List<EmployeeEntity>
        {
            new EmployeeEntity { Id = 1, FirstName = "John" }
        };
        _employeeRepositorySubstitute.ListAsync().Returns(employeeEntities);

        // Act
        var result = await _employeeService.ListFilteredAsync(
            "", "", "", 0, 0, 0, 0);

        // Assert
        await _employeeRepositorySubstitute.Received(1).ListAsync();
        await _employeeRepositorySubstitute.DidNotReceive().ListFilteredAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<decimal>(), Arg.Any<int>(), Arg.Any<int>());
        Assert.Single(result);
    }

    [Fact]
    public async Task ListFilteredAsync_SomeFiltersProvided_CallsRepositoryListFilteredAsync()
    {
        // Arrange
        var employeeEntities = new List<EmployeeEntity>
        {
            new EmployeeEntity { Id = 1, FirstName = "John" }
        };
        _employeeRepositorySubstitute.ListFilteredAsync(
            "John", "USA", "NY", 50000, 100000, 1, 1).Returns(employeeEntities);

        // Act
        var result = await _employeeService.ListFilteredAsync(
            "John", "USA", "NY", 50000, 100000, 1, 1);

        // Assert
        await _employeeRepositorySubstitute.Received(1).ListFilteredAsync(
            "John", "USA", "NY", 50000, 100000, 1, 1);
        Assert.Single(result);
    }

    [Fact]
    public async Task AddAsync_ValidModel_ReturnsIdAfterValidationAndMapping()
    {
        // Arrange
        var validModel = new EmployeeModel
        {
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "M",
            BirthDate = new DateTime(1980, 1, 1),
            HireDate = new DateTime(2020, 1, 1),
            Salary = 50000,
            DepartmentId = 1,
            PositionId = 1
        };
        _employeeRepositorySubstitute.AddAsync(Arg.Any<EmployeeEntity>()).Returns(5);

        // Act
        var result = await _employeeService.AddAsync(validModel);

        // Assert
        Assert.Equal(5, result);
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData("", null, null)]
    [InlineData("   ", null, null)]
    [InlineData("John123", null, nameof(EmployeeModel.FirstName))]
    [InlineData("VeryLongNameVeryLongNameVeryLongNameVeryLongNameVeryLongNameVeryLong", null, nameof(EmployeeModel.FirstName))]
    public async Task AddAsync_InvalidFirstName_ThrowsArgumentException(string firstName, string dummy, string expectedParamName)
    {
        // Arrange
        var invalidModel = new EmployeeModel
        {
            FirstName = firstName,
            LastName = "Doe",
            MiddleName = "M",
            BirthDate = new DateTime(1980, 1, 1),
            HireDate = new DateTime(2020, 1, 1),
            Salary = 50000,
            DepartmentId = 1,
            PositionId = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _employeeService.AddAsync(invalidModel));
        Assert.Equal(expectedParamName ?? nameof(EmployeeModel.FirstName), exception.ParamName);
    }

    [Fact]
    public async Task AddAsync_InvalidCountry_ThrowsArgumentException()
    {
        // Arrange
        var invalidModel = new EmployeeModel
        {
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "M",
            BirthDate = new DateTime(1980, 1, 1),
            Country = "123Invalid",
            HireDate = new DateTime(2020, 1, 1),
            Salary = 50000,
            DepartmentId = 1,
            PositionId = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _employeeService.AddAsync(invalidModel));
        Assert.Equal(nameof(EmployeeModel.Country), exception.ParamName);
    }

    [Fact]
    public async Task AddAsync_InvalidAddress_ThrowsArgumentExceptionWithCityParamName()
    {
        // Arrange
        var invalidModel = new EmployeeModel
        {
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "M",
            BirthDate = new DateTime(1980, 1, 1),
            Address = new string('a', 51),
            HireDate = new DateTime(2020, 1, 1),
            Salary = 50000,
            DepartmentId = 1,
            PositionId = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _employeeService.AddAsync(invalidModel));
        Assert.Equal("City", exception.ParamName);
    }

    [Theory]
    [InlineData("abc", nameof(EmployeeModel.Phone))]
    [InlineData("+12345", nameof(EmployeeModel.Phone))]
    [InlineData("+012345678901234", nameof(EmployeeModel.Phone))]
    public async Task AddAsync_InvalidPhone_ThrowsArgumentException(string phone, string expectedParamName)
    {
        // Arrange
        var invalidModel = new EmployeeModel
        {
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "M",
            BirthDate = new DateTime(1980, 1, 1),
            Phone = phone,
            HireDate = new DateTime(2020, 1, 1),
            Salary = 50000,
            DepartmentId = 1,
            PositionId = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _employeeService.AddAsync(invalidModel));
        Assert.Equal(expectedParamName, exception.ParamName);
    }

    [Fact]
    public async Task UpdateAsync_ValidModel_ReturnsId()
    {
        // Arrange
        var validModel = new EmployeeModel
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "M",
            BirthDate = new DateTime(1980, 1, 1),
            HireDate = new DateTime(2020, 1, 1),
            Salary = 50000,
            DepartmentId = 1,
            PositionId = 1
        };
        _employeeRepositorySubstitute.UpdateAsync(Arg.Any<EmployeeEntity>()).Returns(1);

        // Act
        var result = await _employeeService.UpdateAsync(validModel);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task UpdateAsync_ZeroId_ThrowsArgumentException()
    {
        // Arrange
        var invalidModel = new EmployeeModel
        {
            Id = 0,
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "M",
            BirthDate = new DateTime(1980, 1, 1),
            HireDate = new DateTime(2020, 1, 1),
            Salary = 50000,
            DepartmentId = 1,
            PositionId = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _employeeService.UpdateAsync(invalidModel));
        Assert.Equal("Id", exception.ParamName);
    }

    [Fact]
    public async Task UpdateAsync_NullModel_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _employeeService.UpdateAsync(null));
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_ReturnsId()
    {
        // Arrange
        var id = 1;
        _employeeRepositorySubstitute.DeleteAsync(id).Returns(1);

        // Act
        var result = await _employeeService.DeleteAsync(id);

        // Assert
        Assert.Equal(1, result);
    }
}
