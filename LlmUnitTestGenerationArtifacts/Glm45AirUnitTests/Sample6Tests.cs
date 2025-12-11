using AutoMapper;
using Dataset.Sample6;
using NSubstitute;

namespace Glm45AirUnitTests;

public class LotServiceTests
{
    private readonly IUnitOfWork _unitOfWorkSubstitute;
    private readonly IMapper _mapperSubstitute;
    private readonly LotService _lotService;

    public LotServiceTests()
    {
        _unitOfWorkSubstitute = Substitute.For<IUnitOfWork>();
        _mapperSubstitute = Substitute.For<IMapper>();
        _lotService = new LotService(_unitOfWorkSubstitute, _mapperSubstitute);
    }

    public class AddLot : LotServiceTests
    {
        [Fact]
        public void GivenNullLot_ThrowsInvalidLotException()
        {
            // Arrange
            LotDTO lot = null;

            // Act & Assert
            Assert.Throws<InvalidLotException>(() => _lotService.AddLot(lot));
        }

        [Fact]
        public void GivenLotWithSoldTrue_ThrowsInvalidLotException()
        {
            // Arrange
            var lot = new LotDTO { Sold = true };

            // Act & Assert
            Assert.Throws<InvalidLotException>(() => _lotService.AddLot(lot));
        }

        [Fact]
        public void GivenLotWithNullName_ThrowsInvalidLotException()
        {
            // Arrange
            var lot = new LotDTO { Name = null };

            // Act & Assert
            Assert.Throws<InvalidLotException>(() => _lotService.AddLot(lot));
        }

        [Fact]
        public void GivenLotWithNullOwner_ThrowsInvalidLotException()
        {
            // Arrange
            var lot = new LotDTO { Owner = null };

            // Act & Assert
            Assert.Throws<InvalidLotException>(() => _lotService.AddLot(lot));
        }

        [Fact]
        public void GivenLotWithNullCategory_ThrowsInvalidLotException()
        {
            // Arrange
            var lot = new LotDTO { Category = null };

            // Act & Assert
            Assert.Throws<InvalidLotException>(() => _lotService.AddLot(lot));
        }

        [Fact]
        public void GivenValidLot_AddsLotAndCommits()
        {
            // Arrange
            var lotDto = new LotDTO { Name = "Test Lot", Owner = "Test Owner", Category = "Test Category", Sold = false };
            var mappedLot = new Lot();
            _mapperSubstitute.Map<LotDTO, Lot>(lotDto).Returns(mappedLot);

            // Act
            _lotService.AddLot(lotDto);

            // Assert
            _unitOfWorkSubstitute.Lots.Received(1).Add(mappedLot);
            _unitOfWorkSubstitute.Received(1).Commit();
            Assert.NotNull(lotDto.Auction);
        }
    }

    public class DeleteLot : LotServiceTests
    {
        [Fact]
        public void GivenInvalidId_ThrowsInvalidIdException()
        {
            // Arrange
            int id = 0;

            // Act & Assert
            Assert.Throws<InvalidIdException>(() => _lotService.DeleteLot(id));
        }

        [Fact]
        public void GivenValidId_DeletesLotAndAuctionAndCommits()
        {
            // Arrange
            int id = 1;

            // Act
            _lotService.DeleteLot(id);

            // Assert
            _unitOfWorkSubstitute.Auctions.Received(1).Delete(id);
            _unitOfWorkSubstitute.Lots.Received(1).Delete(id);
            _unitOfWorkSubstitute.Received(1).Commit();
        }
    }

    public class GetLot : LotServiceTests
    {
        [Fact]
        public void GivenInvalidId_ThrowsInvalidIdException()
        {
            // Arrange
            int id = 0;
            _unitOfWorkSubstitute.Lots.Get(id).Returns((Lot)null);

            // Act & Assert
            Assert.Throws<InvalidIdException>(() => _lotService.GetLot(id));
        }

        [Fact]
        public void GivenValidId_WhenLotExists_ReturnsMappedLot()
        {
            // Arrange
            int id = 1;
            var lot = new Lot();
            var lotDto = new LotDTO();
            _unitOfWorkSubstitute.Lots.Get(id).Returns(lot);
            _mapperSubstitute.Map<Lot, LotDTO>(lot).Returns(lotDto);

            // Act
            var result = _lotService.GetLot(id);

            // Assert
            Assert.Equal(lotDto, result);
        }

        [Fact]
        public void GivenValidId_WhenLotDoesNotExist_ThrowsInvalidIdException()
        {
            // Arrange
            int id = 1;
            _unitOfWorkSubstitute.Lots.Get(id).Returns((Lot)null);

            // Act & Assert
            Assert.Throws<InvalidIdException>(() => _lotService.GetLot(id));
        }
    }

    public class GetLots : LotServiceTests
    {
        [Fact]
        public void WhenCalled_ReturnsMappedLots()
        {
            // Arrange
            var lots = new List<Lot>().AsReadOnly();
            var lotDtos = new List<LotDTO>().AsReadOnly();
            _unitOfWorkSubstitute.Lots.GetAll().Returns(lots);
            _mapperSubstitute.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(lots).Returns(lotDtos);

            // Act
            var result = _lotService.GetLots();

            // Assert
            Assert.Equal(lotDtos, result);
        }
    }

