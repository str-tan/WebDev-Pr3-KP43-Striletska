using TuneBox.Models;
namespace TuneBox.Repositories.Interfaces
{
    public interface ISongRepository
    {
        IEnumerable<Song> GetAll();
        Song? GetById(int id);
        IEnumerable<Song> GetByArtistId(int artistId);
        void Add(Song song);
        void Update(Song song);
        void Delete(Song song);
        void DeleteAll();
    }
}
