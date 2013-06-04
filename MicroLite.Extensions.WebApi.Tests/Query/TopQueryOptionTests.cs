namespace MicroLite.Extensions.WebApi.Tests.Query
{
    using MicroLite.Extensions.WebApi.Query;
    using Xunit;

    public class TopQueryOptionTests
    {
        public class WhenConstructedWithAnInvalidValue
        {
            [Fact]
            public void AnODataExceptionShouldBeThrown()
            {
                Assert.Throws<ODataException>(() => new TopQueryOption("$top=wibble"));
            }
        }

        public class WhenConstructedWithAValidValue
        {
            private readonly TopQueryOption option;
            private readonly string rawValue;

            public WhenConstructedWithAValidValue()
            {
                this.rawValue = "$top=25";
                this.option = new TopQueryOption(this.rawValue);
            }

            [Fact]
            public void TheRawValueShouldEqualTheValuePassedToTheConstructor()
            {
                Assert.Equal(this.rawValue, this.option.RawValue);
            }

            [Fact]
            public void TheTopValueShouldEqualTheValueFromTheRawValue()
            {
                Assert.Equal(25, this.option.Value);
            }
        }
    }
}