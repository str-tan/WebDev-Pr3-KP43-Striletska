namespace TuneBox.DTOs
{
    // Використовується при GET-запитах
    public class ArtistResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    // Використовується при POST/PUT-запитах
    public class ArtistCreateUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}