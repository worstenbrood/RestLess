using RestLesser.OData.Attributes;

namespace RestLesser.Tests
{
    public class SingleKey
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string? Name { get; set; }
    }

    public class MultiKey
    {
        [PrimaryKey]
        public int Id1 { get; set; }

        [PrimaryKey]
        public int Id2 { get; set; }

        public string? Name { get; set; }
    }

    public class StringKey
    {
        [PrimaryKey]
        public string? Id { get; set; }
    }

    public class PrimaryKeyTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PrimaryKey_SingleKeyTest()
        {
            int id = Random.Shared.Next();
            var key = new SingleKey
            {
                Id = id,
                Name = "Single Key Test"
            };

            string result = PrimaryKey<SingleKey>.GetValue(key);
            Assert.That(result, Is.EqualTo($"(Id={id})"));
        }


        [Test]
        public void PrimaryKey_MultiKeyTest()
        {
            int id1 = Random.Shared.Next();
            int id2 = Random.Shared.Next();
            var key = new MultiKey
            {
                Id1 = id1,
                Id2 = id2,
                Name = "Multi Key Test"
            };

            string result = PrimaryKey<MultiKey>.GetValue(key);
            Assert.That(result, Is.EqualTo($"(Id1={id1},Id2={id2})"));
        }

        [Test]
        public void PrimaryKey_StringKeyTest()
        {
            var key = new StringKey
            {
                Id = "String Key Test"
            };

            string result = PrimaryKey<StringKey>.GetValue(key);
            Assert.That(result, Is.EqualTo($"(Id='{key.Id}')"));
        }
    }
}
