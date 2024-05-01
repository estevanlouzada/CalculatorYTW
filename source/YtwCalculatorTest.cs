using System;
using System.Threading.Tasks;
using Xunit;
using Moq;

public class YtwCalculatorTests
{
    [Fact]
    public async Task CalculateYtwForBond_Should_Return_Correct_Ytw()
    {
        // Arrange
        var bond = new Bond { /* Initialize bond properties */ };
        var settlementDate = new DateTime(2024, 5, 1);
        var index = 0.05m; // Example index value for reference value to assert .
        var calculatorMock = new Mock<IImtcCalculator>();
        var indexProviderMock = new Mock<IIndexProvider>();
        var timeServiceMock = new Mock<ITimeService>();

        calculatorMock.Setup(c => c.CalculateYtw(bond, settlementDate, index)).Returns(0.06m);
        indexProviderMock.Setup(i => i.GetIndex(It.IsAny<string>(), settlementDate)).ReturnsAsync(index);

        var ytwCalculator = new YtwCalculator(calculatorMock.Object, indexProviderMock.Object, timeServiceMock.Object);

        // Act
        var result = await ytwCalculator.CalculateYtwForBond(bond, settlementDate);

        // Assert
        Assert.Equal(0.06m, result);
    }

    [Fact]
    public async Task CalculateYtwForBond_Should_Throw_ArgumentNullException_For_Null_Bond()
    {
        // Arrange
        var settlementDate = new DateTime(2024, 5, 1);
        var calculatorMock = new Mock<IImtcCalculator>();
        var indexProviderMock = new Mock<IIndexProvider>();
        var timeServiceMock = new Mock<ITimeService>();

        var ytwCalculator = new YtwCalculator(calculatorMock.Object, indexProviderMock.Object, timeServiceMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => ytwCalculator.CalculateYtwForBond(null, settlementDate));
    }
}