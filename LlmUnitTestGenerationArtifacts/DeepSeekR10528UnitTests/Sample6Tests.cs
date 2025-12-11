using AutoMapper;
using Dataset.Sample6;
using NSubstitute;

namespace DeepSeekR10528UnitTests;

public class LotServiceTests
{
    private readonly ILotService _lotService;
    private readonly IUnitOfWork _databaseSubstitute;
    private readonly IMapper _mapperSubstitute;
    private readonly ILotRepository _lotRepositorySubstitute;
    private readonly IAuctionRepository _auctionRepositorySubstitute;

    public LotServiceTests()
    {
        _databaseSubstitute = Substitute.For<IUnitOfWork>();
        _mapperSubstitute = Substitute.For<IMapper>();
        _lotRepositorySubstitute = Substitute.For<ILotRepository>();
        _auctionRepositorySubstitute = Substitute.For<IAuctionRepository>();

        _databaseSubstitute.Lots.Returns(_lotRepositorySubstitute);
        _databaseSubstitute.Auctions.Returns(_auctionRepositorySubstitute);

        _lotService = new LotService(_databaseSubstitute, _mapperSubstitute);
    }

    [Fact]
    public void AddLot_LotIsNull_ThrowsInvalidLotException()
    {
        // Arrange
        LotDTO lotDto = null;

        // Act & Assert
        Assert.Throws<InvalidLotException>(() => _lotService.AddLot(lotDto));
    }

    [Fact]
    public void AddLot_LotSoldIsTrue_ThrowsInvalidLotException()
    {
        // Arrange
        var lotDto = new LotDTO { Sold = true };

        // Act & Assert
        Assert.Throws<InvalidLotException>(() => _lotService.AddLot(lotDto));
    }

    [Fact]
    public void AddLot_LotNameIsNull_ThrowsInvalidLotException()
    {
        // Arrange
        var lotDto = new LotDTO { Sold = false, Owner = "owner", Category = "category" };

        // Act & Assert
        Assert.Throws<InvalidLotException>(() => _lotService.AddLot(lotDto));
    }

    [Fact]
    public void AddLot_LotOwnerIsNull_ThrowsInvalidLotException()
    {
        // Arrange
        var lotDto = new LotDTO { Sold = false, Name = "name", Category = "category" };

        // Act & Assert
        Assert.Throws<InvalidLotException>(() => _lotService.AddLot(lotDto));
    }

    [Fact]
    public void AddLot_LotCategoryIsNull_ThrowsInvalidLotException()
    {
        // Arrange
        var lotDto = new LotDTO { Sold = false, Name = "name", Owner = "owner" };

        // Act & Assert
        Assert.Throws<InvalidLotException>(() => _lotService.AddLot(lotDto));
    }

    [Fact]
    public void AddLot_ValidLot_AddsLotAndCommits()
    {
        // Arrange
        var lotDto = new LotDTO
        {
            Name = "name",
            Owner = "owner",
            Sold = false,
            Category = "category"
        };
        var mappedLot = new Lot();
        _mapperSubstitute.Map<LotDTO, Lot>(lotDto).Returns(mappedLot);

        // Act
        _lotService.AddLot(lotDto);

        // Assert
        Assert.NotNull(lotDto.Auction);
        _lotRepositorySubstitute.Received().Add(mappedLot);
        _databaseSubstitute.Received().Commit();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DeleteLot_InvalidId_ThrowsInvalidIdException(int id)
    {
        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _lotService.DeleteLot(id));
    }

    [Fact]
    public void DeleteLot_ValidId_DeletesLotAndCommits()
    {
        // Arrange
        int id = 1;

        // Act
        _lotService.DeleteLot(id);

        // Assert
        _auctionRepositorySubstitute.Received().Delete(id);
        _lotRepositorySubstitute.Received().Delete(id);
        _databaseSubstitute.Received().Commit();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetLot_InvalidId_ThrowsInvalidIdException(int id)
    {
        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _lotService.GetLot(id));
    }

