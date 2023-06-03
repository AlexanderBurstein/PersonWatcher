using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using personwatcherapi.Data;
using personwatcherapi.Engine;
using personwatcherapi.Extensions;
using personwatcherapi.Models;
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
                    return await _context.Persons.Where(x => x.Birthdate > dateSearch.AddDays(-3) && x.Birthdate < dateSearch.AddDays(3)).OrderBy(x => x.Name)
                        .SelectAsync(async x => await UsefulExtensions.ModifyName(x, contacts, opposites), 1);
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
            return await _context.Persons.Where(x => x.Name.Contains(searchStr)).OrderBy(x => x.Name)
                .SelectAsync(async x => await UsefulExtensions.ModifyName(x, contacts, opposites), 1);


        }
        [HttpGet]
        [Route("Rank")]
        public async Task<IEnumerable<Person>> GetRanksAsync()
        {
            var (contacts, opposites) = Calculator.GetInstance().GetInterestingPoses();
            var distances = _context.Persons.Where(x => x.NextStart < DateTime.Now.AddHours(2))
                .Select(x => new Tuple<int, int>(x.PersonId, x.HowCloseToSunAndMoon(contacts, opposites))).ToList();
            var maxDistance = distances.Max(x => x.Item2);
            var interestingId = distances.Where(x=>x.Item2 == maxDistance).Min(x => x.Item1);
            var persons = await _context.Persons.Where(x=> x.NextStart < DateTime.Now.AddHours(2) &&
                (x.NextStart > DateTime.Now.AddHours(-3) || interestingId == x.PersonId)).ToListAsync<Person>();
            var placeIds = persons.Select(x => x.PlaceId).ToList();
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
            (person.EventPredictability, person.VenusPos, person.UranusPos) 
                = Calculator.GetInstance().SignifyRanking(person, _context.Places.Find(person.PlaceId));
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
            (person.EventPredictability, person.VenusPos, person.UranusPos)
                = Calculator.GetInstance().SignifyRanking(person, _context.Places.Find(person.PlaceId));
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
