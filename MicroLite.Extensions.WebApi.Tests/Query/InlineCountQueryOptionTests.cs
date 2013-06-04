namespace MicroLite.Extensions.WebApi.Tests.Query
{
    using MicroLite.Extensions.WebApi.Query;
    using Xunit;

    public class InlineCountQueryOptionTests
    {
        public class WhenConstructedWithAnInvalidValue
        {
            [Fact]
            public void AnODataExceptionShouldBeThrown()
            {
                Assert.Throws<ODataException>(() => new InlineCountQueryOption("$inlinecount=wibble"));
            }
        }

        public class WhenConstructedWithInlineCountAllPages
        {
            private readonly InlineCountQueryOption option;
            private readonly string rawValue;

            public WhenConstructedWithInlineCountAllPages()
            {
                this.rawValue = "$inlinecount=allpages";
                this.option = new InlineCountQueryOption(this.rawValue);
            }

            [Fact]
            public void TheInlineCountShouldEqualAllPages()
            {
                Assert.Equal(InlineCount.AllPages, this.option.InlineCount);
            }

            [Fact]
            public void TheRawValueShouldEqualTheValuePassedToTheConstructor()
            {
                Assert.Equal(this.rawValue, this.option.RawValue);
            }
        }

        public class WhenConstructedWithInlineCountNone
        {
            private readonly InlineCountQueryOption option;
            private readonly string rawValue;

            public WhenConstructedWithInlineCountNone()
            {
                this.rawValue = "$inlinecount=none";
                this.option = new InlineCountQueryOption(this.rawValue);
            }

            [Fact]
            public void TheInlineCountShouldEqualNone()
            {
                Assert.Equal(InlineCount.None, this.option.InlineCount);
            }

            [Fact]
            public void TheRawValueShouldEqualTheValuePassedToTheConstructor()
            {
                Assert.Equal(this.rawValue, this.option.RawValue);
            }
        }
    }
}