    [Fact]
    public void GetLot_ValidIdLotDoesNotExist_ThrowsInvalidIdException()
    {
        // Arrange
        int id = 1;
        _lotRepositorySubstitute.Get(id).Returns((Lot)null);

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _lotService.GetLot(id));
    }

    [Fact]
    public void GetLot_ValidIdLotExists_ReturnsLotDTO()
    {
        // Arrange
        int id = 1;
        var lot = new Lot();
        var expectedDto = new LotDTO();
        _lotRepositorySubstitute.Get(id).Returns(lot);
        _mapperSubstitute.Map<Lot, LotDTO>(lot).Returns(expectedDto);

        // Act
        var result = _lotService.GetLot(id);

        // Assert
        Assert.Same(expectedDto, result);
    }

    [Fact]
    public void GetLots_WhenCalled_ReturnsListOfLotDTO()
    {
        // Arrange
        var lots = new List<Lot>();
        var expectedDtos = new List<LotDTO>();
        _lotRepositorySubstitute.GetAll().Returns(lots);
        _mapperSubstitute.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(lots).Returns(expectedDtos);

        // Act
        var result = _lotService.GetLots();

        // Assert
        Assert.Same(expectedDtos, result);
    }

    [Fact]
    public void GetLotsByCategory_CategoryIsNull_ThrowsInvalidCategoryException()
    {
        // Act & Assert
        Assert.Throws<InvalidCategoryException>(() => _lotService.GetLotsByCategory(null));
    }

    [Fact]
    public void GetLotsByCategory_ValidCategory_ReturnsFilteredLots()
    {
        // Arrange
        string category = "category";
        var lots = new List<Lot>();
        var expectedDtos = new List<LotDTO>();
        _lotRepositorySubstitute.GetLotsByCategory(category).Returns(lots);
        _mapperSubstitute.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(lots).Returns(expectedDtos);

        // Act
        var result = _lotService.GetLotsByCategory(category);

        // Assert
        Assert.Same(expectedDtos, result);
    }

    [Fact]
    public void GetLotsByName_NameIsNull_ThrowsInvalidNameException()
    {
        // Act & Assert
        Assert.Throws<InvalidNameException>(() => _lotService.GetLotsByName(null));
    }

    [Fact]
    public void GetLotsByName_ValidName_ReturnsFilteredLots()
    {
        // Arrange
        string name = "name";
        var lots = new List<Lot>();
        var expectedDtos = new List<LotDTO>();
        _lotRepositorySubstitute.GetLotsByName(name).Returns(lots);
        _mapperSubstitute.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(lots).Returns(expectedDtos);

        // Act
        var result = _lotService.GetLotsByName(name);

        // Assert
        Assert.Same(expectedDtos, result);
    }

    [Fact]
    public void GetNotSoldLots_WhenCalled_ReturnsNotSoldLots()
    {
        // Arrange
        var lots = new List<Lot>();
        var expectedDtos = new List<LotDTO>();
        _lotRepositorySubstitute.GetNotSoldLots().Returns(lots);
        _mapperSubstitute.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(lots).Returns(expectedDtos);

        // Act
        var result = _lotService.GetNotSoldLots();

        // Assert
        Assert.Same(expectedDtos, result);
    }

    [Fact]
    public void GetSoldLots_WhenCalled_ReturnsSoldLots()
    {
        // Arrange
        var lots = new List<Lot>();
        var expectedDtos = new List<LotDTO>();
        _lotRepositorySubstitute.GetSoldLots().Returns(lots);
        _mapperSubstitute.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(lots).Returns(expectedDtos);

        // Act
        var result = _lotService.GetSoldLots();

        // Assert
        Assert.Same(expectedDtos, result);
    }

    [Fact]
    public void UpdateLot_LotDtoIsNull_ThrowsInvalidLotException()
    {
        // Act & Assert
        Assert.Throws<InvalidLotException>(() => _lotService.UpdateLot(null));
    }

    [Fact]
    public void UpdateLot_LotDtoNameIsNull_ThrowsInvalidLotException()
    {
        // Arrange
        var lotDto = new LotDTO { Owner = "owner", Category = "category" };

        // Act & Assert
        Assert.Throws<InvalidLotException>(() => _lotService.UpdateLot(lotDto));
    }

    [Fact]
    public void UpdateLot_LotDtoOwnerIsNull_ThrowsInvalidLotException()
    {
        // Arrange
        var lotDto = new LotDTO { Name = "name", Category = "category" };

        // Act & Assert
        Assert.Throws<InvalidLotException>(() => _lotService.UpdateLot(lotDto));
    }

    [Fact]
    public void UpdateLot_LotDtoCategoryIsNull_ThrowsInvalidLotException()
    {
        // Arrange
        var lotDto = new LotDTO { Name = "name", Owner = "owner" };

        // Act & Assert
        Assert.Throws<InvalidLotException>(() => _lotService.UpdateLot(lotDto));
    }

    [Fact]
    public void UpdateLot_LotDoesNotExistInDatabase_ThrowsInvalidIdException()
    {
        // Arrange
        var lotDto = new LotDTO { ID = 1, Name = "name", Owner = "owner", Category = "category" };
        _lotRepositorySubstitute.Get(lotDto.ID).Returns((Lot)null);

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _lotService.UpdateLot(lotDto));
    }

    [Fact]
    public void UpdateLot_ValidLotDto_UpdatesLotAndCommits()
    {
        // Arrange
        var lotDto = new LotDTO
        {
            ID = 1,
            Name = "newName",
            Owner = "newOwner",
            Sold = true,
            Category = "newCategory",
            Details = "newDetails"
        };
        var existingLot = new Lot();
        _lotRepositorySubstitute.Get(lotDto.ID).Returns(existingLot);

        // Act
        _lotService.UpdateLot(lotDto);

        // Assert
        Assert.Equal("newName", existingLot.Name);
        Assert.Equal("newOwner", existingLot.Owner);
        Assert.True(existingLot.Sold);
        Assert.Equal("newCategory", existingLot.Category);
        Assert.Equal("newDetails", existingLot.Details);
        _lotRepositorySubstitute.Received().Update(existingLot);
        _databaseSubstitute.Received().Commit();
    }
}
