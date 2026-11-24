using Microsoft.AspNetCore.Mvc;
using TuneBox.DTOs;
using TuneBox.Services;

namespace TuneBox.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly SongService _songService;

        public SongsController(SongService songService)
        {
            _songService = songService;
        }

        // GET: api/songs
        [HttpGet]
        public ActionResult<IEnumerable<SongResponseDto>> GetAllSongs()
        {
            return Ok(_songService.GetAllSongs());
        }

        // GET: api/songs/{id}
        [HttpGet("{id}")]
        public ActionResult<SongResponseDto> GetSongById(int id)
        {
            var song = _songService.GetSongById(id);
            if (song == null)
            {
                return NotFound($"Song with ID {id} not found.");
            }

            return Ok(song);
        }

        // GET: api/songs/artist/{artistId}
        [HttpPost]
        public ActionResult<SongResponseDto> CreateSong([FromBody] SongCreateUpdateDto newSong)
        {
            var result = _songService.CreateSong(newSong);

            if (result.ErrorMessage != null)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(nameof(GetSongById), new { id = result.SongDto!.Id }, result.SongDto);
        }

        // PUT: api/songs/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateSong(int id, [FromBody] SongCreateUpdateDto updatedSong)
        {
            var result = _songService.UpdateSong(id, updatedSong);

            if (result.IsNotFound)
            {
                return NotFound($"Song with ID {id} not found.");
            }

            if (result.ErrorMessage != null)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        // DELETE: api/songs/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteSong(int id)
        {
            var isDeleted = _songService.DeleteSong(id);
            if (!isDeleted)
            {
                return NotFound($"Song with ID {id} not found.");
            }

            return NoContent();
        }

        // DELETE: api/songs
        [HttpDelete]
        public IActionResult DeleteAllSongs()
        {
            _songService.DeleteAllSongs();
            return NoContent();
        }
    }
}