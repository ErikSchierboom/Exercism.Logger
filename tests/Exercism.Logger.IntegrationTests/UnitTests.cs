using Xunit;

namespace Exercism.Logger.IntegrationTests
{
    public class UnitTests
    {
        [Fact]
        public void Should_pass() =>
            Assert.Equal(2, 1 + 1);
        
        [Fact]
        public void Should_fail() =>
            Assert.Equal(4, 1 - 1);
    }
}