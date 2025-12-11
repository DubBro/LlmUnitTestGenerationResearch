using AutoMapper;
using Dataset.Sample10;
using NSubstitute;

namespace Gpt5MiniUnitTests
{
    public class AuctionServiceTests
    {
        private static (AuctionService Sut, IUnitOfWork Db, IAuctionRepository Repo, IMapper Mapper) CreateSut()
        {
            var repo = Substitute.For<IAuctionRepository>();
            var db = Substitute.For<IUnitOfWork>();
            db.Auctions.Returns(repo);
            var mapper = Substitute.For<IMapper>();
            var sut = new AuctionService(db, mapper);
            return (sut, db, repo, mapper);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void GetAuction_IdLessOrEqualZero_ThrowsInvalidIdException(int id)
        {
            // Arrange
            var (sut, _, _, _) = CreateSut();

            // Act / Assert
            Assert.Throws<InvalidIdException>(() => sut.GetAuction(id));
        }

        [Fact]
        public void GetAuction_MapperReturnsDto_ReturnsMappedDto()
        {
            // Arrange
            var (sut, db, repo, mapper) = CreateSut();
            var auction = new Auction { ID = 1, Bid = 10, Leader = "Alice", Started = true, Ended = false };
            repo.Get(1).Returns(auction);
            var dto = new AuctionDTO { ID = 1, Bid = 10, Leader = "Alice", Started = true, Ended = false };
            mapper.Map<Auction, AuctionDTO>(auction).Returns(dto);

            // Act
            var result = sut.GetAuction(1);

            // Assert
            Assert.Same(dto, result);
        }

        [Fact]
        public void GetAuction_MapperReturnsNull_ThrowsInvalidIdException()
        {
            // Arrange
            var (sut, db, repo, mapper) = CreateSut();
            repo.Get(42).Returns((Auction?)null);
            mapper.Map<Auction, AuctionDTO>(Arg.Any<Auction>()).Returns((AuctionDTO?)null);

            // Act / Assert
            Assert.Throws<InvalidIdException>(() => sut.GetAuction(42));
        }

        [Fact]
        public void GetAuctions_MapsAllAuctions_ReturnsDtos()
        {
            // Arrange
            var (sut, db, repo, mapper) = CreateSut();
            var auctions = new List<Auction>
            {
                new Auction { ID = 1, Bid = 5, Leader = "A", Started = true, Ended = false },
                new Auction { ID = 2, Bid = 15, Leader = "B", Started = true, Ended = false }
            };
            repo.GetAll().Returns(auctions);
            var mapped = new List<AuctionDTO>
            {
                new AuctionDTO { ID = 1, Bid = 5, Leader = "A", Started = true, Ended = false },
                new AuctionDTO { ID = 2, Bid = 15, Leader = "B", Started = true, Ended = false }
            };
            mapper.Map<IEnumerable<Auction>, IEnumerable<AuctionDTO>>(auctions).Returns(mapped);

            // Act
            var result = sut.GetAuctions();

            // Assert
            Assert.Equal(mapped, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void OpenAuction_InvalidId_ThrowsInvalidIdException(int id)
        {
            // Arrange
            var (sut, _, _, _) = CreateSut();

            // Act / Assert
            Assert.Throws<InvalidIdException>(() => sut.OpenAuction(id));
        }

        [Fact]
        public void OpenAuction_AuctionNotFound_ThrowsInvalidIdException()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            repo.Get(1).Returns((Auction?)null);

            // Act / Assert
            Assert.Throws<InvalidIdException>(() => sut.OpenAuction(1));
        }

        [Fact]
        public void OpenAuction_WhenEnded_ThrowsInvalidAuctionExceptionWithCorrectMessage()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            var auction = new Auction { ID = 1, Started = false, Ended = true };
            repo.Get(1).Returns(auction);

            // Act
            var ex = Assert.Throws<InvalidAuctionException>(() => sut.OpenAuction(1));

            // Assert
            Assert.Equal("ERROR: Auction has already finished", ex.Message);
        }

        [Fact]
        public void OpenAuction_WhenAlreadyStarted_ThrowsInvalidAuctionExceptionWithCorrectMessage()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            var auction = new Auction { ID = 1, Started = true, Ended = false };
            repo.Get(1).Returns(auction);

            // Act
            var ex = Assert.Throws<InvalidAuctionException>(() => sut.OpenAuction(1));

            // Assert
            Assert.Equal("ERROR: Auction has already started", ex.Message);
        }

        [Fact]
        public void OpenAuction_ValidAuction_StartsAuctionAndSaves()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            var auction = new Auction { ID = 1, Started = false, Ended = false };
            repo.Get(1).Returns(auction);

            // Act
            sut.OpenAuction(1);

