using FluentAssertions;
using Moq;
using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.Domain.RepositoryContracts;
using OfficeAssetManager.Core.DTO;
using OfficeAssetManager.Core.ServiceContracts;
using OfficeAssetManager.Core.Services;

namespace OfficeAssetManager.Tests;

public class AssetServiceUnitTests
{
    private readonly Mock<IAssetRepository> _repositoryMock;
    private readonly IAssetService _assetService;

    public AssetServiceUnitTests()
    {
        _repositoryMock = new Mock<IAssetRepository>();
        _assetService = new AssetService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAssetAsync_ShouldThrowException_WhenAssetTagExists()
    {
        // Arrange
        var dto = new AssetCreateDto { AssetTag = "TAG-001" };
        _repositoryMock.Setup(repo => repo.AssetTagExistsAsync(dto.AssetTag))
                       .ReturnsAsync(true);

        // Act
        Func<Task> action = async () => await _assetService.CreateAssetAsync(dto);

        // Assert
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Asset tag already exists.");
    }

    [Fact]
    public async Task GetAssetByIdAsync_ShouldReturnNull_WhenAssetDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                       .ReturnsAsync((Asset)null!);

        // Act
        var result = await _assetService.GetAssetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAssetAsync_ShouldReturnResponseDto_WhenSuccessful()
    {
        // Arrange
        var dto = new AssetCreateDto
        {
            Name = "Laptop",
            AssetTag = "TAG-123",
            SerialNumber = "SN-123"
        };

        _repositoryMock.Setup(repo => repo.AssetTagExistsAsync(dto.AssetTag))
                       .ReturnsAsync(false);

        // Act
        var result = await _assetService.CreateAssetAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.AssetTag.Should().Be(dto.AssetTag);
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Asset>()), Times.Once);
    }
}