using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personwatcherapi.Data;
using personwatcherapi.Models;
using System.Collections;

namespace personwatcherapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly PersonWatcherDbContext _context;
        public PlaceController(PersonWatcherDbContext context) => _context = context;
        [HttpGet]
        public async Task<IEnumerable<Place>> GetPlaces(string searchStr = "")
        {
            if (string.IsNullOrWhiteSpace(searchStr) || searchStr.Length < 3) 
            {
                return new List<Place>();
            }
            return await _context.Places.Where(x => x.Placename.Contains(searchStr)).ToListAsync<Place>();
        }
        [HttpGet("id")]
        [ProducesResponseType(typeof(Place), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int placeId)
        {
            var place = await _context.Places.FindAsync(placeId);
            return place == null ? NotFound() : Ok(place);
        }
    }
}
