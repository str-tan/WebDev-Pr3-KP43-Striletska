namespace TuneBox.Options
{
    public class TuneBoxOptions
    {
        public const string SectionName = "TuneBox";

        // Максимальна кількість пісень в одного виконавця
        public int MaxSongsPerArtist { get; set; } = 5;

        // Максимальна дозволена тривалість пісні (у секундах)
        public int MaxSongDurationSeconds { get; set; } = 600;

        // Час життя кешу в секундах
        public int CacheDurationSeconds { get; set; } = 60;
    }
}
