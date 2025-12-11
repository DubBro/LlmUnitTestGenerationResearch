using AutoMapper;
using Dataset.Sample10;
using NSubstitute;

namespace Qwen3Coder30BUnitTests
{
    public class AuctionServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuctionService _auctionService;

        public AuctionServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mapper = Substitute.For<IMapper>();
            _auctionService = new AuctionService(_unitOfWork, _mapper);
        }

        [Fact]
        public void Bet_PassingInvalidId_ThrowsInvalidIdException()
        {
            // Arrange
            var id = -1;
            var customerName = "John Doe";
            var bid = 100;

            // Act & Assert
            var exception = Assert.Throws<InvalidIdException>(() => _auctionService.Bet(id, customerName, bid));
        }

        [Fact]
        public void Bet_PassingNullCustomerName_ThrowsInvalidNameException()
        {
            // Arrange
            var id = 1;
            string customerName = null;
            var bid = 100;

            // Act & Assert
            var exception = Assert.Throws<InvalidNameException>(() => _auctionService.Bet(id, customerName, bid));
        }

        [Fact]
        public void Bet_PassingNonExistentAuction_ThrowsInvalidIdException()
        {
            // Arrange
            var id = 1;
            var customerName = "John Doe";
            var bid = 100;

            _unitOfWork.Auctions.Get(id).Returns((Auction)null);

            // Act & Assert
            var exception = Assert.Throws<InvalidIdException>(() => _auctionService.Bet(id, customerName, bid));
        }

        [Fact]
        public void Bet_PassingUnstartedAuction_ThrowsInvalidAuctionException()
        {
            // Arrange
            var id = 1;
            var customerName = "John Doe";
            var bid = 100;

            var auction = new Auction { ID = id, Started = false, Ended = false, Bid = 50 };
            _unitOfWork.Auctions.Get(id).Returns(auction);

            // Act & Assert
            var exception = Assert.Throws<InvalidAuctionException>(() => _auctionService.Bet(id, customerName, bid));
        }

        [Fact]
        public void Bet_PassingFinishedAuction_ThrowsInvalidAuctionException()
        {
            // Arrange
            var id = 1;
            var customerName = "John Doe";
            var bid = 100;

            var auction = new Auction { ID = id, Started = true, Ended = true, Bid = 50 };
            _unitOfWork.Auctions.Get(id).Returns(auction);

            // Act & Assert
            var exception = Assert.Throws<InvalidAuctionException>(() => _auctionService.Bet(id, customerName, bid));
        }

        [Fact]
        public void Bet_PassingLowerOrEqualBid_ThrowsInvalidAuctionException()
        {
            // Arrange
            var id = 1;
            var customerName = "John Doe";
            var bid = 100;

            var auction = new Auction { ID = id, Started = true, Ended = false, Bid = 150 };
            _unitOfWork.Auctions.Get(id).Returns(auction);

            // Act & Assert
            var exception = Assert.Throws<InvalidAuctionException>(() => _auctionService.Bet(id, customerName, bid));
        }

        [Fact]
        public void Bet_ValidInputs_UpdatesAuctionAndCommits()
        {
            // Arrange
            var id = 1;
            var customerName = "John Doe";
            var bid = 200;

            var auction = new Auction { ID = id, Started = true, Ended = false, Bid = 100 };
            _unitOfWork.Auctions.Get(id).Returns(auction);

            // Act
            _auctionService.Bet(id, customerName, bid);

            // Assert
            Assert.Equal(bid, auction.Bid);
            Assert.Equal(customerName, auction.Leader);
            _unitOfWork.Auctions.Received(1).Update(auction);
            _unitOfWork.Received(1).Commit();
        }

        [Fact]
        public void CloseAuction_PassingInvalidId_ThrowsInvalidIdException()
        {
            // Arrange
            var id = -1;

            // Act & Assert
            var exception = Assert.Throws<InvalidIdException>(() => _auctionService.CloseAuction(id));
        }

        [Fact]
        public void CloseAuction_PassingNonExistentAuction_ThrowsInvalidIdException()
        {
            // Arrange
            var id = 1;

            _unitOfWork.Auctions.Get(id).Returns((Auction)null);

            // Act & Assert
            var exception = Assert.Throws<InvalidIdException>(() => _auctionService.CloseAuction(id));
        }

        [Fact]
        public void CloseAuction_PassingUnstartedAuction_ThrowsInvalidAuctionException()
        {
            // Arrange
            var id = 1;

            var auction = new Auction { ID = id, Started = false, Ended = false };
            _unitOfWork.Auctions.Get(id).Returns(auction);

            // Act & Assert
            var exception = Assert.Throws<InvalidAuctionException>(() => _auctionService.CloseAuction(id));
        }

        [Fact]
        public void CloseAuction_PassingAlreadyFinishedAuction_ThrowsInvalidAuctionException()
        {
            // Arrange
            var id = 1;

            var auction = new Auction { ID = id, Started = true, Ended = true };
            _unitOfWork.Auctions.Get(id).Returns(auction);

            // Act & Assert
            var exception = Assert.Throws<InvalidAuctionException>(() => _auctionService.CloseAuction(id));
        }

        [Fact]
        public void CloseAuction_ValidInputs_SetsEndedTrueAndCommits()
        {
            // Arrange
            var id = 1;

            var auction = new Auction { ID = id, Started = true, Ended = false };
            _unitOfWork.Auctions.Get(id).Returns(auction);

            // Act
            _auctionService.CloseAuction(id);

            // Assert
            Assert.True(auction.Ended);
            _unitOfWork.Auctions.Received(1).Update(auction);
            _unitOfWork.Received(1).Commit();
        }

        [Fact]
        public void GetAuction_PassingInvalidId_ThrowsInvalidIdException()
        {
            // Arrange
            var id = -1;

            // Act & Assert
            var exception = Assert.Throws<InvalidIdException>(() => _auctionService.GetAuction(id));
        }

        [Fact]
        public void GetAuction_PassingNonExistentAuction_ThrowsInvalidIdException()
        {
            // Arrange
            var id = 1;

            _unitOfWork.Auctions.Get(id).Returns((Auction)null);

            // Act & Assert
            var exception = Assert.Throws<InvalidIdException>(() => _auctionService.GetAuction(id));
        }

        [Fact]
        public void GetAuction_ValidInputs_ReturnsMappedAuctionDTO()
        {
            // Arrange
            var id = 1;

            var auction = new Auction { ID = id, Bid = 100, Leader = "John Doe", Started = true, Ended = false };
            var auctionDto = new AuctionDTO { ID = id, Bid = 100, Leader = "John Doe", Started = true, Ended = false };

            _unitOfWork.Auctions.Get(id).Returns(auction);
            _mapper.Map<Auction, AuctionDTO>(auction).Returns(auctionDto);

            // Act
            var result = _auctionService.GetAuction(id);

            // Assert
            Assert.Equal(auctionDto, result);
        }

        [Fact]
        public void GetAuctions_ReturnsMappedAuctions()
        {
            // Arrange
            var auctions = new List<Auction>
            {
                new Auction { ID = 1, Bid = 100, Leader = "John Doe", Started = true, Ended = false },
                new Auction { ID = 2, Bid = 200, Leader = "Jane Smith", Started = true, Ended = false }
            };

            var auctionDtos = new List<AuctionDTO>
            {
                new AuctionDTO { ID = 1, Bid = 100, Leader = "John Doe", Started = true, Ended = false },
                new AuctionDTO { ID = 2, Bid = 200, Leader = "Jane Smith", Started = true, Ended = false }
            };

            _unitOfWork.Auctions.GetAll().Returns(auctions);
            _mapper.Map<IEnumerable<Auction>, IEnumerable<AuctionDTO>>(auctions).Returns(auctionDtos);

            // Act
            var result = _auctionService.GetAuctions();

            // Assert
            Assert.Equal(auctionDtos, result.ToList());
        }

        [Fact]
        public void OpenAuction_PassingInvalidId_ThrowsInvalidIdException()
        {
            // Arrange
            var id = -1;

            // Act & Assert
            var exception = Assert.Throws<InvalidIdException>(() => _auctionService.OpenAuction(id));
        }

        [Fact]
        public void OpenAuction_PassingNonExistentAuction_ThrowsInvalidIdException()
        {
            // Arrange
            var id = 1;

            _unitOfWork.Auctions.Get(id).Returns((Auction)null);

            // Act & Assert
            var exception = Assert.Throws<InvalidIdException>(() => _auctionService.OpenAuction(id));
        }

        [Fact]
        public void OpenAuction_PassingAlreadyFinishedAuction_ThrowsInvalidAuctionException()
        {
            // Arrange
            var id = 1;

            var auction = new Auction { ID = id, Started = false, Ended = true };
            _unitOfWork.Auctions.Get(id).Returns(auction);

            // Act & Assert
            var exception = Assert.Throws<InvalidAuctionException>(() => _auctionService.OpenAuction(id));
        }

        [Fact]
        public void OpenAuction_PassingAlreadyStartedAuction_ThrowsInvalidAuctionException()
        {
            // Arrange
            var id = 1;

            var auction = new Auction { ID = id, Started = true, Ended = false };
            _unitOfWork.Auctions.Get(id).Returns(auction);

            // Act & Assert
            var exception = Assert.Throws<InvalidAuctionException>(() => _auctionService.OpenAuction(id));
        }

        [Fact]
        public void OpenAuction_ValidInputs_SetsStartedTrueAndCommits()
        {
            // Arrange
            var id = 1;

            var auction = new Auction { ID = id, Started = false, Ended = false };
            _unitOfWork.Auctions.Get(id).Returns(auction);

            // Act
            _auctionService.OpenAuction(id);

            // Assert
            Assert.True(auction.Started);
            _unitOfWork.Auctions.Received(1).Update(auction);
            _unitOfWork.Received(1).Commit();
        }
    }
}
