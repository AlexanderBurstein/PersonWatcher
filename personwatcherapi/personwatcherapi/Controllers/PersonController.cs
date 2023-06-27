using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using personwatcherapi.Data;
using personwatcherapi.Engine;
using personwatcherapi.Extensions;
using personwatcherapi.Models;
using System;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;

namespace personwatcherapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly PersonWatcherDbContext _context;
        public PersonController(PersonWatcherDbContext context) => _context = context;
        [HttpGet]
        public async Task<IEnumerable<Person>> GetPersonsAsync(string searchStr = "", string dateStr = "")
        {
            var (contacts, opposites) = Calculator.GetInstance().GetInterestingPoses();
            if (!string.IsNullOrEmpty(dateStr))
            {
                try
                {
                    var dateSearch = DateTime.Parse(dateStr);
                    var thePerson = _context.Persons.Where(x => x.Birthdate <= dateSearch && x.Birthdate >= dateSearch.AddHours(-1)).AsEnumerable().MaxBy(x => x.NextStart);
                    var theSign = Calculator.GetInstance().SignifyRanking(thePerson, _context.Places.Find(thePerson.PlaceId));
                    var todayPersons = _context.Persons.Where(x => x.NextStart > DateTime.Now.AddDays(-1) && x.NextStart < DateTime.Now.AddHours(1)).ToList();
                    var idsToShow = new List<int>();
                    foreach (var person in todayPersons)
                    { 
                        var curSign = Calculator.GetInstance().SignifyRanking(person, _context.Places.Find(person.PlaceId));
                        if (curSign == theSign)
                            idsToShow.Add(person.PersonId);
                    }
                    return await _context.Persons.Where(x => idsToShow.Contains(x.PersonId)).ToListAsync();
                }
                catch (FormatException fe)
                {
                    Console.WriteLine(fe.Message);
                }
            }

            if (string.IsNullOrWhiteSpace(searchStr) || searchStr.Length < 3)
            {
                return new List<Person>();
            }
            return await _context.Persons.Where(x => x.Name.Contains(searchStr)).OrderBy(x => x.Name).ToListAsync();


        }
        [HttpGet]
        [Route("Rank")]
        public async Task<IEnumerable<Person>> GetRanksAsync()
        {
            var (contacts, opposites) = Calculator.GetInstance().GetInterestingPoses();
            var placeIds = _context.Persons.Where(x => x.NextStart < DateTime.Now.AddHours(1) && x.NextStart > DateTime.Now.AddMinutes(-118))
                .Select(x => x.PlaceId).Distinct().ToList();
            var persons = await _context.Persons.Where(x => x.NextStart < DateTime.Now.AddHours(1) && x.NextStart > DateTime.Now.AddMinutes(-118)).ToListAsync();
            return Calculator.GetInstance().Ranks(persons, _context.Places.Where(x=>placeIds.Contains(x.PlaceId)));
        }
        [HttpGet("id")]
        [ProducesResponseType(typeof(Person), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int personId)
        {
            var person = await _context.Persons.FindAsync(personId);
            return person == null ? NotFound() : Ok(person);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAsync(Person person)
        {
            Calculator.GetInstance().TransformPerson(ref person);
            person.Place = null;
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();
            person.ExtraInfo = Calculator.GetInstance().SignifyRanking(person, _context.Places.Find(person.PlaceId));
            return CreatedAtAction(nameof(GetById), new {personId = person.PersonId}, person);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAsync(int id, Person person)
        {
            if (id != person.PersonId) return BadRequest();

            Calculator.GetInstance().TransformPerson(ref person);
            person.Place = null;
            _context.Entry(person).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            person.ExtraInfo = Calculator.GetInstance().SignifyRanking(person, _context.Places.Find(person.PlaceId));
            return CreatedAtAction(nameof(GetById), new { personId = person.PersonId }, person); 
        }
        [HttpGet]
        [Route("Header")]
        public IActionResult GetHeader()
        {
            return Content("{\"data\": \"" + Calculator.GetInstance().GetHeader() + "\"}");
        }
    }
}
