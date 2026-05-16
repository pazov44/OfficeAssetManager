using FluentAssertions;
using Moq;
using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Core.Domain.RepositoryContracts;
using OfficeAssetManager.Core.ServiceContracts;
using OfficeAssetManager.Core.Services;

namespace OfficeAssetManager.Tests;

public class AssetLogServiceUnitTests
{
    private readonly Mock<IAssetLogRepository> _logRepoMock;
    private readonly IAssetLogService _logService;

    public AssetLogServiceUnitTests()
    {
        _logRepoMock = new Mock<IAssetLogRepository>();
        _logService = new AssetLogService(_logRepoMock.Object);
    }

    [Fact]
    public async Task RecordLogAsync_ShouldCallRepository_WithCorrectData()
    {
        // Arrange
        int assetId = 10;
        string action = "Update";
        string details = "Changed status";
        string performer = "admin@office.com";

        // Act
        await _logService.RecordLogAsync(assetId, action, details, performer);

        // Assert
        _logRepoMock.Verify(repo => repo.AddAsync(It.Is<AssetLog>(l =>
            l.AssetId == assetId &&
            l.Action == action &&
            l.Details == details &&
            l.PerformedBy == performer
        )), Times.Once);

        _logRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLogsByAssetIdAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        int assetId = 1;
        var logs = new List<AssetLog>
        {
            new AssetLog { Id = 1, AssetId = assetId, Action = "Create", CreatedAt = DateTime.UtcNow },
            new AssetLog { Id = 2, AssetId = assetId, Action = "Edit", CreatedAt = DateTime.UtcNow }
        };

        _logRepoMock.Setup(repo => repo.GetLogsByAssetIdAsync(assetId))
                    .ReturnsAsync(logs);

        // Act
        var result = await _logService.GetLogsByAssetIdAsync(assetId);

        // Assert
        result.Should().HaveCount(2);
        result.First().Action.Should().Be("Create");
        _logRepoMock.Verify(repo => repo.GetLogsByAssetIdAsync(assetId), Times.Once);
    }
}