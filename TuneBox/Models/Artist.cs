namespace TuneBox.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        // Навігаційна колекція (один виконавець має багато пісень)
        public List<Song> Songs { get; set; } = new List<Song>();
    }
}