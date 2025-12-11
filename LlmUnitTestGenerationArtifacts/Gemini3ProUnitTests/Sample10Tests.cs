using AutoMapper;
using Dataset.Sample10;
using NSubstitute;

namespace Gemini3ProUnitTests;

public class AuctionServiceTests
{
    private readonly IUnitOfWork _unitOfWorkSubstitute;
    private readonly IAuctionRepository _auctionRepositorySubstitute;
    private readonly IMapper _mapperSubstitute;
    private readonly AuctionService _service;

    public AuctionServiceTests()
    {
        _unitOfWorkSubstitute = Substitute.For<IUnitOfWork>();
        _auctionRepositorySubstitute = Substitute.For<IAuctionRepository>();
        _mapperSubstitute = Substitute.For<IMapper>();

        _unitOfWorkSubstitute.Auctions.Returns(_auctionRepositorySubstitute);

        _service = new AuctionService(_unitOfWorkSubstitute, _mapperSubstitute);
    }

    #region Bet Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Bet_InvalidId_ThrowsInvalidIdException(int id)
    {
        // Arrange
        var customerName = "John Doe";
        var bid = 100;

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.Bet(id, customerName, bid));
    }

    [Fact]
    public void Bet_NullCustomerName_ThrowsInvalidNameException()
    {
        // Arrange
        var id = 1;
        string customerName = null!;
        var bid = 100;

        // Act & Assert
        Assert.Throws<InvalidNameException>(() => _service.Bet(id, customerName, bid));
    }

    [Fact]
    public void Bet_AuctionNotFound_ThrowsInvalidIdException()
    {
        // Arrange
        var id = 1;
        var customerName = "John Doe";
        var bid = 100;

        _auctionRepositorySubstitute.Get(id).Returns((Auction)null!);

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.Bet(id, customerName, bid));
    }

    [Fact]
    public void Bet_AuctionNotStarted_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var customerName = "John Doe";
        var bid = 100;
        var auction = new Auction { ID = id, Started = false };

        _auctionRepositorySubstitute.Get(id).Returns(auction);

        // Act & Assert
        var exception = Assert.Throws<InvalidAuctionException>(() => _service.Bet(id, customerName, bid));
        Assert.Equal("ERROR: Auction has not started yet", exception.Message);
    }

    [Fact]
    public void Bet_AuctionEnded_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var customerName = "John Doe";
        var bid = 100;
        var auction = new Auction { ID = id, Started = true, Ended = true };

        _auctionRepositorySubstitute.Get(id).Returns(auction);

        // Act & Assert
        var exception = Assert.Throws<InvalidAuctionException>(() => _service.Bet(id, customerName, bid));
        Assert.Equal("ERROR: Auction is over", exception.Message);
    }

    [Fact]
    public void Bet_BidNotHigherThanCurrent_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var customerName = "John Doe";
        var currentBid = 100;
        var newBid = 100; // Same or lower
        var auction = new Auction { ID = id, Started = true, Ended = false, Bid = currentBid };

        _auctionRepositorySubstitute.Get(id).Returns(auction);

        // Act & Assert
        var exception = Assert.Throws<InvalidAuctionException>(() => _service.Bet(id, customerName, newBid));
        Assert.Equal("ERROR: Invalid bid", exception.Message);
    }

    [Fact]
    public void Bet_ValidScenario_UpdatesAuctionAndCommits()
    {
        // Arrange
        var id = 1;
        var customerName = "Jane Doe";
        var currentBid = 100;
        var newBid = 150;
        var auction = new Auction
        {
            ID = id,
            Started = true,
            Ended = false,
            Bid = currentBid,
            Leader = "Old Leader"
        };

        _auctionRepositorySubstitute.Get(id).Returns(auction);

        // Act
        _service.Bet(id, customerName, newBid);

        // Assert
        Assert.Equal(newBid, auction.Bid);
        Assert.Equal(customerName, auction.Leader);
        _auctionRepositorySubstitute.Received(1).Update(auction);
        _unitOfWorkSubstitute.Received(1).Commit();
    }

    #endregion

    #region CloseAuction Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CloseAuction_InvalidId_ThrowsInvalidIdException(int id)
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.CloseAuction(id));
    }

    [Fact]
    public void CloseAuction_AuctionNotFound_ThrowsInvalidIdException()
    {
        // Arrange
        var id = 1;
        _auctionRepositorySubstitute.Get(id).Returns((Auction)null!);

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.CloseAuction(id));
    }

    [Fact]
    public void CloseAuction_AuctionNotStarted_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id, Started = false };
        _auctionRepositorySubstitute.Get(id).Returns(auction);

        // Act & Assert
        var exception = Assert.Throws<InvalidAuctionException>(() => _service.CloseAuction(id));
        Assert.Equal("ERROR: Auction has not started yet", exception.Message);
    }

    [Fact]
    public void CloseAuction_AuctionAlreadyEnded_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id, Started = true, Ended = true };
        _auctionRepositorySubstitute.Get(id).Returns(auction);

        // Act & Assert
        var exception = Assert.Throws<InvalidAuctionException>(() => _service.CloseAuction(id));
        Assert.Equal("ERROR: Auction has already finished", exception.Message);
    }

    [Fact]
    public void CloseAuction_ValidScenario_SetsEndedAndCommits()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id, Started = true, Ended = false };
        _auctionRepositorySubstitute.Get(id).Returns(auction);

        // Act
        _service.CloseAuction(id);

        // Assert
        Assert.True(auction.Ended);
        _auctionRepositorySubstitute.Received(1).Update(auction);
        _unitOfWorkSubstitute.Received(1).Commit();
    }

    #endregion

    #region GetAuction Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetAuction_InvalidId_ThrowsInvalidIdException(int id)
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.GetAuction(id));
    }

    [Fact]
    public void GetAuction_MappingReturnsNull_ThrowsInvalidIdException()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id };

        // Simulate repo returning an object, but mapping resulting in null (or repo returning null)
        _auctionRepositorySubstitute.Get(id).Returns(auction);
        _mapperSubstitute.Map<Auction, AuctionDTO>(auction).Returns((AuctionDTO)null!);

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.GetAuction(id));
    }

    [Fact]
    public void GetAuction_ValidScenario_ReturnsAuctionDTO()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id };
        var expectedDto = new AuctionDTO { ID = id };

        _auctionRepositorySubstitute.Get(id).Returns(auction);
        _mapperSubstitute.Map<Auction, AuctionDTO>(auction).Returns(expectedDto);

        // Act
        var result = _service.GetAuction(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto, result);
    }

    #endregion

    #region GetAuctions Tests

    [Fact]
    public void GetAuctions_WhenCalled_ReturnsMappedAuctions()
    {
        // Arrange
        var auctions = new List<Auction> { new Auction { ID = 1 }, new Auction { ID = 2 } };
        var expectedDtos = new List<AuctionDTO> { new AuctionDTO { ID = 1 }, new AuctionDTO { ID = 2 } };

        _auctionRepositorySubstitute.GetAll().Returns(auctions);
        _mapperSubstitute.Map<IEnumerable<Auction>, IEnumerable<AuctionDTO>>(auctions).Returns(expectedDtos);

        // Act
        var result = _service.GetAuctions();

        // Assert
        Assert.Same(expectedDtos, result);
    }

    #endregion

    #region OpenAuction Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void OpenAuction_InvalidId_ThrowsInvalidIdException(int id)
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.OpenAuction(id));
    }

    [Fact]
    public void OpenAuction_AuctionNotFound_ThrowsInvalidIdException()
    {
        // Arrange
        var id = 1;
        _auctionRepositorySubstitute.Get(id).Returns((Auction)null!);

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.OpenAuction(id));
    }

    [Fact]
    public void OpenAuction_AuctionAlreadyEnded_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id, Ended = true };
        _auctionRepositorySubstitute.Get(id).Returns(auction);

        // Act & Assert
        var exception = Assert.Throws<InvalidAuctionException>(() => _service.OpenAuction(id));
        Assert.Equal("ERROR: Auction has already finished", exception.Message);
    }

    [Fact]
    public void OpenAuction_AuctionAlreadyStarted_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id, Ended = false, Started = true };
        _auctionRepositorySubstitute.Get(id).Returns(auction);

        // Act & Assert
        var exception = Assert.Throws<InvalidAuctionException>(() => _service.OpenAuction(id));
        Assert.Equal("ERROR: Auction has already started", exception.Message);
    }

    [Fact]
    public void OpenAuction_ValidScenario_SetsStartedAndCommits()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id, Ended = false, Started = false };
        _auctionRepositorySubstitute.Get(id).Returns(auction);

        // Act
        _service.OpenAuction(id);

        // Assert
        Assert.True(auction.Started);
        _auctionRepositorySubstitute.Received(1).Update(auction);
        _unitOfWorkSubstitute.Received(1).Commit();
    }

    #endregion
}
