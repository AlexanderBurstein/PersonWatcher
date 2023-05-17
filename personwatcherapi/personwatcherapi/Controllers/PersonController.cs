using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personwatcherapi.Data;
using personwatcherapi.Engine;
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
        public async Task<IEnumerable<Person>> GetPersons(string searchStr = "", string dateStr = "")
        {
            if (!string.IsNullOrEmpty(dateStr))
            {
                try
                {
                    var dateSearch = DateTime.Parse(dateStr);
                    return await _context.Persons.Where(x => x.Birthdate > dateSearch.AddDays(-3) && x.Birthdate < dateSearch.AddDays(3)).OrderBy(x => x.Name).ToListAsync<Person>();
                }
                catch (FormatException fe)
                {
                    Console.WriteLine(fe.Message);
                }
            }
            return await _context.Persons.Where(x => x.Name.Contains(searchStr)).OrderBy(x => x.Name).ToListAsync<Person>();
        }
        [HttpGet]
        [Route("Rank")]
        public async Task<IEnumerable<Person>> GetRanks()
        {
            var interestPosList = Calculator.GetInstance().GetCurrentPositionInterests();
            var persons = await _context.Persons.Where(x=> x.NextStart > DateTime.Now.AddHours(-2) 
            && x.NextStart < DateTime.Now.AddHours(13)).ToListAsync<Person>();
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
        public async Task<IActionResult> Create(Person person)
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
        public async Task<IActionResult> Update(int id, Person person)
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
