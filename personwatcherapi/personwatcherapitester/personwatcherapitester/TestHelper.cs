using System.Text.Json;

namespace personwatcherapitester
{
    internal class TestHelper
    {
        public static void AreEqualByJson(object expected, object actual)
        {
            var expectedJson = JsonSerializer.Serialize(expected);
            var actualJson = JsonSerializer.Serialize(actual);
            Assert.AreEqual(expectedJson, actualJson);
        }
    }
}
