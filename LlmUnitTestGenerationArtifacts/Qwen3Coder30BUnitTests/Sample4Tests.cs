using AutoMapper;
using Dataset.Sample4;
using NSubstitute;

namespace Qwen3Coder30BUnitTests;

public class CompanyServiceTests
{
    [Fact]
    public async Task GetAsync_ValidId_ReturnsMappedCompanyModel()
    {
        // Arrange
        var companyEntity = new CompanyEntity
        {
            Id = 1,
            Name = "Test Company",
            Description = "Test Description"
        };

        var companyRepository = Substitute.For<ICompanyRepository>();
        companyRepository.GetAsync(1).Returns(companyEntity);

        var mapper = Substitute.For<IMapper>();
        var expectedModel = new CompanyModel
        {
            Id = 1,
            Name = "Test Company",
            Description = "Test Description"
        };
        mapper.Map<CompanyModel>(companyEntity).Returns(expectedModel);

        var service = new CompanyService(companyRepository, mapper);

        // Act
        var result = await service.GetAsync(1);

        // Assert
        Assert.Equal(expectedModel.Id, result.Id);
        Assert.Equal(expectedModel.Name, result.Name);
        Assert.Equal(expectedModel.Description, result.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public async Task GetAsync_InvalidId_ReturnsCompanyModelWithDefaultValues(int id)
    {
        // Arrange
        var companyEntity = new CompanyEntity
        {
            Id = id,
            Name = string.Empty,
            Description = string.Empty
        };

        var companyRepository = Substitute.For<ICompanyRepository>();
        companyRepository.GetAsync(id).Returns(companyEntity);

        var mapper = Substitute.For<IMapper>();
        var expectedModel = new CompanyModel
        {
            Id = id,
            Name = string.Empty,
            Description = string.Empty
        };
        mapper.Map<CompanyModel>(companyEntity).Returns(expectedModel);

        var service = new CompanyService(companyRepository, mapper);

        // Act
        var result = await service.GetAsync(id);

        // Assert
        Assert.Equal(expectedModel.Id, result.Id);
        Assert.Equal(expectedModel.Name, result.Name);
        Assert.Equal(expectedModel.Description, result.Description);
    }
}
