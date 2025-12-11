using AutoMapper;
using Dataset.Sample4;
using NSubstitute;

namespace DeepSeekR10528UnitTests;

public class CompanyServiceTests
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;
    private readonly CompanyService _sut;

    public CompanyServiceTests()
    {
        _companyRepository = Substitute.For<ICompanyRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new CompanyService(_companyRepository, _mapper);
    }

    [Fact]
    public async Task GetAsync_ValidEntityFound_ReturnsMappedModel()
    {
        // Arrange
        var entity = new CompanyEntity { Id = 1, Name = "Test", Description = "Description" };
        var expectedModel = new CompanyModel();
        _companyRepository.GetAsync(1).Returns(Task.FromResult<CompanyEntity>(entity));
        _mapper.Map<CompanyModel>(entity).Returns(expectedModel);

        // Act
        var result = await _sut.GetAsync(1);

        // Assert
        Assert.Same(expectedModel, result);
    }

    [Fact]
    public async Task GetAsync_EntityNotFound_ReturnsNull()
    {
        // Arrange
        _companyRepository.GetAsync(999).Returns(Task.FromResult<CompanyEntity>(null!));
        _mapper.Map<CompanyModel>(null!).Returns((CompanyModel)null!);

        // Act
        var result = await _sut.GetAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public async Task GetAsync_WithId_CallsRepositoryWithCorrectId(int id)
    {
        // Arrange
        _companyRepository.GetAsync(id).Returns(Task.FromResult<CompanyEntity>(null!));
        _mapper.Map<CompanyModel>(null!).Returns((CompanyModel)null!);

        // Act
        await _sut.GetAsync(id);

        // Assert
        await _companyRepository.Received(1).GetAsync(id);
    }

    [Fact]
    public async Task GetAsync_WithoutId_UsesDefaultId()
    {
        // Arrange
        const int defaultId = 1;
        _companyRepository.GetAsync(defaultId).Returns(Task.FromResult<CompanyEntity>(null!));
        _mapper.Map<CompanyModel>(null!).Returns((CompanyModel)null!);

        // Act
        await _sut.GetAsync();

        // Assert
        await _companyRepository.Received(1).GetAsync(defaultId);
    }

    [Fact]
    public async Task GetAsync_ValidEntity_CallsMapperWithRepositoryResult()
    {
        // Arrange
        var entity = new CompanyEntity { Id = 2, Name = "ABC", Description = "Test Desc" };
        _companyRepository.GetAsync(2).Returns(Task.FromResult(entity));
        var model = new CompanyModel();
        _mapper.Map<CompanyModel>(entity).Returns(model);

        // Act
        await _sut.GetAsync(2);

        // Assert
        _mapper.Received(1).Map<CompanyModel>(entity);
    }
}
