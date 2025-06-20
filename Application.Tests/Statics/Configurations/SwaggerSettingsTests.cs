using Application.Statics.Configurations;

namespace Application.Tests.Statics.Configurations
{
    [TestFixture]
    public class SwaggerSettingsTests
    {
        [Test]
        public void IsSwaggerEnabled_WhenSet_ShouldReturnTrue()
        {
            // Arrange
            SwaggerSettings.IsSwaggerEnabled = true;

            // Act
            var result = SwaggerSettings.IsSwaggerEnabled;

            // Assert
            Assert.That(result, Is.True);
        }
    }
}
