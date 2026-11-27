using TuneBox.Models;
using TuneBox.Repositories.Interfaces;

namespace TuneBox.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly TuneBoxDbContext _context;
        private readonly ILogger<SongRepository> _logger;

        public SongRepository(TuneBoxDbContext context, ILogger<SongRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

    
        public IEnumerable<Song> GetAll()
        {
            _logger.LogInformation("DB: Fetching all songs");
            return _context.Songs.ToList();
        }

       
        public Song? GetById(int id)
        {
            _logger.LogInformation("DB: Fetching song with ID {SongId}", id);
            var song = _context.Songs.FirstOrDefault(s => s.Id == id);

            if (song == null)
            {
                _logger.LogWarning("DB: Song with ID {SongId} was not found", id);
            }

            return song;
        }

        
        public IEnumerable<Song> GetByArtistId(int artistId)
        {
            _logger.LogInformation("DB: Fetching songs for artist ID {ArtistId}", artistId);
            return _context.Songs.Where(s => s.ArtistId == artistId).ToList();
        }

        
        public void Add(Song song)
        {
            _context.Songs.Add(song);
            _context.SaveChanges();
            _logger.LogInformation("Song {SongId} ({SongTitle}) created successfully for artist {ArtistId}", song.Id, song.Title, song.ArtistId);
        }

       
        public void Update(Song song)
        {
            _context.Songs.Update(song);
            _context.SaveChanges();
            _logger.LogInformation("Song {SongId} updated successfully", song.Id);
        }

    
        public void Delete(Song song)
        {
            _context.Songs.Remove(song);
            _context.SaveChanges();
            _logger.LogInformation("Song {SongId} deleted successfully", song.Id);
        }

       
        public void DeleteAll()
        {
            _context.Songs.RemoveRange(_context.Songs);
            _context.SaveChanges();
            _logger.LogWarning("All songs were deleted from database");
        }
    }
}