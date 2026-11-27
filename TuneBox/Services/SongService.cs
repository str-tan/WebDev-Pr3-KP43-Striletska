using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TuneBox.DTOs;
using TuneBox.Models;
using TuneBox.Options;
using TuneBox.Repositories.Interfaces;

namespace TuneBox.Services
{
    public class SongService
    {
        // Ключі кешу
        private const string AllSongsCacheKey = "songs_all";
        private static string SongCacheKey(int id) => $"song_{id}";
        private static string ArtistSongsCacheKey(int artistId) => $"artist_{artistId}_songs";

        private readonly ISongRepository _songRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IOptionsMonitor<TuneBoxOptions> _optionsMonitor;
        private readonly ILogger<SongService> _logger;

        public SongService(
            ISongRepository songRepository,
            IArtistRepository artistRepository,
            IMapper mapper,
            IMemoryCache cache,
            IOptionsMonitor<TuneBoxOptions> optionsMonitor,
            ILogger<SongService> logger)
        {
            _songRepository = songRepository;
            _artistRepository = artistRepository;
            _mapper = mapper;
            _cache = cache;
            _optionsMonitor = optionsMonitor;
            _logger = logger;
        }


        public IEnumerable<SongResponseDto> GetAllSongs()
        {
            if (_cache.TryGetValue(AllSongsCacheKey, out IEnumerable<SongResponseDto>? cachedSongs)
                && cachedSongs != null)
            {
                _logger.LogInformation("Cache HIT: Returning all songs from cache");
                return cachedSongs;
            }

            _logger.LogInformation("Cache MISS: Loading all songs from repository");
            var songs = _songRepository.GetAll();
            var result = _mapper.Map<IEnumerable<SongResponseDto>>(songs);

            // Збереження результату в кеш на час із поточної конфігурації
            var cacheDuration = _optionsMonitor.CurrentValue.CacheDurationSeconds;
            _cache.Set(AllSongsCacheKey, result, TimeSpan.FromSeconds(cacheDuration));
            return result;
        }


        public SongResponseDto? GetSongById(int id)
        {
            var cacheKey = SongCacheKey(id);
            if (_cache.TryGetValue(cacheKey, out SongResponseDto? cachedSong))
            {
                _logger.LogInformation("Cache HIT: Returning song {SongId} from cache", id);
                return cachedSong;
            }

            _logger.LogInformation("Cache MISS: Loading song {SongId} from repository", id);
            var song = _songRepository.GetById(id);
            if (song == null) return null;

            var result = _mapper.Map<SongResponseDto>(song);
            var cacheDuration = _optionsMonitor.CurrentValue.CacheDurationSeconds;
            _cache.Set(cacheKey, result, TimeSpan.FromSeconds(cacheDuration));
            return result;
        }


        public (SongResponseDto? SongDto, string? ErrorMessage) CreateSong(SongCreateUpdateDto newSongDto)
        {
            if (!_artistRepository.Exists(newSongDto.ArtistId))
            {
                return (null, $"Artist with ID {newSongDto.ArtistId} does not exist.");
            }

            var options = _optionsMonitor.CurrentValue;

            // Перевірка максимальної тривалості пісні з поточної конфігурації
            if (newSongDto.Duration > options.MaxSongDurationSeconds)
            {
                _logger.LogWarning(
                    "Blocked song creation: duration {Duration}s exceeds MaxSongDurationSeconds ({Limit}s)",
                    newSongDto.Duration, options.MaxSongDurationSeconds);

                return (null, $"Song duration ({newSongDto.Duration}s) exceeds the maximum allowed limit ({options.MaxSongDurationSeconds}s).");
            }

            // Перевірка ліміту кількості пісень у виконавця
            var currentSongCount = _songRepository.GetByArtistId(newSongDto.ArtistId).Count();
            if (currentSongCount >= options.MaxSongsPerArtist)
            {
                _logger.LogWarning(
                    "Blocked song creation for artist {ArtistId}: reached MaxSongsPerArtist limit ({Limit})",
                    newSongDto.ArtistId, options.MaxSongsPerArtist);

                return (null, $"Artist with ID {newSongDto.ArtistId} already has the maximum allowed number of songs ({options.MaxSongsPerArtist}).");
            }

            var song = _mapper.Map<Song>(newSongDto);
            _songRepository.Add(song);

            // Інвалідація кешу списку пісень та пісень виконавця
            _cache.Remove(AllSongsCacheKey);
            _cache.Remove(ArtistSongsCacheKey(song.ArtistId));

            return (_mapper.Map<SongResponseDto>(song), null);
        }


        public (bool IsSuccess, bool IsNotFound, string? ErrorMessage) UpdateSong(int id, SongCreateUpdateDto updatedSongDto)
        {
            var existingSong = _songRepository.GetById(id);
            if (existingSong == null) return (false, true, null);

            if (!_artistRepository.Exists(updatedSongDto.ArtistId))
            {
                return (false, false, $"Artist with ID {updatedSongDto.ArtistId} does not exist.");
            }

            var options = _optionsMonitor.CurrentValue;

            // Перевірка максимальної тривалості пісні з поточної конфігурації
            if (updatedSongDto.Duration > options.MaxSongDurationSeconds)
            {
                _logger.LogWarning(
                    "Blocked updating song {SongId}: duration {Duration}s exceeds MaxSongDurationSeconds ({Limit}s)",
                    id, updatedSongDto.Duration, options.MaxSongDurationSeconds);

                return (false, false, $"Song duration ({updatedSongDto.Duration}s) exceeds the maximum allowed limit ({options.MaxSongDurationSeconds}s).");
            }

            // Якщо пісню переносять до іншого виконавця — перевіряємо його ліміт пісень
            var oldArtistId = existingSong.ArtistId;
            if (oldArtistId != updatedSongDto.ArtistId)
            {
                var targetSongCount = _songRepository.GetByArtistId(updatedSongDto.ArtistId).Count();
                if (targetSongCount >= options.MaxSongsPerArtist)
                {
                    _logger.LogWarning(
                        "Blocked moving song {SongId} to artist {ArtistId}: reached MaxSongsPerArtist limit ({Limit})",
                        id, updatedSongDto.ArtistId, options.MaxSongsPerArtist);

                    return (false, false, $"Artist with ID {updatedSongDto.ArtistId} already has the maximum allowed number of songs ({options.MaxSongsPerArtist}).");
                }
            }

            _mapper.Map(updatedSongDto, existingSong);
            _songRepository.Update(existingSong);

            // Інвалідація кешу пісні, загального списку та списків обох виконавців
            _cache.Remove(AllSongsCacheKey);
            _cache.Remove(SongCacheKey(id));
            _cache.Remove(ArtistSongsCacheKey(oldArtistId));
            _cache.Remove(ArtistSongsCacheKey(updatedSongDto.ArtistId));

            return (true, false, null);
        }


        public bool DeleteSong(int id)
        {
            var songToRemove = _songRepository.GetById(id);
            if (songToRemove == null) return false;

            var artistId = songToRemove.ArtistId;
            _songRepository.Delete(songToRemove);

            // Інвалідація кешу пісні та пов'язаних списків
            _cache.Remove(AllSongsCacheKey);
            _cache.Remove(SongCacheKey(id));
            _cache.Remove(ArtistSongsCacheKey(artistId));

            return true;
        }


        public void DeleteAllSongs()
        {
            _songRepository.DeleteAll();

            // Повне очищення кешу в пам'яті
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Clear();
            }
        }
    }
}