            // Assert
            Assert.True(auction.Started);
            repo.Received(1).Update(auction);
            db.Received(1).Commit();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        public void CloseAuction_InvalidId_ThrowsInvalidIdException(int id)
        {
            // Arrange
            var (sut, _, _, _) = CreateSut();

            // Act / Assert
            Assert.Throws<InvalidIdException>(() => sut.CloseAuction(id));
        }

        [Fact]
        public void CloseAuction_AuctionNotFound_ThrowsInvalidIdException()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            repo.Get(5).Returns((Auction?)null);

            // Act / Assert
            Assert.Throws<InvalidIdException>(() => sut.CloseAuction(5));
        }

        [Fact]
        public void CloseAuction_NotStarted_ThrowsInvalidAuctionExceptionWithCorrectMessage()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            var auction = new Auction { ID = 1, Started = false, Ended = false };
            repo.Get(1).Returns(auction);

            // Act
            var ex = Assert.Throws<InvalidAuctionException>(() => sut.CloseAuction(1));

            // Assert
            Assert.Equal("ERROR: Auction has not started yet", ex.Message);
        }

        [Fact]
        public void CloseAuction_AlreadyEnded_ThrowsInvalidAuctionExceptionWithCorrectMessage()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            var auction = new Auction { ID = 1, Started = true, Ended = true };
            repo.Get(1).Returns(auction);

            // Act
            var ex = Assert.Throws<InvalidAuctionException>(() => sut.CloseAuction(1));

            // Assert
            Assert.Equal("ERROR: Auction has already finished", ex.Message);
        }

        [Fact]
        public void CloseAuction_ValidAuction_SetsEndedAndSaves()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            var auction = new Auction { ID = 1, Started = true, Ended = false };
            repo.Get(1).Returns(auction);

            // Act
            sut.CloseAuction(1);

            // Assert
            Assert.True(auction.Ended);
            repo.Received(1).Update(auction);
            db.Received(1).Commit();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void Bet_InvalidId_ThrowsInvalidIdException(int id)
        {
            // Arrange
            var (sut, _, _, _) = CreateSut();

            // Act / Assert
            Assert.Throws<InvalidIdException>(() => sut.Bet(id, "name", 100));
        }

        [Fact]
        public void Bet_NullCustomerName_ThrowsInvalidNameException()
        {
            // Arrange
            var (sut, _, _, _) = CreateSut();

            // Act / Assert
            Assert.Throws<InvalidNameException>(() => sut.Bet(1, null!, 10));
        }

        [Fact]
        public void Bet_AuctionNotFound_ThrowsInvalidIdException()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            repo.Get(10).Returns((Auction?)null);

            // Act / Assert
            Assert.Throws<InvalidIdException>(() => sut.Bet(10, "Bob", 20));
        }

        [Fact]
        public void Bet_AuctionNotStarted_ThrowsInvalidAuctionExceptionWithCorrectMessage()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            var auction = new Auction { ID = 1, Started = false, Ended = false, Bid = 0 };
            repo.Get(1).Returns(auction);

            // Act
            var ex = Assert.Throws<InvalidAuctionException>(() => sut.Bet(1, "Bob", 10));

            // Assert
            Assert.Equal("ERROR: Auction has not started yet", ex.Message);
        }

        [Fact]
        public void Bet_AuctionEnded_ThrowsInvalidAuctionExceptionWithCorrectMessage()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            var auction = new Auction { ID = 1, Started = true, Ended = true, Bid = 0 };
            repo.Get(1).Returns(auction);

            // Act
            var ex = Assert.Throws<InvalidAuctionException>(() => sut.Bet(1, "Bob", 10));

            // Assert
            Assert.Equal("ERROR: Auction is over", ex.Message);
        }

        [Fact]
        public void Bet_BidNotHigher_ThrowsInvalidAuctionExceptionWithCorrectMessage()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            var auction = new Auction { ID = 1, Started = true, Ended = false, Bid = 100 };
            repo.Get(1).Returns(auction);

            // Act
            var ex = Assert.Throws<InvalidAuctionException>(() => sut.Bet(1, "Bob", 100));

            // Assert
            Assert.Equal("ERROR: Invalid bid", ex.Message);
        }

        [Fact]
        public void Bet_ValidBid_UpdatesLeaderAndBidAndSaves()
        {
            // Arrange
            var (sut, db, repo, _) = CreateSut();
            var auction = new Auction { ID = 1, Started = true, Ended = false, Bid = 50, Leader = "Old" };
            repo.Get(1).Returns(auction);

            // Act
            sut.Bet(1, "NewLeader", 75);

            // Assert
            Assert.Equal(75, auction.Bid);
            Assert.Equal("NewLeader", auction.Leader);
            repo.Received(1).Update(auction);
            db.Received(1).Commit();
        }
    }
}
