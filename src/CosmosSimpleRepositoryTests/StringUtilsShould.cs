using CosmosSimpleRepository;
using System.Collections.Generic;
using Xunit;

namespace CosmosSimpleRepositoryTests
{
    public class StringUtilsShould
    {
        [Fact]
        public void ConvertConnectionStringToEmptyDictionary()
        {
            var connectionString = "alsdjflksdjflksdjlfk";

            var expected = new Dictionary<string, string>();

            IStringUtils sut = new StringUtils();

            var result = sut.ConvertConnectionString(connectionString);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertConnectionStringToDictionary()
        {
            var connectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

            var expected = new Dictionary<string, string>
            {
                {"AccountEndpoint", "https://localhost:8081/"},
                {"AccountKey", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="}
            };

            IStringUtils sut = new StringUtils();

            var result = sut.ConvertConnectionString(connectionString);

            Assert.Equal(expected, result);
        }
    }
}
