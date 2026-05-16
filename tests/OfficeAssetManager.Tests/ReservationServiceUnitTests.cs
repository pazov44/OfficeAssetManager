using Moq;
using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.Domain.Enums;
using OfficeAssetManager.Core.DTO;
using OfficeAssetManager.Core.ServiceContracts;
using OfficeAssetManager.Core.Services;
using FluentAssertions;
using OfficeAssetManager.Core.Domain.RepositoryContracts;

namespace OfficeAssetManager.Tests;

public class ReservationServiceUnitTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock;
    private readonly Mock<IAssetRepository> _assetRepoMock;
    private readonly IReservationService _reservationService;

    public ReservationServiceUnitTests()
    {
        _reservationRepoMock = new Mock<IReservationRepository>();
        _assetRepoMock = new Mock<IAssetRepository>();

        // Inject both mocks into the service
        _reservationService = new ReservationService(
            _reservationRepoMock.Object,
            _assetRepoMock.Object
        );
    }

    [Fact]
    public async Task CreateReservationAsync_ShouldThrowException_WhenAssetNotFound()
    {
        // Arrange
        var dto = new ReservationRequestDto { AssetId = 99 };
        _assetRepoMock.Setup(repo => repo.GetByIdAsync(dto.AssetId))
                      .ReturnsAsync((Asset)null!);

        // Act
        Func<Task> action = async () => await _reservationService.CreateReservationAsync(1, dto);

        // Assert
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Asset not found.");
    }

    [Fact]
    public async Task CreateReservationAsync_ShouldThrowException_WhenDatesOverlap()
    {
        // Arrange
        var dto = new ReservationRequestDto
        {
            AssetId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(2)
        };

        _assetRepoMock.Setup(repo => repo.GetByIdAsync(dto.AssetId))
                      .ReturnsAsync(new Asset { Id = 1, Name = "Laptop" });

        // Simulate that an overlap already exists in the database
        _reservationRepoMock.Setup(repo => repo.HasOverlappingReservationAsync(dto.AssetId, dto.StartDate, dto.EndDate))
                            .ReturnsAsync(true);

        // Act
        Func<Task> action = async () => await _reservationService.CreateReservationAsync(1, dto);

        // Assert
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Asset is already reserved for these dates.");
    }

    [Fact]
    public async Task CreateReservationAsync_ShouldSucceed_WhenAssetIsFree()
    {
        // Arrange
        var userId = 1;
        var dto = new ReservationRequestDto
        {
            AssetId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };

        _assetRepoMock.Setup(repo => repo.GetByIdAsync(dto.AssetId))
                      .ReturnsAsync(new Asset { Id = 1, Name = "Laptop" });

        _reservationRepoMock.Setup(repo => repo.HasOverlappingReservationAsync(dto.AssetId, dto.StartDate, dto.EndDate))
                            .ReturnsAsync(false);

        // Act
        var result = await _reservationService.CreateReservationAsync(userId, dto);

        // Assert
        result.Should().NotBeNull();
        result.AssetName.Should().Be("Laptop");
        _reservationRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Reservation>()), Times.Once);
        _reservationRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }
}