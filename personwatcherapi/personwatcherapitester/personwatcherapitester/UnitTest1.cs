using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using personwatcherapi.Controllers;
using personwatcherapi.Data;
using personwatcherapi.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.Json.Nodes;
using Zahasoft.Extension;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace personwatcherapitester
{
    [TestFixture]
    public class Tests
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        PersonWatcherDbContext _testContext;
        string CONNECTION_STRING = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=personwatcherdb;Integrated Security=True";
        [SetUp]
        public void Setup()
        {
            _semaphore.Wait();
            var optionsBuilder = new DbContextOptionsBuilder<PersonWatcherDbContext>();
            optionsBuilder.UseSqlServer(CONNECTION_STRING);
            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                con.Open(); 
                string truncateQuery = @"TRUNCATE TABLE Persons;
                   DELETE FROM Places;
                    DBCC CHECKIDENT ('Places', RESEED, 0);";
                var cmd = new SqlCommand(truncateQuery, con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            _testContext = new PersonWatcherDbContext(optionsBuilder.Options);
        }

        [Test]
        public void TestBasics()
        {
            PlaceController placeController = new PlaceController(_testContext);
            var places = placeController.GetPlacesAsync();
            Assert.AreEqual(0, places.Result.Count());
            Assert.AreEqual(TaskStatus.RanToCompletion, places.Status);

            PersonController personController = new PersonController(_testContext);
            var people = personController.GetPersonsAsync();
            Assert.AreEqual(0, people.Result.Count());
            Assert.AreEqual(TaskStatus.RanToCompletion, people.Status);
            Assert.Pass();
        }
        [Test]
        public async Task TestOneItemEach()
        {
            PlaceController placeController = new PlaceController(_testContext);
            var thePlace = new Place();
            thePlace.Placename = "Portugal, Boliqueime";
            thePlace.Latitude = "N37.07.59";
            thePlace.Longitude = "W008.09.32";
            _testContext.Places.Add(thePlace);
            _testContext.SaveChanges();
            Assert.AreEqual(1, _testContext.Places.Count());
            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                con.Open();
                string allPersonsQuery = "SELECT COUNT(PlaceId) FROM Places";
                var cmdPlaces = new SqlCommand(allPersonsQuery, con);
                Assert.AreEqual(1, cmdPlaces.ExecuteScalar());
                con.Close();
            }

            var places = await placeController.GetPlacesAsync();
            Assert.AreEqual(0, places.Count());

            places = await placeController.GetPlacesAsync("Bol");
            Assert.AreEqual(1, places.Count());
            TestHelper.AreEqualByJson(thePlace, places.First());

            var placeTaken = placeController.GetByIdAsync(1).Result as OkObjectResult;
            var placeObj = placeTaken.Value as Place;
            TestHelper.AreEqualByJson(thePlace, placeObj);

            PersonController personController = new PersonController(_testContext);
            var thePerson = new Person();
            thePerson.Name = "Simply Me";
            thePerson.Birthdate = DateTime.Now;
            thePerson.EventPredictability = 0;
            thePerson.ExtraInfo = "";
            thePerson.EventType = EventType.TeamLead;
            thePerson.PlaceId = 1;
            thePerson.Place = new Place();
            thePerson.SunPos = 0;
            thePerson.MoonPos = 0;
            thePerson.MercuryPos = 0;
            thePerson.VenusPos = 0;
            thePerson.MarsPos = 0;
            thePerson.JupiterPos = 0;
            thePerson.SaturnPos = 0;
            thePerson.UranusPos = 0;
            thePerson.NeptunePos = 0;

            var addingResult = personController.CreateAsync(thePerson);
            Assert.IsNotNull(addingResult.Result);
            var parsedAddingResult = addingResult.Result as CreatedAtActionResult;
            TestHelper.AreEqualByJson(thePerson, parsedAddingResult.Value);

            var header = personController.GetHeader() as ContentResult;
            header.ShouldNotBeNull();
            header.Content.ShouldNotBeNull();
            var parsedHeader = JObject.Parse(header.Content);
            parsedHeader.ShouldNotBeNull();
            var parsedHeaderDataStr = parsedHeader.Value<string>("data");
            var headerDetails = parsedHeaderDataStr.Split(" f ");
            Assert.AreEqual(11, headerDetails.Length);

            var currentSun = headerDetails[0].Split(" ");
            Assert.AreEqual(3, currentSun.Length);

            var modifiedPerson = parsedAddingResult.Value as Person;
            Assert.AreEqual(currentSun[1], (modifiedPerson.SunPos % 30).ToString());
            
            Assert.Pass();
        }
        [TearDown]
        public void TearDown() {
            _testContext.Dispose();
            _semaphore.Release();
        }
    }
}