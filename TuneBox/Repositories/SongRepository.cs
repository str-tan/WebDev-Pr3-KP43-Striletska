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

        // Отримання всіх пісень із БД
        public IEnumerable<Song> GetAll()
        {
            _logger.LogInformation("DB: Fetching all songs");
            return _context.Songs.ToList();
        }

        // Отримання пісні за ID із БД
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

        // Отримання всіх пісень конкретного виконавця із БД
        public IEnumerable<Song> GetByArtistId(int artistId)
        {
            _logger.LogInformation("DB: Fetching songs for artist ID {ArtistId}", artistId);
            return _context.Songs.Where(s => s.ArtistId == artistId).ToList();
        }

        // Додавання нової пісні в БД
        public void Add(Song song)
        {
            _context.Songs.Add(song);
            _context.SaveChanges();
            _logger.LogInformation("Song {SongId} ({SongTitle}) created successfully for artist {ArtistId}", song.Id, song.Title, song.ArtistId);
        }

        // Оновлення пісні в БД
        public void Update(Song song)
        {
            _context.Songs.Update(song);
            _context.SaveChanges();
            _logger.LogInformation("Song {SongId} updated successfully", song.Id);
        }

        // Видалення пісні з БД
        public void Delete(Song song)
        {
            _context.Songs.Remove(song);
            _context.SaveChanges();
            _logger.LogInformation("Song {SongId} deleted successfully", song.Id);
        }

        // Видалення всіх пісень із БД
        public void DeleteAll()
        {
            _context.Songs.RemoveRange(_context.Songs);
            _context.SaveChanges();
            _logger.LogWarning("All songs were deleted from database");
        }
    }
}