using TuneBox.Models;
using TuneBox.Repositories.Interfaces;

namespace TuneBox.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly TuneBoxDbContext _context;
        private readonly ILogger<ArtistRepository> _logger;

        public ArtistRepository(TuneBoxDbContext context, ILogger<ArtistRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Отримання всіх виконавців із БД
        public IEnumerable<Artist> GetAll()
        {
            _logger.LogInformation("DB: Fetching all artists");
            return _context.Artists.ToList();
        }

        // Отримання виконавця за ID із БД
        public Artist? GetById(int id)
        {
            _logger.LogInformation("DB: Fetching artist with ID {ArtistId}", id);
            var artist = _context.Artists.FirstOrDefault(a => a.Id == id);

            if (artist == null)
            {
                _logger.LogWarning("DB: Artist with ID {ArtistId} was not found", id);
            }

            return artist;
        }

        // Додавання нового виконавця в БД
        public void Add(Artist artist)
        {
            _context.Artists.Add(artist);
            _context.SaveChanges();
            _logger.LogInformation("Artist {ArtistId} ({ArtistName}) created successfully", artist.Id, artist.Name);
        }

        // Оновлення виконавця в БД
        public void Update(Artist artist)
        {
            _context.Artists.Update(artist);
            _context.SaveChanges();
            _logger.LogInformation("Artist {ArtistId} updated successfully", artist.Id);
        }

        // Видалення виконавця з БД
        public void Delete(Artist artist)
        {
            _context.Artists.Remove(artist);
            _context.SaveChanges();
            _logger.LogInformation("Artist {ArtistId} deleted successfully", artist.Id);
        }

        // Видалення всіх виконавців із БД
        public void DeleteAll()
        {
            _context.Artists.RemoveRange(_context.Artists);
            _context.SaveChanges();
            _logger.LogWarning("All artists were deleted from database");
        }

        // Перевірка існування виконавця за ID
        public bool Exists(int id)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(id);
            _logger.LogInformation("DB: Checking existence of artist with ID {ArtistId}", id);
            return _context.Artists.Any(a => a.Id == id);
        }
    }
}
