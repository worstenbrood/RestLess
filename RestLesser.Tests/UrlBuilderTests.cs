namespace RestLesser.Tests
{
    public class UrlBuilderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void UrlBuilder_SetParameterTest()
        {
            var builder = new UrlBuilder("/api/test");
            builder.SetQueryParameter("$filter", "ID eq guid'00000000-0000-0000-0000-000000000000'");
            Assert.That(builder.ToString(), Is.EqualTo("/api/test?$filter=ID+eq+guid%2700000000-0000-0000-0000-000000000000%27"));
        }

        [Test]
        public void UrlBuilder_SetParameterTestWithQuery()
        {
            var builder = new UrlBuilder("/api/test?param=value");
            builder.SetQueryParameter("$filter", "ID eq guid'00000000-0000-0000-0000-000000000000'");
            Assert.That(builder.ToString(), Is.EqualTo("/api/test?param=value&$filter=ID+eq+guid%2700000000-0000-0000-0000-000000000000%27"));
        }

        [Test]
        public void UrlBuilder_SetParametersTest()
        {
            var builder = new UrlBuilder("/api/test");
            builder.SetQueryParameters(new Dictionary<string, string>
            {
                { "$select", "ID" },
                { "$filter", "ID eq guid'00000000-0000-0000-0000-000000000000'" }
            });
            Assert.That(builder.ToString(), Is.EqualTo("/api/test?$select=ID&$filter=ID+eq+guid%2700000000-0000-0000-0000-000000000000%27"));
        }

        [Test]
        public void UrlBuilder_SetParametersTestWithQuery()
        {
            var builder = new UrlBuilder("/api/test?param=value");
            builder.SetQueryParameters(new Dictionary<string, string>
            {
                { "$select", "ID" },
                { "$filter", "ID eq guid'00000000-0000-0000-0000-000000000000'" }
            });
            Assert.That(builder.ToString(), Is.EqualTo("/api/test?param=value&$select=ID&$filter=ID+eq+guid%2700000000-0000-0000-0000-000000000000%27"));
        }
    }
}