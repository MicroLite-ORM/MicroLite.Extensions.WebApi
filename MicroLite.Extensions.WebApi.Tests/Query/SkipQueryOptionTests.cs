namespace MicroLite.Extensions.WebApi.Tests.Query
{
    using MicroLite.Extensions.WebApi.Query;
    using Xunit;

    public class SkipQueryOptionTests
    {
        public class WhenConstructedWithAnInvalidValue
        {
            [Fact]
            public void AnODataExceptionShouldBeThrown()
            {
                Assert.Throws<ODataException>(() => new SkipQueryOption("$skip=wibble"));
            }
        }

        public class WhenConstructedWithAValidValue
        {
            private readonly SkipQueryOption option;
            private readonly string rawValue;

            public WhenConstructedWithAValidValue()
            {
                this.rawValue = "$skip=25";
                this.option = new SkipQueryOption(this.rawValue);
            }

            [Fact]
            public void TheRawValueShouldEqualTheValuePassedToTheConstructor()
            {
                Assert.Equal(this.rawValue, this.option.RawValue);
            }

            [Fact]
            public void TheSkipValueShouldEqualTheValueFromTheRawValue()
            {
                Assert.Equal(25, this.option.Value);
            }
        }

        public class WhenConstructedWithAValueBelowZero
        {
            [Fact]
            public void AnODataExceptionShouldBeThrown()
            {
                var exception = Assert.Throws<ODataException>(() => new SkipQueryOption("$skip=-1"));
                Assert.Equal(Messages.SkipRawValueInvalid, exception.Message);
            }
        }
    }
}