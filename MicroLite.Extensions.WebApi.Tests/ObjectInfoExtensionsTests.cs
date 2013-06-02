namespace MicroLite.Extensions.WebApi.Tests
{
    using System;
    using MicroLite.Mapping;
    using Xunit;

    public class ObjectInfoExtensionsTests
    {
        public class WhenCallingMapWithIncludeIdFalse
        {
            private readonly Customer destination = new Customer();
            private readonly IObjectInfo objectInfo = ObjectInfo.For(typeof(Customer));

            private readonly Customer source = new Customer
            {
                Created = DateTime.Now,
                Id = 12442,
                Name = "John Smith"
            };

            public WhenCallingMapWithIncludeIdFalse()
            {
                this.objectInfo.Map(this.source, this.destination, includeId: false);
            }

            [Fact]
            public void TheCreatedShouldBeCoppied()
            {
                Assert.Equal(this.source.Created, this.destination.Created);
            }

            [Fact]
            public void TheIdentifierShouldNotBeCoppied()
            {
                Assert.NotEqual(this.source.Id, this.destination.Id);
            }

            [Fact]
            public void TheNameShouldBeCoppied()
            {
                Assert.Equal(this.source.Name, this.destination.Name);
            }
        }

        public class WhenCallingMapWithIncludeIdTrue
        {
            private readonly Customer destination = new Customer();
            private readonly IObjectInfo objectInfo = ObjectInfo.For(typeof(Customer));

            private readonly Customer source = new Customer
            {
                Created = DateTime.Now,
                Id = 12442,
                Name = "John Smith"
            };

            public WhenCallingMapWithIncludeIdTrue()
            {
                this.objectInfo.Map(this.source, this.destination, includeId: true);
            }

            [Fact]
            public void TheCreatedShouldBeCoppied()
            {
                Assert.Equal(this.source.Created, this.destination.Created);
            }

            [Fact]
            public void TheIdentifierShouldBeCoppied()
            {
                Assert.Equal(this.source.Id, this.destination.Id);
            }

            [Fact]
            public void TheNameShouldBeCoppied()
            {
                Assert.Equal(this.source.Name, this.destination.Name);
            }
        }

        private class Customer
        {
            public DateTime Created
            {
                get;
                set;
            }

            public int Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
        }
    }
}