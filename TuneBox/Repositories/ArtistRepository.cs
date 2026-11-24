using TuneBox.Models;
using TuneBox.Repositories.Interfaces;

namespace TuneBox.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly TuneBoxDbContext _context;
        public ArtistRepository(TuneBoxDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Artist> GetAll() => _context.Artists.ToList();
        public Artist? GetById(int id)
        {
            return _context.Artists.FirstOrDefault(a => a.Id == id);
        }

        public void Add(Artist artist) 
        {
            _context.Artists.Add(artist); _context.SaveChanges();
        }
        public void Update(Artist artist) 
        {
            _context.Artists.Update(artist); _context.SaveChanges(); 
        }
        public void Delete(Artist artist) 
        {
            _context.Artists.Remove(artist); _context.SaveChanges(); 
        }
        public void DeleteAll() 
        { 
            _context.Artists.RemoveRange(_context.Artists); _context.SaveChanges(); 
        }
        public bool Exists(int id)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(id);
            return _context.Artists.Any(a => a.Id == id);
        }
    }
}
