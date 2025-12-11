using AutoMapper;
using Dataset.Sample10;
using NSubstitute;

namespace DeepSeekR10528UnitTests;

public class AuctionServiceTests
{
    private readonly IAuctionService _service;
    private readonly IUnitOfWork _database;
    private readonly IAuctionRepository _auctionRepository;
    private readonly IMapper _mapper;

    public AuctionServiceTests()
    {
        _database = Substitute.For<IUnitOfWork>();
        _auctionRepository = Substitute.For<IAuctionRepository>();
        _database.Auctions.Returns(_auctionRepository);
        _mapper = Substitute.For<IMapper>();
        _service = new AuctionService(_database, _mapper);
    }

    [Fact]
    public void GetAuction_PassingInvalidId_ThrowsInvalidIdException()
    {
        // Arrange
        var invalidId = 0;

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.GetAuction(invalidId));
    }

    [Fact]
    public void GetAuction_AuctionNotFound_ThrowsInvalidIdException()
    {
        // Arrange
        var id = 1;
        _auctionRepository.Get(id).Returns((Auction)null);

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.GetAuction(id));
    }

    [Fact]
    public void GetAuction_ValidId_ReturnsMappedAuctionDTO()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id };
        var expectedDto = new AuctionDTO();
        _auctionRepository.Get(id).Returns(auction);
        _mapper.Map<AuctionDTO>(auction).Returns(expectedDto);

        // Act
        var result = _service.GetAuction(id);

        // Assert
        Assert.Same(expectedDto, result);
    }

    [Fact]
    public void GetAuctions_WhenCalled_ReturnsMappedCollection()
    {
        // Arrange
        var auctions = new List<Auction> { new Auction() };
        var expectedDtos = new List<AuctionDTO> { new AuctionDTO() };
        _auctionRepository.GetAll().Returns(auctions);
        _mapper.Map<IEnumerable<Auction>, IEnumerable<AuctionDTO>>(auctions).Returns(expectedDtos);

        // Act
        var result = _service.GetAuctions();

        // Assert
        Assert.Same(expectedDtos, result);
    }

    [Fact]
    public void OpenAuction_PassingInvalidId_ThrowsInvalidIdException()
    {
        // Arrange
        var invalidId = 0;

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.OpenAuction(invalidId));
    }

    [Fact]
    public void OpenAuction_AuctionNotFound_ThrowsInvalidIdException()
    {
        // Arrange
        var id = 1;
        _auctionRepository.Get(id).Returns((Auction)null);

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.OpenAuction(id));
    }

    [Fact]
    public void OpenAuction_AuctionEnded_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var endedAuction = new Auction { ID = id, Ended = true };
        _auctionRepository.Get(id).Returns(endedAuction);

        // Act & Assert
        var ex = Assert.Throws<InvalidAuctionException>(() => _service.OpenAuction(id));
        Assert.Equal("ERROR: Auction has already finished", ex.Message);
    }

    [Fact]
    public void OpenAuction_AuctionAlreadyStarted_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var startedAuction = new Auction { ID = id, Started = true };
        _auctionRepository.Get(id).Returns(startedAuction);

        // Act & Assert
        var ex = Assert.Throws<InvalidAuctionException>(() => _service.OpenAuction(id));
        Assert.Equal("ERROR: Auction has already started", ex.Message);
    }

    [Fact]
    public void OpenAuction_ValidState_OpensAuctionAndCommits()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id, Started = false, Ended = false };
        _auctionRepository.Get(id).Returns(auction);

        // Act
        _service.OpenAuction(id);

        // Assert
        Assert.True(auction.Started);
        _auctionRepository.Received(1).Update(auction);
        _database.Received(1).Commit();
    }

    [Fact]
    public void CloseAuction_PassingInvalidId_ThrowsInvalidIdException()
    {
        // Arrange
        var invalidId = 0;

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.CloseAuction(invalidId));
    }

    [Fact]
    public void CloseAuction_AuctionNotFound_ThrowsInvalidIdException()
    {
        // Arrange
        var id = 1;
        _auctionRepository.Get(id).Returns((Auction)null);

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.CloseAuction(id));
    }

    [Fact]
    public void CloseAuction_AuctionNotStarted_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id, Started = false };
        _auctionRepository.Get(id).Returns(auction);

        // Act & Assert
        var ex = Assert.Throws<InvalidAuctionException>(() => _service.CloseAuction(id));
        Assert.Equal("ERROR: Auction has not started yet", ex.Message);
    }

    [Fact]
    public void CloseAuction_AuctionAlreadyEnded_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id, Started = true, Ended = true };
        _auctionRepository.Get(id).Returns(auction);

        // Act & Assert
        var ex = Assert.Throws<InvalidAuctionException>(() => _service.CloseAuction(id));
        Assert.Equal("ERROR: Auction has already finished", ex.Message);
    }

    [Fact]
    public void CloseAuction_ValidState_ClosesAuctionAndCommits()
    {
        // Arrange
        var id = 1;
        var auction = new Auction { ID = id, Started = true, Ended = false };
        _auctionRepository.Get(id).Returns(auction);

        // Act
        _service.CloseAuction(id);

        // Assert
        Assert.True(auction.Ended);
        _auctionRepository.Received(1).Update(auction);
        _database.Received(1).Commit();
    }

    [Fact]
    public void Bet_PassingInvalidId_ThrowsInvalidIdException()
    {
        // Arrange
        var invalidId = 0;
        var customerName = "Customer";
        var bid = 100;

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.Bet(invalidId, customerName, bid));
    }

    [Fact]
    public void Bet_PassingNullCustomerName_ThrowsInvalidNameException()
    {
        // Arrange
        var id = 1;
        string customerName = null;
        var bid = 100;

        // Act & Assert
        Assert.Throws<InvalidNameException>(() => _service.Bet(id, customerName, bid));
    }

    [Fact]
    public void Bet_AuctionNotFound_ThrowsInvalidIdException()
    {
        // Arrange
        var id = 1;
        var customerName = "Customer";
        var bid = 100;
        _auctionRepository.Get(id).Returns((Auction)null);

        // Act & Assert
        Assert.Throws<InvalidIdException>(() => _service.Bet(id, customerName, bid));
    }

    [Fact]
    public void Bet_AuctionNotStarted_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var customerName = "Customer";
        var bid = 100;
        var auction = new Auction { ID = id, Started = false };
        _auctionRepository.Get(id).Returns(auction);

        // Act & Assert
        var ex = Assert.Throws<InvalidAuctionException>(() => _service.Bet(id, customerName, bid));
        Assert.Equal("ERROR: Auction has not started yet", ex.Message);
    }

    [Fact]
    public void Bet_AuctionEnded_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var customerName = "Customer";
        var bid = 100;
        var auction = new Auction { ID = id, Started = true, Ended = true };
        _auctionRepository.Get(id).Returns(auction);

        // Act & Assert
        var ex = Assert.Throws<InvalidAuctionException>(() => _service.Bet(id, customerName, bid));
        Assert.Equal("ERROR: Auction is over", ex.Message);
    }

    [Fact]
    public void Bet_BidNotHigherThanCurrent_ThrowsInvalidAuctionException()
    {
        // Arrange
        var id = 1;
        var customerName = "Customer";
        var bid = 100;
        var auction = new Auction { ID = id, Started = true, Ended = false, Bid = 150 };
        _auctionRepository.Get(id).Returns(auction);

        // Act & Assert
        var ex = Assert.Throws<InvalidAuctionException>(() => _service.Bet(id, customerName, bid));
        Assert.Equal("ERROR: Invalid bid", ex.Message);
    }

    [Fact]
    public void Bet_ValidBet_UpdatesAuctionAndCommits()
    {
        // Arrange
        var id = 1;
        var customerName = "Customer";
        var bid = 200;
        var auction = new Auction { ID = id, Started = true, Ended = false, Bid = 100 };
        _auctionRepository.Get(id).Returns(auction);

        // Act
        _service.Bet(id, customerName, bid);

        // Assert
        Assert.Equal(bid, auction.Bid);
        Assert.Equal(customerName, auction.Leader);
        _auctionRepository.Received(1).Update(auction);
        _database.Received(1).Commit();
    }
}
