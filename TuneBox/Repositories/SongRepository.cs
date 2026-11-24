using TuneBox.Models;
using TuneBox.Repositories.Interfaces;

namespace TuneBox.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly TuneBoxDbContext _context;
        public SongRepository(TuneBoxDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Song> GetAll()
        {
            return _context.Songs.ToList();
        }

        public Song? GetById(int id)
        {
            return _context.Songs.FirstOrDefault(s => s.Id == id);
        }

        public IEnumerable<Song> GetByArtistId(int artistId)
        {
            return _context.Songs.Where(s => s.ArtistId == artistId).ToList();
        }

        public void Add(Song song) 
        {
            _context.Songs.Add(song); _context.SaveChanges(); 
        }
        public void Update(Song song) 
        {
            _context.Songs.Update(song); _context.SaveChanges(); 
        }
        public void Delete(Song song) 
        { 
            _context.Songs.Remove(song); _context.SaveChanges();
        }
        public void DeleteAll() 
        {
            _context.Songs.RemoveRange(_context.Songs); _context.SaveChanges(); 
        }
    }
}
