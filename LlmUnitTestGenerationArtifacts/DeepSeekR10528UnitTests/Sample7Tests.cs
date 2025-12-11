using Dataset.Sample7;
using NSubstitute;

namespace DeepSeekR10528UnitTests;

public class WalkServiceTests
{
    private readonly IWalkRepository _walkRepositorySubstitute;
    private readonly IWalkService _service;

    public WalkServiceTests()
    {
        _walkRepositorySubstitute = Substitute.For<IWalkRepository>();
        _service = new WalkService(_walkRepositorySubstitute);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("12345678901234")]   // 14 digits
    [InlineData("1234567890123456")] // 16 digits
    [InlineData("12345678901234A")]  // non-digit
    [InlineData("IM1234567890123")]  // non-digit at beginning
    public async Task Get10LongestWalksAsync_InvalidImei_ThrowsBusinessException(string invalidImei)
    {
        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(
            () => _service.Get10LongestWalksAsync(invalidImei)
        );
    }

    [Fact]
    public async Task Get10LongestWalksAsync_ValidImei_ReturnsOrderedDTOsWithNumbers()
    {
        // Arrange
        string validImei = "123456789012345";
        var entities = new List<WalkEntity>
        {
            new WalkEntity { Distance = 20, Duration = 100 },
            new WalkEntity { Distance = 15, Duration = 90 }
        };
        _walkRepositorySubstitute.Get10LongestWalksAsync(validImei).Returns(entities);

        // Act
        var result = await _service.Get10LongestWalksAsync(validImei);

        // Assert
        Assert.Collection(result,
            dto1 =>
            {
                Assert.Equal(1, dto1.Number);
                Assert.Equal(20, dto1.Distance);
                Assert.Equal(100, dto1.Duration);
            },
            dto2 =>
            {
                Assert.Equal(2, dto2.Number);
                Assert.Equal(15, dto2.Distance);
                Assert.Equal(90, dto2.Duration);
            }
        );
    }

    [Fact]
    public async Task Get10LongestWalksAsync_RepositoryReturnsEmpty_ReturnsEmptyList()
    {
        // Arrange
        string validImei = "123456789012345";
        _walkRepositorySubstitute.Get10LongestWalksAsync(validImei).Returns(new List<WalkEntity>());

        // Act
        var result = await _service.Get10LongestWalksAsync(validImei);

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("12345678901234")]
    [InlineData("1234567890123456")]
    [InlineData("12345678901234A")]
    [InlineData("IM1234567890123")]
    public async Task GetWalksGeneralInfoAsync_InvalidImei_ThrowsBusinessException(string invalidImei)
    {
        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(
            () => _service.GetWalksGeneralInfoAsync(invalidImei)
        );
    }

    [Fact]
    public async Task GetWalksGeneralInfoAsync_ValidImei_MapsEntityToDTO()
    {
        // Arrange
        string validImei = "123456789012345";
        var entity = new WalksGeneralInfoEntity
        {
            TotalCount = 10,
            TotalDistance = 150,
            TotalDuration = 500
        };
        _walkRepositorySubstitute.GetWalksGeneralInfoAsync(validImei).Returns(entity);

        // Act
        var result = await _service.GetWalksGeneralInfoAsync(validImei);

        // Assert
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(150, result.TotalDistance);
        Assert.Equal(500, result.TotalDuration);
    }
}
