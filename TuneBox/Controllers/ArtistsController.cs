using Microsoft.AspNetCore.Mvc;
using TuneBox.DTOs;
using TuneBox.Services;

namespace TuneBox.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistsController : ControllerBase
    {
        private readonly ArtistService _artistService;

        public ArtistsController(ArtistService artistService)
        {
            _artistService = artistService;
        }

        // GET: api/artists
        [HttpGet]
        public ActionResult<IEnumerable<ArtistResponseDto>> GetAllArtists()
        {
            return Ok(_artistService.GetAllArtists());
        }

        // GET: api/artists/{id}
        [HttpGet("{id}")]
        public ActionResult<ArtistResponseDto> GetArtistById(int id)
        {
            var artist = _artistService.GetArtistById(id);
            if (artist == null)
            {
                return NotFound($"Artist with ID {id} not found.");
            }

            return Ok(artist);
        }

        // POST: api/artists
        [HttpPost]
        public ActionResult<ArtistResponseDto> CreateArtist([FromBody] ArtistCreateUpdateDto newArtist)
        {
            var createdArtist = _artistService.CreateArtist(newArtist);
            return CreatedAtAction(nameof(GetArtistById), new { id = createdArtist.Id }, createdArtist);
        }

        // PUT: api/artists/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateArtist(int id, [FromBody] ArtistCreateUpdateDto updatedArtist)
        {
            var isUpdated = _artistService.UpdateArtist(id, updatedArtist);
            if (!isUpdated)
            {
                return NotFound($"Artist with ID {id} not found.");
            }

            return NoContent();
        }

        // DELETE: api/artists/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteArtist(int id)
        {
            var isDeleted = _artistService.DeleteArtist(id);
            if (!isDeleted)
            {
                return NotFound($"Artist with ID {id} not found.");
            }

            return NoContent();
        }

        // DELETE: api/artists
        [HttpDelete]
        public IActionResult DeleteAllArtists()
        {
            _artistService.DeleteAllArtists();
            return NoContent();
        }

        // GET: api/artists/{id}/songs
        [HttpGet("{id}/songs")]
        public ActionResult<IEnumerable<SongResponseDto>> GetSongsByArtist(int id)
        {
            var artistSongs = _artistService.GetSongsByArtist(id);
            if (artistSongs == null)
            {
                return NotFound($"Artist with ID {id} not found.");
            }

            return Ok(artistSongs);
        }
    }
}