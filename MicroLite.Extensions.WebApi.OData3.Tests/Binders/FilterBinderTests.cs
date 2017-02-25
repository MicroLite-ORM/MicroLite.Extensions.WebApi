namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using System;
    using System.Net.Http;
    using MicroLite.Builder;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using MicroLite.Extensions.WebApi.Tests.TestEntities;
    using MicroLite.Mapping;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query;
    using Xunit;

    public class FilterBinderTests
    {
        [Fact]
        public void BindFilterThrowsODataExceptionForUnspportedFunctionName()
        {
            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=indexof(Name, 'ayes') eq 1"));

            var exception = Assert.Throws<ODataException>(() => FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))));

            Assert.Equal("The function 'indexof' is not supported", exception.Message);
        }

        [Fact]
        public void BindFilterThrowsODataExceptionForUnspportedPropertyName()
        {
            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=FirstName eq 'Fred'"));

            var exception = Assert.Throws<ODataException>(() => FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))));

            Assert.Equal("The type Customer does not have a property called FirstName", exception.Message);
        }

        public class WhenCallingApplyToWithAComplexQuery
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingApplyToWithAComplexQuery()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Created ge datetime'2013-05-01' and Created le datetime'2013-06-12' and Reference eq 'A/0113334' and startswith(Name, 'Hayes') eq true"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(new DateTime(2013, 5, 1), this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheFourthQueryValue()
            {
                Assert.Equal("Hayes%", this.sqlQuery.Arguments[3].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 6, 12), this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheThirdQueryValue()
            {
                Assert.Equal("A/0113334", this.sqlQuery.Arguments[2].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("((((Created >= ?) AND (Created <= ?)) AND (Reference = ?)) AND (Name LIKE ?))", new DateTime(2013, 5, 1), new DateTime(2013, 6, 12), "A/0113334", "Hayes%")
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

        public class WhenCallingApplyToWithAGroupedFunctionAndFunction
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingApplyToWithAGroupedFunctionAndFunction()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=(endswith(Name, 'son') and endswith(Name, 'nes')))"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("%son", this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal("%nes", this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name LIKE ? AND Name LIKE ?)", "%son", "%nes")
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

        public class WhenCallingApplyToWithAGroupedFunctionOrFunction
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingApplyToWithAGroupedFunctionOrFunction()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=(endswith(Name, 'son') or endswith(Name, 'nes')))"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("%son", this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal("%nes", this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name LIKE ? OR Name LIKE ?)", "%son", "%nes")
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

        public class WhenCallingBindFilterQueryOptionWithAPropertyEqualsAndGreaterThanAndLessThan
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithAPropertyEqualsAndGreaterThanAndLessThan()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Name eq 'Fred Bloggs' and Created gt datetime'2013-04-01' and Created lt datetime'2013-04-30'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("Fred Bloggs", this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheThirdQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 30), this.sqlQuery.Arguments[2].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(((Name = ?) AND (Created > ?)) AND (Created < ?))", "Fred Bloggs", new DateTime(2013, 4, 1), new DateTime(2013, 4, 30))
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

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("Fred Bloggs", this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheThirdQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 30), this.sqlQuery.Arguments[2].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(((Name = ?) AND (Created > ?)) OR (Created < ?))", "Fred Bloggs", new DateTime(2013, 4, 1), new DateTime(2013, 4, 30))
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

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 30), this.sqlQuery.Arguments[1].Value);
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

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 30), this.sqlQuery.Arguments[1].Value);
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

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyCeiling
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyCeiling()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=ceiling(Id) eq 32"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(32, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(CEILING(Id) = ?)", 32)
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

#if !ODATA3

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyContains
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyContains()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=contains(Name, 'Bloggs')"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("%Bloggs%", this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("Name LIKE ?", "%Bloggs%")
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

