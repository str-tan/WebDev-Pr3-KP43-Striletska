using TuneBox.Models;
namespace TuneBox.Repositories.Interfaces
{
    public interface IArtistRepository
    {
        IEnumerable<Artist> GetAll();
        Artist? GetById(int id);
        void Add(Artist artist);
        void Update(Artist artist);
        void Delete(Artist artist);
        void DeleteAll();
        bool Exists(int id);
    }
}
