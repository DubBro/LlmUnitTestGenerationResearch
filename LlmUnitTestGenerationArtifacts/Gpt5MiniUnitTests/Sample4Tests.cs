using AutoMapper;
using Dataset.Sample4;
using NSubstitute;

namespace Gpt5MiniUnitTests
{
    public class CompanyServiceTests
    {
        [Fact]
        public async Task GetAsync_WhenCalledWithDefaultId_ReturnsMappedCompanyModel()
        {
            // Arrange
            var repository = Substitute.For<ICompanyRepository>();
            var mapper = Substitute.For<IMapper>();

            var entity = new CompanyEntity { Id = 1, Name = "TestCo", Description = "Desc" };
            var model = new CompanyModel { Id = 1, Name = "TestCoMapped", Description = "DescMapped" };

            repository.GetAsync(1).Returns(Task.FromResult(entity));
            mapper.Map<CompanyModel>(entity).Returns(model);

            var service = new CompanyService(repository, mapper);

            // Act
            var result = await service.GetAsync();

            // Assert
            Assert.Same(model, result);
            repository.Received(1).GetAsync(1);
            mapper.Received(1).Map<CompanyModel>(Arg.Is<CompanyEntity>(e => e == entity));
        }

        [Fact]
        public async Task GetAsync_WithSpecificId_PassesIdToRepositoryAndReturnsMappedModel()
        {
            // Arrange
            var repository = Substitute.For<ICompanyRepository>();
            var mapper = Substitute.For<IMapper>();

            int requestedId = 42;
            var entity = new CompanyEntity { Id = requestedId, Name = "Company42", Description = "Desc42" };
            var model = new CompanyModel { Id = requestedId, Name = "Company42Mapped", Description = "Desc42Mapped" };

            repository.GetAsync(requestedId).Returns(Task.FromResult(entity));
            mapper.Map<CompanyModel>(entity).Returns(model);

            var service = new CompanyService(repository, mapper);

            // Act
            var result = await service.GetAsync(requestedId);

            // Assert
            Assert.Same(model, result);
            repository.Received(1).GetAsync(requestedId);
            mapper.Received(1).Map<CompanyModel>(Arg.Is<CompanyEntity>(e => e == entity));
        }

        [Fact]
        public async Task GetAsync_WhenRepositoryReturnsNull_MapperCalledWithNullAndReturnsNullModel()
        {
            // Arrange
            var repository = Substitute.For<ICompanyRepository>();
            var mapper = Substitute.For<IMapper>();

            repository.GetAsync(Arg.Any<int>()).Returns(Task.FromResult<CompanyEntity?>(null));
            mapper.Map<CompanyModel>(Arg.Is<CompanyEntity>(e => e == null)).Returns((CompanyModel?)null);

            var service = new CompanyService(repository, mapper);

            // Act
            var result = await service.GetAsync(5);

            // Assert
            Assert.Null(result);
            repository.Received(1).GetAsync(5);
            mapper.Received(1).Map<CompanyModel>(Arg.Is<CompanyEntity>(e => e == null));
        }
    }
}