    public class GetLotsByCategory : LotServiceTests
    {
        [Fact]
        public void GivenNullCategory_ThrowsInvalidCategoryException()
        {
            // Arrange
            string category = null;

            // Act & Assert
            Assert.Throws<InvalidCategoryException>(() => _lotService.GetLotsByCategory(category));
        }

        [Fact]
        public void GivenValidCategory_ReturnsMappedLots()
        {
            // Arrange
            string category = "Test Category";
            var lots = new List<Lot>().AsReadOnly();
            var lotDtos = new List<LotDTO>().AsReadOnly();
            _unitOfWorkSubstitute.Lots.GetLotsByCategory(category).Returns(lots);
            _mapperSubstitute.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(lots).Returns(lotDtos);

            // Act
            var result = _lotService.GetLotsByCategory(category);

            // Assert
            Assert.Equal(lotDtos, result);
        }
    }

    public class GetLotsByName : LotServiceTests
    {
        [Fact]
        public void GivenNullName_ThrowsInvalidNameException()
        {
            // Arrange
            string name = null;

            // Act & Assert
            Assert.Throws<InvalidNameException>(() => _lotService.GetLotsByName(name));
        }

        [Fact]
        public void GivenValidName_ReturnsMappedLots()
        {
            // Arrange
            string name = "Test Name";
            var lots = new List<Lot>().AsReadOnly();
            var lotDtos = new List<LotDTO>().AsReadOnly();
            _unitOfWorkSubstitute.Lots.GetLotsByName(name).Returns(lots);
            _mapperSubstitute.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(lots).Returns(lotDtos);

            // Act
            var result = _lotService.GetLotsByName(name);

            // Assert
            Assert.Equal(lotDtos, result);
        }
    }

    public class GetNotSoldLots : LotServiceTests
    {
        [Fact]
        public void WhenCalled_ReturnsMappedLots()
        {
            // Arrange
            var lots = new List<Lot>().AsReadOnly();
            var lotDtos = new List<LotDTO>().AsReadOnly();
            _unitOfWorkSubstitute.Lots.GetNotSoldLots().Returns(lots);
            _mapperSubstitute.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(lots).Returns(lotDtos);

            // Act
            var result = _lotService.GetNotSoldLots();

            // Assert
            Assert.Equal(lotDtos, result);
        }
    }

    public class GetSoldLots : LotServiceTests
    {
        [Fact]
        public void WhenCalled_ReturnsMappedLots()
        {
            // Arrange
            var lots = new List<Lot>().AsReadOnly();
            var lotDtos = new List<LotDTO>().AsReadOnly();
            _unitOfWorkSubstitute.Lots.GetSoldLots().Returns(lots);
            _mapperSubstitute.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(lots).Returns(lotDtos);

            // Act
            var result = _lotService.GetSoldLots();

            // Assert
            Assert.Equal(lotDtos, result);
        }
    }

    public class UpdateLot : LotServiceTests
    {
        [Fact]
        public void GivenNullLotDto_ThrowsInvalidLotException()
        {
            // Arrange
            LotDTO lotDto = null;

            // Act & Assert
            Assert.Throws<InvalidLotException>(() => _lotService.UpdateLot(lotDto));
        }

        [Fact]
        public void GivenLotDtoWithNullName_ThrowsInvalidLotException()
        {
            // Arrange
            var lotDto = new LotDTO { Name = null };

            // Act & Assert
            Assert.Throws<InvalidLotException>(() => _lotService.UpdateLot(lotDto));
        }

        [Fact]
        public void GivenLotDtoWithNullOwner_ThrowsInvalidLotException()
        {
            // Arrange
            var lotDto = new LotDTO { Owner = null };

            // Act & Assert
            Assert.Throws<InvalidLotException>(() => _lotService.UpdateLot(lotDto));
        }

        [Fact]
        public void GivenLotDtoWithNullCategory_ThrowsInvalidLotException()
        {
            // Arrange
            var lotDto = new LotDTO { Category = null };

            // Act & Assert
            Assert.Throws<InvalidLotException>(() => _lotService.UpdateLot(lotDto));
        }

        [Fact]
        public void GivenLotDtoWithNonExistingId_ThrowsInvalidIdException()
        {
            // Arrange
            var lotDto = new LotDTO { ID = 1, Name = "Test", Owner = "Test Owner", Category = "Test" };
            _unitOfWorkSubstitute.Lots.Get(lotDto.ID).Returns((Lot)null);

            // Act & Assert
            Assert.Throws<InvalidIdException>(() => _lotService.UpdateLot(lotDto));
        }

        [Fact]
        public void GivenValidLotDto_UpdatesLotAndCommits()
        {
            // Arrange
            var lotDto = new LotDTO { ID = 1, Name = "Updated Name", Owner = "Updated Owner", Category = "Updated Category", Sold = true, Details = "Updated Details" };
            var existingLot = new Lot();
            _unitOfWorkSubstitute.Lots.Get(lotDto.ID).Returns(existingLot);

            // Act
            _lotService.UpdateLot(lotDto);

            // Assert
            Assert.Equal(lotDto.Name, existingLot.Name);
            Assert.Equal(lotDto.Owner, existingLot.Owner);
            Assert.Equal(lotDto.Sold, existingLot.Sold);
            Assert.Equal(lotDto.Category, existingLot.Category);
            Assert.Equal(lotDto.Details, existingLot.Details);
            _unitOfWorkSubstitute.Lots.Received(1).Update(existingLot);
            _unitOfWorkSubstitute.Received(1).Commit();
        }
    }
}
