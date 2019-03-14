using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using AutoFixture;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Microsoft.Azure.Documents.Client;
using CosmosSimpleRepository;

namespace CosmosSimpleRepositoryTests
{
    public class RepositoryShould : IDisposable
    {
        readonly IDocumentAdapter _doc;
        readonly IRepository<Car> _carRepository;

        public RepositoryShould()
        {
            _doc = GetDocumentAdapter();

            IDbInitializer dbInitializer = new DbInitializer(_doc, new string[] { "Cars", "Persons" });

            dbInitializer.InitializeAsync().Wait();

            _carRepository = new CarRepository(_doc);
        }

        class CarRepository : Repository<Car>
        {
            public CarRepository(IDocumentAdapter documentAdapter) : base("Cars", documentAdapter) { }
        }

        IDocumentAdapter GetDocumentAdapter()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = configurationBuilder.Build();

            var cosmosDbConnection = config.GetSection("cosmosDb:connectionString");
            var cosmosDbId = config.GetSection("cosmosDb:databaseId");

            var stringUtils = new StringUtils();

            IDocumentAdapter doc = new DocumentAdapter(cosmosDbConnection.Value, cosmosDbId.Value, stringUtils);

            return doc;
        }

        public class Car
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("make")]
            public Makes Make { get; set; }
            [JsonProperty("produced")]
            public DateTime Produced { get; set; }
            [JsonProperty("model")]
            public CarModel Model { get; set; }
            [JsonProperty("owners")]
            public IEnumerable<CarOwners> Owners { get; set; }
            [JsonProperty("writtenOff")]
            public bool WrittenOff { get; set; }

            public enum Makes
            {
                Ford = 1,
                Honda = 2,
                Fiat = 3
            }
        }

        public class CarModel
        {
            [JsonProperty("model")]
            public string Model { get; set; }
            [JsonProperty("body")]
            public string Body { get; set; }
            [JsonProperty("year")]
            public int Year { get; set; }
        }

        public class CarOwners
        {
            [JsonProperty("personId")]
            public string PersonId { get; set; }
            [JsonProperty("purchaseId")]
            public DateTime Purchased { get; set; }
        }

        public class Person
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("addresses")]
            public IEnumerable<string> Addresses { get; set; }
        }

        [Fact]
        public async Task CreateItemInDocumentDb()
        {
            var fixture = new Fixture();

            var carFixture = fixture.Build<Car>()
                .Without(x => x.Id)
                .Create();

            var result = await _carRepository.CreateItemAsync(carFixture);

            carFixture.Id = result.Id;

            Assert.NotNull(result.Id);
            Assert.Equal(JsonConvert.SerializeObject(carFixture), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public async Task GetItemFromDocumentDb()
        {
            var fixture = new Fixture();

            var carFixture = fixture.Build<Car>()
                .Without(x => x.Id)
                .Create();

            var expected = await _carRepository.CreateItemAsync(carFixture);

            var actual = await _carRepository.GetItemAsync(expected.Id);

            Assert.NotNull(actual);
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }

        [Fact]
        public async Task GetItemsFromDocumentDb()
        {
            var fixture = new Fixture();

            var carsFixture = fixture.Build<Car>()
                .Without(x => x.Id)
                .CreateMany(5);

            var cars = new List<Car>();

            foreach (var car in carsFixture)
            {
                cars.Add(await _carRepository.CreateItemAsync(car));
            }

            Expression<Func<Car, bool>> filter = f => f.WrittenOff && f.Make == Car.Makes.Ford;

            var actual = await _carRepository.GetItemsAsync(filter);

            var expected = cars.Where(filter.Compile());

            Assert.NotNull(actual);
            Assert.Equal(JsonConvert.SerializeObject(expected.OrderBy(x => x.Id)), JsonConvert.SerializeObject(actual.OrderBy(x => x.Id)));
        }

        public void Dispose()
        {
            _doc.Client.DeleteDatabaseAsync(
                UriFactory.CreateDatabaseUri(_doc.DatabaseId)).Wait();
        }
    }
}
