namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using System;
    using System.Net.Http;
    using MicroLite.Extensions.WebApi.OData;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using MicroLite.Query;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query;
    using Xunit;

    public class FilterBinderTests
    {
        private enum CustomerStatus
        {
            New = 0,
            Current = 1,
            Migrated = 2,
            Closed = 3
        }

        [Fact]
        public void BindFilterThrowsODataExceptionForUnspportedFunctionName()
        {
            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=indexof(Name, 'ayes') eq 1"));

            var exception = Assert.Throws<ODataException>(() => FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))));

            Assert.Equal("The function 'indexof' is not supported", exception.Message);
        }

        public class WhenCallingApplyToWithAComplexQuery
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingApplyToWithAComplexQuery()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Created ge datetime'2013-05-01' and Created le datetime'2013-06-12' and Reference eq 'A/0113334' and substringof('Hayes', Name) eq true"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(new DateTime(2013, 5, 1), this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheArgumentsShouldContainTheFourthQueryValue()
            {
                Assert.Equal("%Hayes%", this.sqlQuery.Arguments[3]);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 6, 12), this.sqlQuery.Arguments[1]);
            }

            [Fact]
            public void TheArgumentsShouldContainTheThirdQueryValue()
            {
                Assert.Equal("A/0113334", this.sqlQuery.Arguments[2]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("((Created >= ?) AND ((Created <= ?) AND ((Reference = ?) AND (Name LIKE ?))))", new DateTime(2013, 5, 1), new DateTime(2013, 6, 12), "A/0113334", "%Hayes")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe4ArgumentValues()
            {
                Assert.Equal(4, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithAPropertyEqualsAndGreaterThanAndLessThan
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithAPropertyEqualsAndGreaterThanAndLessThan()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Name eq 'Fred Bloggs' and Created gt datetime'2013-04-01' and Created lt datetime'2013-04-30'"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("Fred Bloggs", this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[1]);
            }

            [Fact]
            public void TheArgumentsShouldContainTheThirdQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 30), this.sqlQuery.Arguments[2]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("((Name = ?) AND ((Created > ?) AND (Created < ?)))", "Fred Bloggs", new DateTime(2013, 4, 1), new DateTime(2013, 4, 30))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe3ArgumentValues()
            {
                Assert.Equal(3, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithAPropertyEqualsAndGreaterThanOrLessThan
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithAPropertyEqualsAndGreaterThanOrLessThan()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Name eq 'Fred Bloggs' and Created gt datetime'2013-04-01' or Created lt datetime'2013-04-30'"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("Fred Bloggs", this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[1]);
            }

            [Fact]
            public void TheArgumentsShouldContainTheThirdQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 30), this.sqlQuery.Arguments[2]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("((Name = ?) AND ((Created > ?) OR (Created < ?)))", "Fred Bloggs", new DateTime(2013, 4, 1), new DateTime(2013, 4, 30))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe3ArgumentValues()
            {
                Assert.Equal(3, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithAPropertyGreaterThanAndLessThan
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithAPropertyGreaterThanAndLessThan()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Created gt datetime'2013-04-01' and Created lt datetime'2013-04-30'"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 30), this.sqlQuery.Arguments[1]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("((Created > ?) AND (Created < ?))", new DateTime(2013, 4, 1), new DateTime(2013, 4, 30))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe2ArgumentValues()
            {
                Assert.Equal(2, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithAPropertyGreaterThanOrLessThan
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithAPropertyGreaterThanOrLessThan()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Created gt datetime'2013-04-01' or Created lt datetime'2013-04-30'"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 30), this.sqlQuery.Arguments[1]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("((Created > ?) OR (Created < ?))", new DateTime(2013, 4, 1), new DateTime(2013, 4, 30))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe2ArgumentValues()
            {
                Assert.Equal(2, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyEndsWith
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyEndsWith()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=endswith(Name, 'Bloggs') eq true"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("%Bloggs", this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name LIKE ?)", "%Bloggs")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyEqual
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyEqual()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Name eq 'Fred Bloggs'"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred Bloggs", this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name = ?)", "Fred Bloggs")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyGreaterThan
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyGreaterThan()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Created gt datetime'2013-04-01'"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Created > ?)", new DateTime(2013, 4, 1))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyGreaterThanOrEqual
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyGreaterThanOrEqual()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Created ge datetime'2013-04-01'"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Created >= ?)", new DateTime(2013, 4, 1))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyLessThan
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyLessThan()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Created lt datetime'2013-04-01'"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Created < ?)", new DateTime(2013, 4, 1))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyLessThanOrEqual
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyLessThanOrEqual()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Created le datetime'2013-04-01'"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Created <= ?)", new DateTime(2013, 4, 1))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyNotEqual
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyNotEqual()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Name ne 'Fred Bloggs'"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred Bloggs", this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name <> ?)", "Fred Bloggs")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyStartsWith
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyStartsWith()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=startswith(Name, 'Fred') eq true"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred%", this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name LIKE ?)", "Fred%")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertySubStringOf
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertySubStringOf()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=substringof(Name, 'Bloggs') eq true"));

                this.sqlQuery = FilterBinder.BindFilter<Customer>(queryOptions.Filter, SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("%Bloggs%", this.sqlQuery.Arguments[0]);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name LIKE ?)", "%Bloggs%")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, this.sqlQuery.Arguments.Count);
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

            public string Reference
            {
                get;
                set;
            }

            public CustomerStatus Status
            {
                get;
                set;
            }
        }
    }
}