using RankCalculator;

namespace Rank.Tests.RankCalculatorTests;

public class RankCalculatorTests
{
    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Calculate_Rank_With_Different_Inputs_Returns_Expected_Value(string text, string expectedRank)
    {
        // Arrange
        Func<string, string> calculator = (string text) => Program.CalculateRank(text);

        // Act
        string factResult = calculator(text);

        // Assert
        Assert.Equal(expectedRank, factResult);
    }

    public static TheoryData<string, string> GetTestData()
    {
        return new TheoryData<string, string>
        {
            { "a", "0" },
            { "", "0" },
            { "a1", "0,5" },
            { "1", "1" },
            { "&", "1" },
            { "abc👍", "0,25" }
        };
    }

}
