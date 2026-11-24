using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TuneBox.DTOs;
using TuneBox.Models;
using TuneBox.Options;
using TuneBox.Repositories.Interfaces;

namespace TuneBox.Services
{
    public class ArtistService
    {
        // Ключі кешу
        private const string AllArtistsCacheKey = "artists_all";
        private const string AllSongsCacheKey = "songs_all";
        private static string ArtistCacheKey(int id) => $"artist_{id}";
        private static string ArtistSongsCacheKey(int artistId) => $"artist_{artistId}_songs";
        private static string SongCacheKey(int id) => $"song_{id}";

        private readonly IArtistRepository _artistRepository;
        private readonly ISongRepository _songRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly TuneBoxOptions _options;
        private readonly ILogger<ArtistService> _logger;

        public ArtistService(
            IArtistRepository artistRepository,
            ISongRepository songRepository,
            IMapper mapper,
            IMemoryCache cache,
            IOptionsSnapshot<TuneBoxOptions> options,
            ILogger<ArtistService> logger)
        {
            _artistRepository = artistRepository;
            _songRepository = songRepository;
            _mapper = mapper;
            _cache = cache;
            _options = options.Value;
            _logger = logger;
        }

        // Отримання всіх виконавців (із кешуванням)
        public IEnumerable<ArtistResponseDto> GetAllArtists()
        {
            if (_cache.TryGetValue(AllArtistsCacheKey, out IEnumerable<ArtistResponseDto>? cachedArtists)
                && cachedArtists != null)
            {
                _logger.LogInformation("Cache HIT: Returning all artists from cache");
                return cachedArtists;
            }

            _logger.LogInformation("Cache MISS: Loading all artists from repository");
            var artists = _artistRepository.GetAll();
            var result = _mapper.Map<IEnumerable<ArtistResponseDto>>(artists);

            // Збереження результату в кеш на час із конфігурації
            _cache.Set(AllArtistsCacheKey, result, TimeSpan.FromSeconds(_options.CacheDurationSeconds));
            return result;
        }

        // Отримання виконавця за ID (із кешуванням)
        public ArtistResponseDto? GetArtistById(int id)
        {
            var cacheKey = ArtistCacheKey(id);
            if (_cache.TryGetValue(cacheKey, out ArtistResponseDto? cachedArtist))
            {
                _logger.LogInformation("Cache HIT: Returning artist {ArtistId} from cache", id);
                return cachedArtist;
            }

            _logger.LogInformation("Cache MISS: Loading artist {ArtistId} from repository", id);
            var artist = _artistRepository.GetById(id);
            if (artist == null) return null;

            var result = _mapper.Map<ArtistResponseDto>(artist);
            _cache.Set(cacheKey, result, TimeSpan.FromSeconds(_options.CacheDurationSeconds));
            return result;
        }

        // Створення нового виконавця
        public ArtistResponseDto CreateArtist(ArtistCreateUpdateDto newArtistDto)
        {
            var artist = _mapper.Map<Artist>(newArtistDto);
            _artistRepository.Add(artist);

            // Інвалідація кешу списку виконавців
            _cache.Remove(AllArtistsCacheKey);
            return _mapper.Map<ArtistResponseDto>(artist);
        }

        // Оновлення виконавця
        public bool UpdateArtist(int id, ArtistCreateUpdateDto updatedArtistDto)
        {
            var existingArtist = _artistRepository.GetById(id);
            if (existingArtist == null) return false;

            _mapper.Map(updatedArtistDto, existingArtist);
            _artistRepository.Update(existingArtist);

            // Інвалідація кешу списку та конкретного виконавця
            _cache.Remove(AllArtistsCacheKey);
            _cache.Remove(ArtistCacheKey(id));
            return true;
        }

        // Видалення виконавця
        public bool DeleteArtist(int id)
        {
            var artistToRemove = _artistRepository.GetById(id);
            if (artistToRemove == null) return false;

            // Запам'ятовуємо пісні виконавця до каскадного видалення, щоб очистити їх із кешу
            var artistSongs = _songRepository.GetByArtistId(id).ToList();

            _artistRepository.Delete(artistToRemove);

            // Очищення кешу виконавця та його пісень
            _cache.Remove(AllArtistsCacheKey);
            _cache.Remove(ArtistCacheKey(id));
            _cache.Remove(ArtistSongsCacheKey(id));

            if (artistSongs.Any())
            {
                _cache.Remove(AllSongsCacheKey);
                foreach (var song in artistSongs)
                {
                    _cache.Remove(SongCacheKey(song.Id));
                }
            }

            return true;
        }

        // Видалення всіх виконавців
        public void DeleteAllArtists()
        {
            _artistRepository.DeleteAll();

            // Повне очищення кешу в пам'яті
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Clear();
            }
        }

        // Отримання всіх пісень конкретного виконавця (із кешуванням)
        public IEnumerable<SongResponseDto>? GetSongsByArtist(int id)
        {
            var cacheKey = ArtistSongsCacheKey(id);
            if (_cache.TryGetValue(cacheKey, out IEnumerable<SongResponseDto>? cachedSongs)
                && cachedSongs != null)
            {
                _logger.LogInformation("Cache HIT: Returning songs for artist {ArtistId} from cache", id);
                return cachedSongs;
            }

            _logger.LogInformation("Cache MISS: Loading songs for artist {ArtistId} from repository", id);
            if (!_artistRepository.Exists(id)) return null;

            var songs = _songRepository.GetByArtistId(id);
            var result = _mapper.Map<IEnumerable<SongResponseDto>>(songs);

            _cache.Set(cacheKey, result, TimeSpan.FromSeconds(_options.CacheDurationSeconds));
            return result;
        }
    }
}