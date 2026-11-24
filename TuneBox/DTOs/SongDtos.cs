namespace TuneBox.DTOs
{
    // Використовується при GET-запитах
    public class SongResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int ArtistId { get; set; }
    }

    // Використовується при POST/PUT-запитах
    public class SongCreateUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int ArtistId { get; set; }
    }
}