#endif

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyDay
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyDay()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=day(DateOfBirth) eq 22"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(22, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(DAY(DateOfBirth) = ?)", 22)
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

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyEndsWith
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyEndsWith()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=endswith(Name, 'Bloggs')"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("%Bloggs", this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("Name LIKE ?", "%Bloggs")
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

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyEndsWithEqTrue
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyEndsWithEqTrue()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=endswith(Name, 'Bloggs') eq true"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("%Bloggs", this.sqlQuery.Arguments[0].Value);
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

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred Bloggs", this.sqlQuery.Arguments[0].Value);
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

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyEqualNull
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyEqualNull()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Name eq null"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("Name").IsNull()
                    .ToSqlQuery()
                    .CommandText
                    .Replace("(", "((").Replace(")", "))"); // HACK - Add an extra set of parenthesis, it's an unnecessary bug in FilterBuilder which hasn't been fixed yet.

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBeNoArgumentValues()
            {
                Assert.Equal(0, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyFloor
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyFloor()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=floor(Id) eq 32"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(32, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(FLOOR(Id) = ?)", 32)
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

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[0].Value);
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

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[0].Value);
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

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[0].Value);
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

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), this.sqlQuery.Arguments[0].Value);
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

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyMonth
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyMonth()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=month(DateOfBirth) eq 6"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(6, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(MONTH(DateOfBirth) = ?)", 6)
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

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred Bloggs", this.sqlQuery.Arguments[0].Value);
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

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyNotEqualNull
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyNotEqualNull()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=Name ne null"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("Name").IsNotNull()
                    .ToSqlQuery()
                    .CommandText
                    .Replace("(", "((").Replace(")", "))"); // HACK - Add an extra set of parenthesis, it's an unnecessary bug in FilterBuilder which hasn't been fixed yet.

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBeNoArgumentValues()
            {
                Assert.Equal(0, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyReplace
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyReplace()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=replace(Name, ' ', '') eq 'JohnSmith'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void ArgumentOneShouldBeTheReplacementValue()
            {
                Assert.Equal("", this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void ArgumentTwoShouldBeTheValueToFind()
            {
                Assert.Equal("JohnSmith", this.sqlQuery.Arguments[2].Value);
            }

            [Fact]
            public void ArgumentZeroShouldBeTheValueToBeReplaced()
            {
                Assert.Equal(" ", this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(REPLACE(Name, ?, ?) = ?)", "JohnSmith")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe3ArgumentValue()
            {
                Assert.Equal(3, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyRound
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyRound()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=round(Id) eq 32"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(32, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(ROUND(Id) = ?)", 32)
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
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=startswith(Name, 'Fred')"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred%", this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("Name LIKE ?", "Fred%")
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

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyStartsWithEqTrue
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyStartsWithEqTrue()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=startswith(Name, 'Fred') eq true"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred%", this.sqlQuery.Arguments[0].Value);
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

#if ODATA3
        public class WhenCallingBindFilterQueryOptionWithASinglePropertySubStringOf
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertySubStringOf()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=substringof('Bloggs', Name) eq true"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("%Bloggs%", this.sqlQuery.Arguments[0].Value);
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
#endif

        public class WhenCallingBindFilterQueryOptionWithASinglePropertySubStringWithStartAndLength
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertySubStringWithStartAndLength()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=substring(Name, 1, 2) eq 'oh'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void ArgumentOneShouldBeTheValueToBeLength()
            {
                Assert.Equal(2, this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void ArgumentTwoShouldBeTheValueToFind()
            {
                Assert.Equal("oh", this.sqlQuery.Arguments[2].Value);
            }

            [Fact]
            public void ArgumentZeroShouldBeTheValueToBeStartIndex()
            {
                Assert.Equal(1, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(SUBSTRING(Name, ?, ?) = ?)", 1, 2, "oh")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe3ArgumentValue()
            {
                Assert.Equal(3, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertySubStringWithStartOnly
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertySubStringWithStartOnly()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=substring(Name, 1) eq 'ohnSmith'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void ArgumentOneShouldBeTheValueToFind()
            {
                Assert.Equal("ohnSmith", this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void ArgumentZeroShouldBeTheValueToBeStartIndex()
            {
                Assert.Equal(1, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(SUBSTRING(Name, ?) = ?)", 1, "ohnSmith")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, this.sqlQuery.CommandText);
            }

            [Fact]
            public void ThereShouldBe2ArgumentValue()
            {
                Assert.Equal(2, this.sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyToLower
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyToLower()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=tolower(Name) eq 'fred bloggs'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("fred bloggs", this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(LOWER(Name) = ?)", "fred bloggs")
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

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyToUpper
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyToUpper()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=toupper(Name) eq 'FRED BLOGGS'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("FRED BLOGGS", this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(UPPER(Name) = ?)", "FRED BLOGGS")
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

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyTrim
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyTrim()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=trim(Name) eq 'FRED BLOGGS'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("FRED BLOGGS", this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(LTRIM(RTRIM(Name)) = ?)", "FRED BLOGGS")
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

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyYear
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyYear()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=year(DateOfBirth) eq 1971"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(1971, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(YEAR(DateOfBirth) = ?)", 1971)
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

        public class WhenCallingBindFilterQueryOptionWithNotSinglePropertyEqual
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithNotSinglePropertyEqual()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$filter=not Name eq 'Fred Bloggs'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred Bloggs", this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("NOT (Name = ?)", "Fred Bloggs")
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

        public class WhenCallingBindFilterQueryOptionWithPropertyAddValueEquals
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithPropertyAddValueEquals()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Invoices?$filter=Quantity add 10 eq 15'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Invoice)), SqlBuilder.Select("*").From(typeof(Invoice))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(10, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(15, this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Invoice))
                    .Where("((Quantity + ?) = ?)", 10, 15)
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

        public class WhenCallingBindFilterQueryOptionWithPropertyDivideValueEquals
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithPropertyDivideValueEquals()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Invoices?$filter=Quantity div 10 eq 15'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Invoice)), SqlBuilder.Select("*").From(typeof(Invoice))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(10, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(15, this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Invoice))
                    .Where("((Quantity / ?) = ?)", 10, 15)
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

        public class WhenCallingBindFilterQueryOptionWithPropertyModuloValueEquals
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithPropertyModuloValueEquals()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Invoices?$filter=Quantity mod 10 eq 15'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Invoice)), SqlBuilder.Select("*").From(typeof(Invoice))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(10, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(15, this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Invoice))
                    .Where("((Quantity % ?) = ?)", 10, 15)
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

        public class WhenCallingBindFilterQueryOptionWithPropertyMultiplyValueEquals
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithPropertyMultiplyValueEquals()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Invoices?$filter=Quantity mul 10 eq 15'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Invoice)), SqlBuilder.Select("*").From(typeof(Invoice))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(10, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(15, this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Invoice))
                    .Where("((Quantity * ?) = ?)", 10, 15)
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

        public class WhenCallingBindFilterQueryOptionWithPropertySubtractValueEquals
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindFilterQueryOptionWithPropertySubtractValueEquals()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Invoices?$filter=Quantity sub 10 eq 15'"));

                this.sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Invoice)), SqlBuilder.Select("*").From(typeof(Invoice))).ToSqlQuery();
            }

            [Fact]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(10, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(15, this.sqlQuery.Arguments[1].Value);
            }

            [Fact]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                var expected = SqlBuilder.Select("*")
                    .From(typeof(Invoice))
                    .Where("((Quantity - ?) = ?)", 10, 15)
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
    }
}