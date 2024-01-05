using Microsoft.EntityFrameworkCore;
using Tgs.Promotion.AccountFirstDeposit.Repositories;
using Tgs.Promotion.AccountFirstDeposit.Repositories.Entities;

namespace AccountFirstDeposit.Repositories.UnitTests;

public class TgsRepositoryTests
{
    [Fact]
    public async Task TgsRepository_取得累計發放金額Id()
    {
        // Arrange
        var dbContext = new TgsContext(new DbContextOptionsBuilder<TgsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        var sut = new TgsRepository(dbContext);

        var calculateId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();

        _ = await dbContext.DepositPromotionCalculatePairs.AddAsync(new DepositPromotionCalculatePair
        {
            PromotionId = promotionId,
            CalculateId = calculateId
        });

        _ = await dbContext.SaveChangesAsync();

        // Act
        var actual = await sut.GetPromotionCalculateIdAsync(promotionId);

        // Assert
        Assert.Equal(calculateId, actual);
    }

    [Fact]
    public async Task TgsRepository_設定活動的累計發放金額Id()
    {
        // Arrange
        var dbContext = new TgsContext(new DbContextOptionsBuilder<TgsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        var sut = new TgsRepository(dbContext);

        var calculateId = Guid.NewGuid();
        var promotionId = Guid.NewGuid();

        // Act
        await sut.SetPromotionCalculateIdAsync(promotionId, calculateId);
        var actual = await dbContext.DepositPromotionCalculatePairs.FindAsync(promotionId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(calculateId, actual.CalculateId);
    }
}
