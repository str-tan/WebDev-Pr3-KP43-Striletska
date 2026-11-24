namespace TuneBox.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Genre { get; set; } = string.Empty;
    
        // Зовнішній ключ
        public int ArtistId { get; set; }
        // Навігаційне посилання
        public Artist? Artist { get; set; }
    }
}