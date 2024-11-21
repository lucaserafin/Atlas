using Atlas.Api.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.UnitTests.Domain
{
    public class PointOfInterestTests
    {
        [Fact]
        public void Should_create_poi()
        {
            // Arrange
            var name = "test";
            var description = "test";
            var location = new Point(1, 1);

            // Act
            var poi = new PointOfInterest(name, description, location);

            // Assert
            poi.Should().NotBeNull();
            poi.Name.Should().Be(name);
            poi.Description.Should().Be(description);
            poi.Id.Should().Be(0);
            poi.Location.Should().Be(location);
            poi.CreatedAt.Should().BeBefore(DateTime.Now);
            poi.CreatedAt.Should().BeAfter(DateTime.Now.AddMinutes(-1));
        }
    }
}
