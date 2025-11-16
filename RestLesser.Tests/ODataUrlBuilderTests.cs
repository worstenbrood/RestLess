using RestLesser.OData;

namespace RestLesser.Tests;

public class ODataUrlBuilderTests
{
    private class Dummy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] Data { get; set; }
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ODataUrlBuilder_SelectTest()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Select(x => x.Name);

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$select=Name"));
    }

    [Test]
    public void ODataUrlBuilder_SelectTest_Multi()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Select(x => x.Name, x => x.Id);

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$select=Name%2cId"));
    }

    [Test]
    public void ODataUrlBuilder_ExpandTest()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Expand(x => x.Name);

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$expand=Name"));
    }

    [Test]
    public void ODataUrlBuilder_ExpandTest_Multi()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Expand(x => x.Name, x => x.Data);

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$expand=Name%2cData"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_Eq()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Name, c => c.Eq("test"));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=Name+eq+%27test%27"));
    }
}
