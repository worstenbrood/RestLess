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
    public void ODataUrlBuilder_TopTest()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Top(5);

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$top=5"));
    }

    [Test]
    public void ODataUrlBuilder_OrderByTest()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .OrderBy(x => x.Id);

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$orderby=Id"));
    }

    [Test]
    public void ODataUrlBuilder_OrderByDescendingTest()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .OrderByDescending(x => x.Id);

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$orderby=Id+desc"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_Eq()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Name, c => c.Eq("test"));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=Name+eq+%27test%27"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_Gt()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Id, c => c.Gt(5));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=Id+gt+5"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_Lt()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Id, c => c.Lt(5));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=Id+lt+5"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_Le()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Id, c => c.Le(5));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=Id+le+5"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_Ge()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Id, c => c.Ge(5));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=Id+ge+5"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_Ne()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Id, c => c.Ne(5));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=Id+ne+5"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_StartsWith()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Name, c => c.StartsWith("test"));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=startswith(Name%2c%27test%27)"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_EndsWith()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Name, c => c.EndsWith("test"));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=endswith(Name%2c%27test%27)"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_Contains()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Name, c => c.Contains("test"));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=contains(Name%2c%27test%27)"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_ToLower()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Name, c => c.ToLower(d => d.Eq("test")));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=tolower(Name)+eq+%27test%27"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_ToUpper()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Name, c => c.ToUpper(d => d.Eq("test")));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=toupper(Name)+eq+%27test%27"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_Substring()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Name, c => c.Substring(5, d => d.Eq("test")));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=substring(Name%2c5)+eq+%27test%27"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_In()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Id, c => c.In(1, 2, 3));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=Id+in+(1%2c2%2c3)"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_And()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Id, c => c.Gt(1))
            .And()
            .Filter(x => x.Id, c => c.Lt(9));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=Id+gt+1+and+Id+lt+9"));
    }

    [Test]
    public void ODataUrlBuilder_FilterTest_Or()
    {
        var query = new ODataUrlBuilder<Dummy>("/dummy")
            .Filter(x => x.Id, c => c.Gt(1))
            .Or()
            .Filter(x => x.Id, c => c.Lt(9));

        Assert.That(query.ToString(), Is.EqualTo("/dummy?$filter=Id+gt+1+or+Id+lt+9"));
    }
}
