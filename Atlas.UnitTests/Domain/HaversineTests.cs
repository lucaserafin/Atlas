using Atlas.Api.Application.Factories;
using FluentAssertions;
using Xunit;

namespace Atlas.UnitTests.Domain
{
    public class HaversineTests
    {
        [Fact]
        public void Calculate_ShouldReturnCorrectDistance()
        {
            // Arrange
            double lat1 = 1;
            double lon1 = 1;
            double lat2 = 0;
            double lon2 = 0;

            // Act
            double distance = Haversine.Calculate(lat1, lon1, lat2, lon2);

            // Assert
            distance.Should().BeApproximately(157.29, 0.1); // Expected distance in kilometers
        }

        [Fact]
        public void Calculate_ShouldReturnZeroForSamePoints()
        {
            // Arrange
            double lat1 = 0.0;
            double lon1 = 0.0;
            double lat2 = 0.0;
            double lon2 = 0.0;

            // Act
            double distance = Haversine.Calculate(lat1, lon1, lat2, lon2);

            // Assert
            distance.Should().Be(0.0);
        }

        [Fact]
        public void Calculate_ShouldReturnCorrectDistanceForNegativeCoordinates()
        {
            // Arrange
            double lat1 = -1;
            double lon1 = -1;
            double lat2 = 0;
            double lon2 = 0;

            // Act
            double distance = Haversine.Calculate(lat1, lon1, lat2, lon2);

            // Assert
            distance.Should().BeApproximately(157.29, 0.1); // Expected distance in kilometers
        }
    }
}
